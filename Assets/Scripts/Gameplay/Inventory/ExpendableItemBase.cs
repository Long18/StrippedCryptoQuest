using System;
using CryptoQuest.Gameplay.Inventory.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.AbilitySystem.Components;

namespace CryptoQuest.Gameplay.Inventory
{
    [Serializable]
    public class ExpendableItemBase : ItemBase
    {
        public new ExpendableItemSO ItemSO => (ExpendableItemSO)base.ItemSO;
        public AbilitySystemBehaviour Owner;
        public ExpendableItemBase(ItemSO itemSO, int quantity = 0) : base(itemSO, quantity) { }

        public ExpendableItemBase(AbilitySystemBehaviour owner, ItemSO itemSO, int quantity = 0) : base(itemSO,
            quantity)
        {
            Owner = owner;
        }


        public override void Use()
        {
            if (Owner == null) return;
            var ability = Owner.GiveAbility(ItemSO.Ability);
            Owner.TryActiveAbility(ability);
        }
    }
}