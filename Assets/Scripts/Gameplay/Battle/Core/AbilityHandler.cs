using CryptoQuest.Events.Gameplay;
using IndiGames.GameplayAbilitySystem.AbilitySystem.Components;
using IndiGames.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using UnityEngine;

namespace CryptoQuest.Gameplay.Battle.Core
{
    public class AbilityHandler : MonoBehaviour
    {
        [SerializeField] private AbilityEventChannelSO _abilityActivatedEventChannel;
        [SerializeField] private AbilitySystemBehaviour _abilitySystemBehaviour;
#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateComponents();
        }
#endif

        private void ValidateComponents()
        {
            if (_abilitySystemBehaviour == null)
            {
                _abilitySystemBehaviour = GetComponent<AbilitySystemBehaviour>();
            }
        }

        private void OnEnable()
        {
            _abilityActivatedEventChannel.EventRaised += ActivateSkill;
        }

        private void OnDisable()
        {
            _abilityActivatedEventChannel.EventRaised -= ActivateSkill;
        }

        public void ActivateSkill(AbilityScriptableObject abilityScriptableObject)
        {
            var ability = abilityScriptableObject.GetAbilitySpec(_abilitySystemBehaviour);
            ability.ActivateAbility();
        }
    }
}