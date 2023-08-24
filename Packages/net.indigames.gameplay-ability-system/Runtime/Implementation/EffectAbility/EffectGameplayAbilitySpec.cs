using System.Collections;
using System.Collections.Generic;
using IndiGames.GameplayAbilitySystem.AbilitySystem;
using IndiGames.GameplayAbilitySystem.AbilitySystem.Components;
using IndiGames.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.EffectSystem;
using IndiGames.GameplayAbilitySystem.EffectSystem.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.Helper;
using IndiGames.GameplayAbilitySystem.Implementation.EffectAbility.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.TagSystem.ScriptableObjects;
using UnityEngine;

namespace IndiGames.GameplayAbilitySystem.Implementation.EffectAbility
{
    public class EffectGameplayAbilitySpec : GameplayAbilitySpec
    {
        public class EffectAbilityContext
        {
            public List<AbilitySystemBehaviour> AffectTargets = new List<AbilitySystemBehaviour>();
        }

        private Dictionary<TagScriptableObject, List<GameplayEffectSpec>> _effectTagDict =
            new Dictionary<TagScriptableObject, List<GameplayEffectSpec>>();

        protected new EffectAbilitySO AbilitySO => (EffectAbilitySO) _abilitySO;

        protected override IEnumerator InternalActiveAbility()
        {
            // Try active ability with tag OnActive
            foreach (var effectContainer in AbilitySO.EffectContainerMap)
            {
                if (effectContainer.Tag == null) continue;
                if (effectContainer.Tag.name == "OnActive")
                {
                    ApplyEffectContainerByTag(effectContainer.Tag, new EffectAbilityContext());
                }
            }

            Debug.Log($"EffectAbility::InternalActiveAbility {AbilitySO.name} Activated");
            yield break;
        }

        public List<GameplayEffectSpec> ApplyEffectContainerByTag(TagScriptableObject tag, EffectAbilityContext context)
        {
            if (!IsActive)
            {
                Debug.Log($"EffectAbility::ApplyEffectContainerByTag: {AbilitySO.name} is not active");
                return new List<GameplayEffectSpec>();
            }

            var specs = CreateEffectContainerSpec(tag, context);
            Debug.Log($"EffectAbility::ApplyEffectWithTags - {tag.name}");

            var returnEffects = new List<GameplayEffectSpec>();
            foreach (var spec in specs)
            {
                returnEffects.AddRange(ApplyEffectContainerSpec(spec, context));
            }

            if (!_effectTagDict.ContainsKey(tag))
            {
                _effectTagDict.Add(tag, new List<GameplayEffectSpec>());
            }
            _effectTagDict[tag].AddRange(returnEffects);

            return returnEffects;
        }

        protected List<AbilityEffectContainerSpec> CreateEffectContainerSpec(TagScriptableObject tag,
            EffectAbilityContext context)
        {
            List<AbilityEffectContainerSpec> returnSpecs = new List<AbilityEffectContainerSpec>();
            foreach (var effectContainer in AbilitySO.EffectContainerMap)
            {
                if (effectContainer.Tag == null) continue;
                if (effectContainer.Tag == tag)
                {
                    foreach (var container in effectContainer.TargetContainer)
                    {
                        returnSpecs.Add(CreateEffectContainerSpecFromContainer(container, context));
                    }
                }
            }

            return returnSpecs;
        }

        protected AbilityEffectContainerSpec CreateEffectContainerSpecFromContainer(AbilityEffectContainer container,
            EffectAbilityContext context)
        {
            var returnSpec = new AbilityEffectContainerSpec();
            if (Owner == null) return returnSpec;

            var targets = new List<AbilitySystemBehaviour>();
            if (container.TargetType)
            {
                container.TargetType.GetTargets(Owner, ref targets);
            }
            else
            {
                targets.AddRange(context.AffectTargets);
            }
            returnSpec.AddTargets(targets);

            var effects = container.Effects;
            if (effects == null) return returnSpec;

            foreach (var effect in effects)
            {
                if (effect == null) continue;
                var effectSpec = CreateEffectSpec(effect);
                returnSpec.EffectSpecs.Add(effectSpec);
            }

            return returnSpec;
        }

        public virtual GameplayEffectSpec CreateEffectSpec(EffectScriptableObject effectScriptableObject)
        {
            return Owner.EffectSystem.GetEffect(effectScriptableObject);
        }

        protected virtual List<GameplayEffectSpec> ApplyEffectContainerSpec(AbilityEffectContainerSpec abilityEffectSpec,
            EffectAbilityContext context)
        {
            var appliedEffect = new List<GameplayEffectSpec>();

            foreach (var effectSpec in abilityEffectSpec.EffectSpecs)
            {
                foreach (var target in abilityEffectSpec.Targets)
                {
                    // appliedEffect.AddRange(AbilitySystemHelper.ApplyEffectSpecToTarget(effectSpec, target));
                }
            }

            return appliedEffect;
        }

        public override void EndAbility()
        {
            base.EndAbility();
            
            foreach (var tagEffect in _effectTagDict)
            {
                var effectSpecs = tagEffect.Value;
                for (int i = 0; i < effectSpecs.Count; i++)
                {
                    var effectSpec = effectSpecs[i];
                    if (effectSpec.RemoveWhenAbilityEnd)
                        Owner.EffectSystem.RemoveEffect(effectSpec);
                }
            }

            _effectTagDict.Clear();
        }

        public override void OnAbilityRemoved(GameplayAbilitySpec gameplayAbilitySpecSpec)
        {
            base.OnAbilityRemoved(gameplayAbilitySpecSpec);
            EndAbility();
        }

        public void RemoveEffectWithTag(TagScriptableObject abilityTag)
        {
            if (!_effectTagDict.ContainsKey(abilityTag)) return;
            foreach (var effectSpec in _effectTagDict[abilityTag])
            {
                effectSpec.Target.EffectSystem.RemoveEffect(effectSpec);
            }

            _effectTagDict.Remove(abilityTag);
        }
    }
}