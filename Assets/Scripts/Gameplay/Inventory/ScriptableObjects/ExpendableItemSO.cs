using System.Collections;
using System.Collections.Generic;
using CryptoQuest.Gameplay.Battle.Core.ScriptableObjects.Skills;
using IndiGames.GameplayAbilitySystem.AbilitySystem.Components;
using IndiGames.GameplayAbilitySystem.AbilitySystem.ScriptableObjects;
using UnityEngine;

namespace CryptoQuest.Gameplay.Inventory.ScriptableObjects

{
    public class ExpendableItemSO : ItemSO
    {
        public AbilityScriptableObject Ability;

        public override void Use(AbilitySystemBehaviour owner = null)
        {
            var ability = owner.GiveAbility(Ability);
            owner.TryActiveAbility(ability);
        }
    }
}