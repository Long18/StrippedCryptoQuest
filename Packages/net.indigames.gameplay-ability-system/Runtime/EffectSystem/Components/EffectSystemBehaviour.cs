using System;
using System.Collections.Generic;
using IndiGames.GameplayAbilitySystem.AbilitySystem.Components;
using IndiGames.GameplayAbilitySystem.AttributeSystem.Components;
using IndiGames.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace IndiGames.GameplayAbilitySystem.EffectSystem.Components
{
    /// <summary>
    /// Wrapper around the <see cref="AttributeSystemBehaviour"/> to handle the effects
    /// for every applied effect find all of it modifiers and add it to the attribute in the <see cref="AttributeSystemBehaviour"/>
    /// </summary>
    [RequireComponent(typeof(AttributeSystemBehaviour))]
    public class EffectSystemBehaviour : MonoBehaviour
    {
        public event Action<ActiveEffectSpecification> EffectAdded;
        public event Action<ActiveEffectSpecification> EffectRemoved;

        [SerializeField] private bool _useUpdate;

        /// <summary>
        /// Currently there are no restrictions on add a new effect to the system except
        /// when using <see cref="ApplyEffectToSelf"/> which will check <see cref="GameplayEffectSpec.CanApply"/>
        /// </summary>
        [SerializeReference] private List<ActiveEffectSpecification> _appliedEffects = new();

        public IReadOnlyList<ActiveEffectSpecification> AppliedEffects => _appliedEffects;
        private AbilitySystemBehaviour _owner;

        public AbilitySystemBehaviour Owner
        {
            get => _owner;
            set => _owner = value;
        }

        private AttributeSystemBehaviour _attributeSystem;

        private void Awake()
        {
            _owner = GetComponent<AbilitySystemBehaviour>();
            _attributeSystem = GetComponent<AttributeSystemBehaviour>();
        }

        /// <summary>
        /// Will create a new AbstractEffect from EffectScriptableObject (data)
        /// this will update the Owner of the effect to this AbilitySystem
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public GameplayEffectSpec GetEffect(GameplayEffectDefinition def)
            => def.CreateEffectSpec(Owner);

        // TODO: Move to AbilityEffectBehaviour
        /// <summary>
        /// AbilitySystemComponent.cpp::ApplyGameplayEffectSpecToSelf::line 730
        ///
        /// Create an active effect spec, apply into the system and update the attribute accordingly in this frame
        /// </summary>
        /// <param name="inSpec"></param>
        /// <returns></returns>
        public ActiveEffectSpecification ApplyEffectToSelf(GameplayEffectSpec inSpec)
        {
            if (inSpec == null || !inSpec.CanApply()) return new ActiveEffectSpecification();

            if (inSpec.Def.IsStack && TryGetActiveEffectWithSameDef(inSpec.Def, out var activeEffect))
            {
                activeEffect.UpdateStackCount(inSpec);
                UpdateAttributeSystemModifiers();
                UpdateEffects();
                if (activeEffect.IsActive) EffectAdded?.Invoke(activeEffect);
                return activeEffect;
            }

            inSpec.Target = Owner;
            var activeEffectSpecification = inSpec.CreateActiveEffectSpec(_owner);
            _appliedEffects.Add(activeEffectSpecification);
            Owner.TagSystem.AddTags(activeEffectSpecification.GrantedTags);
            UpdateAttributeSystemModifiers();
            UpdateEffects();
            // TODO: The check currently only to skip instant effect, could've done better
            if (activeEffectSpecification.IsActive) EffectAdded?.Invoke(activeEffectSpecification);
            return activeEffectSpecification;
        }

        private bool TryGetActiveEffectWithSameDef(GameplayEffectDefinition def,
            out ActiveEffectSpecification effectSpecification)
        {
            foreach (var effect in _appliedEffects)
            {
                if (effect.Spec.Def == def)
                {
                    effectSpecification = effect;
                    return true;
                }
            }

            effectSpecification = new ActiveEffectSpecification();
            return false;
        }

        public void ExpireEffectWithTagImmediately(TagScriptableObject tag)
        {
            ExpireEffectWithTag(tag);
            UpdateAttributeModifiersUsingAppliedEffects();
        }

        public void ExpireEffectWithTag(TagScriptableObject tag)
        {
            foreach (var appliedEffect in _appliedEffects)
            {
                if (appliedEffect.Expired) continue;
                if (!appliedEffect.HasTag(tag)) continue;
                appliedEffect.IsActive = false;
            }
        }

        /// <summary>
        /// Remove the effect from the system
        /// We should also remove the effect's modifiers from the attribute
        /// </summary>
        public virtual void RemoveEffect(GameplayEffectSpec effectSpec)
        {
            if (effectSpec == null || !effectSpec.IsValid())
            {
                Debug.LogWarning("Try remove invalid effect");
                return;
            }

            for (int i = _appliedEffects.Count - 1; i >= 0; i--)
            {
                var effect = _appliedEffects[i];
                if (effectSpec.CompareTo(effect.Spec) != 1) continue;

                RemoveEffectAtIndex(i);
                break;
            }

            // after remove the effect from system we need to update the attribute modifiers
            ForceUpdateAttributeSystemModifiers();
        }

        private void Update()
        {
            if (_useUpdate) UpdateAttributeModifiersUsingAppliedEffects();
        }

        public void UpdateAttributeModifiersUsingAppliedEffects()
        {
            UpdateAttributeSystemModifiers();
            UpdateEffects();
            RemoveExpiredEffects();
        }

        /// <summary>
        /// Force the system to check all effects and update their status
        /// </summary>
        public void ForceUpdateAttributeSystemModifiers()
        {
            UpdateAttributeSystemModifiers();
            _attributeSystem.UpdateAttributeValues();
        }

        /// <summary>
        /// 1. Reset all attributes to their base value
        /// 2. Add all modifiers from all active effects
        /// </summary>
        protected virtual void UpdateAttributeSystemModifiers()
        {
            _attributeSystem.ResetAttributeModifiers();
            for (var index = 0; index < _appliedEffects.Count; index++)
            {
                var effect = _appliedEffects[index];
                if (effect.Expired) continue;
                AddModifiersToAttributeWithEffect(effect);
            }
        }

        private void AddModifiersToAttributeWithEffect(ActiveEffectSpecification activeEffect)
        {
            if (activeEffect.Expired) return;
            if (activeEffect.CanApplyModifiersToAttributeSystem() == false) return;

            for (var index = 0; index < activeEffect.ComputedModifiers.Count; index++)
            {
                var computedModifier = activeEffect.ComputedModifiers[index];

                AddAttributeToSystemIfNotExists(computedModifier.Attribute);
                _attributeSystem.TryAddModifierToAttribute(
                    computedModifier.Modifier,
                    computedModifier.Attribute,
                    activeEffect.ModifierType);
            }
        }

        /// <summary>
        /// The case is we have an effect with modifier want to affect an attribute that is not in the system yet
        ///
        /// e.g. Modifier to increase gold drop rate, but the attribute system does not have gold drop rate attribute.
        /// We can either add the attribute to the system or this method would add it for us. only at runtime
        /// </summary>
        private void AddAttributeToSystemIfNotExists(AttributeScriptableObject attribute)
        {
            if (!_attributeSystem.HasAttribute(attribute, out _))
                _attributeSystem.AddAttribute(attribute);
        }

        protected virtual void UpdateEffects()
        {
            for (var index = 0; index < _appliedEffects.Count; index++)
            {
                var effectContainer = _appliedEffects[index];
                if (effectContainer != null && !effectContainer.Expired)
                    effectContainer.Update(Time.deltaTime);
            }
        }

        protected virtual void RemoveExpiredEffects()
        {
            for (var i = _appliedEffects.Count - 1; i >= 0; i--)
            {
                var effect = _appliedEffects[i];
                if (effect != null && effect.IsValid() && !effect.Expired) continue;
                RemoveEffectAtIndex(i);
            }
        }

        public List<TagScriptableObject> GrantedTags => _owner.TagSystem.GrantedTags;

        /// <summary>
        /// Tests if all modifiers in this GameplayEffect will leave the attribute > 0.f
        /// </summary>
        /// <param name="effectDef"></param>
        /// <returns></returns>
        public bool CanApplyAttributeModifiers<TDef>(TDef effectDef) where TDef : GameplayEffectDefinition
        {
            var spec = new GameplayEffectSpec();
            spec.InitEffect(effectDef, _owner);
            spec.CalculateModifierMagnitudes();

            for (int i = 0; i < spec.Modifiers.Length; i++)
            {
                var modDef = effectDef.EffectDetails.Modifiers[i];
                var modSpec = spec.Modifiers[i];

                // Only worry about additive.  Anything else passes.
                if (modDef.ModifierType != EAttributeModifierType.Add) continue;
                if (modDef.Attribute == null) continue;

                if (!_attributeSystem.TryGetAttributeValue(modDef.Attribute, out var attributeValue)) continue;
                var hasEnoughResource = attributeValue.CurrentValue + modSpec.GetEvaluatedMagnitude() < 0;
                if (hasEnoughResource) return false;
            }

            return true;
        }

        private void OnDestroy()
        {
            RemoveAllEffects();
        }

        private void RemoveAllEffects()
        {
            for (int i = _appliedEffects.Count - 1; i >= 0; i--)
            {
                RemoveEffectAtIndex(i);
            }
        }

        private void RemoveEffectAtIndex(int index)
        {
            var effect = _appliedEffects[index];
            _appliedEffects.RemoveAt(index);
            if (effect?.Spec == null) return;
            Owner.TagSystem.RemoveTags(effect.GrantedTags);
            effect.Release();
            EffectRemoved?.Invoke(effect);
        }
    }
}