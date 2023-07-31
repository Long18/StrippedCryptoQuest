using CryptoQuest.Gameplay.Battle.Core.Components.BattleUnit;
using CryptoQuest.Gameplay.Battle.Core.ScriptableObjects.Data;
using IndiGames.GameplayAbilitySystem.AbilitySystem.Components;
using IndiGames.GameplayAbilitySystem.AttributeSystem;
using IndiGames.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.EffectSystem;
using IndiGames.GameplayAbilitySystem.EffectSystem.ScriptableObjects.EffectExecutionCalculation;
using UnityEngine;
using CryptoQuest.Gameplay.Battle.Core.ScriptableObjects.Events;

namespace CryptoQuest.Gameplay.Battle.Core.ScriptableObjects.Effects.EffectExecutionCalculation
{
    [CreateAssetMenu(fileName = "DamageCaculation", menuName = "Gameplay/Battle/Effects/Execution Calculations/Damage Caculation")]
    public class DamageCalculationSO : AbstractEffectExecutionCalculationSO
    {
        public BattleActionDataEventChannelSO ActionEventSO;
        public BattleActionDataSO TakeDamageActionData;
        public BattleActionDataSO MissActionData;
        public BattleActionDataSO NoDamageActionData;
        public BattleActionDataSO DeathActionData;
        public AttributeScriptableObject OwnerAttack;
        public AttributeScriptableObject TargetHP;

        private IBattleUnit _ownerUnit;
        private IBattleUnit _targetUnit;

        public override bool ExecuteImplementation(ref AbstractEffect effectSpec,
            ref EffectAttributeModifier[] modifiers)
        {
            effectSpec.Owner.AttributeSystem.GetAttributeValue(OwnerAttack, out var attackDamage);
            _ownerUnit = effectSpec.Owner.GetComponent<IBattleUnit>();
            _targetUnit = effectSpec.Target.GetComponent<IBattleUnit>();

            modifiers = effectSpec.EffectSO.EffectDetails.Modifiers;
            float damageValue = attackDamage.CurrentValue;

            for (var index = 0; index < modifiers.Length; index++)
            {
                EffectAttributeModifier effectAttributeModifier = modifiers[index];
                if (effectAttributeModifier.AttributeSO == TargetHP)
                {
                    EffectAttributeModifier previousModifier = effectAttributeModifier;
                    previousModifier.Value = damageValue * -1;
                    modifiers[index] = previousModifier;

                    LogAfterCalculateDamage(damageValue);
                    LogIfTargetDeath(effectSpec, damageValue);
                    return true;
                }
            }

            return false;
        }

        private void LogAfterCalculateDamage(float damage)
        {
            if (_ownerUnit == null || _targetUnit == null) return;

            TakeDamageActionData.Init(_targetUnit.UnitInfo.Owner);
            TakeDamageActionData.AddStringVar("unitName", _targetUnit.UnitInfo.DisplayName);
            TakeDamageActionData.AddFloatVar("damage", damage);
            ActionEventSO.RaiseEvent(TakeDamageActionData);
        }

        private void LogIfTargetDeath(AbstractEffect effectSpec, float damage)
        {
            AbilitySystemBehaviour target = effectSpec.Target;
            target.AttributeSystem.GetAttributeValue(TargetHP, out AttributeValue value);
            if (value.CurrentValue <= damage)
            {
                DeathActionData.Log.Clear();
                DeathActionData.AddStringVar("unitName", _targetUnit.UnitInfo.DisplayName);
                ActionEventSO.RaiseEvent(DeathActionData);
            }
        }
    }
}