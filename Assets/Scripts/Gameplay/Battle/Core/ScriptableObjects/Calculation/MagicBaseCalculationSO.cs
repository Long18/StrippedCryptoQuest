using IndiGames.GameplayAbilitySystem.AttributeSystem;
using IndiGames.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.EffectSystem;
using IndiGames.GameplayAbilitySystem.EffectSystem.ScriptableObjects.EffectExecutionCalculation;
using UnityEngine;

namespace CryptoQuest.Gameplay.Battle.Core.ScriptableObjects.Calculation
{
    [CreateAssetMenu(fileName = "HealCalculation",
        menuName = "Gameplay/Battle/Effects/Execution Calculations/Heal Calculation")]
    public class MagicBaseCalculationSO : AbstractEffectExecutionCalculationSO
    {
        [SerializeField] private AttributeScriptableObject _baseMagicAttackSO;
        [SerializeField] private AttributeScriptableObject _targetedAttributeSO;
        [SerializeField] private float _lowerRandomRange = -0.05f;
        [SerializeField] private float _upperRandomRange = 0.05f;

        public override bool ExecuteImplementation(ref AbstractEffect effectSpec,
            ref EffectAttributeModifier[] attributeModifiers)
        {
            SkillParameters skillParameters = effectSpec.Parameters as SkillParameters;
            effectSpec.Owner.AttributeSystem.TryGetAttributeValue(_baseMagicAttackSO, out AttributeValue baseMagicAttack);
            float baseMagicValue = BattleCalculator.CalculateBaseDamage(skillParameters, baseMagicAttack.CurrentValue,
                Random.Range(_lowerRandomRange, _upperRandomRange));
            for (var index = 0; index < attributeModifiers.Length; index++)
            {
                if (attributeModifiers[index].AttributeSO != _targetedAttributeSO) continue;

                EffectAttributeModifier previousModifier = attributeModifiers[index];
                attributeModifiers[index].Value = baseMagicValue;

                return true;
            }

            return false;
        }
    }
}