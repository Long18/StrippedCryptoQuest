using CryptoQuest.Events.Gameplay;
using CryptoQuest.Gameplay.Inventory;
using CryptoQuest.Gameplay.Inventory.ScriptableObjects;
using IndiGames.GameplayAbilitySystem.AbilitySystem.Components;
using UnityEngine;

namespace CryptoQuest
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private InventorySO _inventorySO;
        [SerializeField] private ItemEventChannelSO _onEquipItemEventChannel;
        [SerializeField] private AbilitySystemBehaviour CurrentOwnerAbilitySystemBehaviour;

        private void OnEnable()
        {
            _onEquipItemEventChannel.EventRaised += EquipItem;
        }

        private void OnDisable()
        {
            _onEquipItemEventChannel.EventRaised -= EquipItem;
        }

        private void EquipItem(ItemBase itemBaseItem)
        {
            ExpendableItemBase expendableItemBase = itemBaseItem as ExpendableItemBase;
            if (expendableItemBase == null) return;
            expendableItemBase.Owner = CurrentOwnerAbilitySystemBehaviour;
            itemBaseItem.Use();
        }
    }
}