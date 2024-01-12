using System;
using System.Collections.Generic;
using CryptoQuest.Battle.Components;
using CryptoQuest.Character.Hero;
using CryptoQuest.Gameplay.PlayerParty;
using CryptoQuest.Item.Equipment;
using CryptoQuest.UI.Character;
using IndiGames.GameplayAbilitySystem.AttributeSystem.ScriptableObjects;
using UnityEngine;

namespace CryptoQuest.UI.Menu.Character
{
    public struct AttributeUIValue
    {
        public UIAttribute AttributeUI;
        public float CurrentValue;
    }

    public class HeroEquipmentPreviewer : MonoBehaviour
    {
        private Dictionary<AttributeScriptableObject, AttributeUIValue> _cachedAttributes = new();

        private HeroBehaviour _clonedHero;
        public bool IsValid => _clonedHero != null;
        public HeroBehaviour ClonedHero => _clonedHero;

        private void OnEnable()
        {
            if (_clonedHero == null) return;
            _clonedHero.Init(new PartySlotSpec() { Hero = _clonedHero.Spec });
            _clonedHero.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (_clonedHero == null) return;
            _clonedHero.gameObject.SetActive(false);
        }

        public void SetPreviewHero(HeroBehaviour hero)
        {
            Clone(hero);
        }

        public void CacheCurrentAttributes(AttributeScriptableObject attribute, UIAttribute uiAttribute, float value)
        {
            var attributeUIValue = new AttributeUIValue()
            {
                AttributeUI = uiAttribute,
                CurrentValue = value
            };
            if (_cachedAttributes.ContainsKey(attribute))
            {
                _cachedAttributes[attribute] = attributeUIValue;
                return;
            }
            _cachedAttributes.Add(attribute, attributeUIValue);
        }

        public void PreviewEquip(IEquipment equipment)
        {
            ResetPreview();

            if (!_clonedHero.IsValid() || equipment == null || !equipment.IsValid()) return;
            if (!_clonedHero.TryGetComponent<EquipmentsController>(out var equipmentController))
                return;

            var attributeSystem = _clonedHero.AttributeSystem;

            equipmentController.Equip(equipment, equipment.AllowedSlots[0]);

            PreviewValue();
        }

        public void PreviewUnequip(IEquipment equipment)
        {
            ResetPreview();

            if (!_clonedHero.IsValid() || equipment == null || !equipment.IsValid()) return;
            if (!_clonedHero.TryGetComponent<EquipmentsController>(out var equipmentController))
                return;

            var attributeSystem = _clonedHero.AttributeSystem;

            equipmentController.Unequip(equipment.AllowedSlots[0]);

            PreviewValue();
        }

        public void ResetPreview()
        {
            foreach (var attribute in _cachedAttributes.Values)
            {
                attribute.AttributeUI.ResetAttributeUI();
            }
        }

        private void Clone(HeroBehaviour hero)
        {
            if (_clonedHero != null)
            {
                Destroy(_clonedHero.gameObject);
            }

            _clonedHero = Instantiate(hero, transform);
            var cloneSpec = new PartySlotSpec()
            {
                Hero = _clonedHero.Spec
            };

            foreach (var slotItem in _clonedHero.GetEquipments().Slots)
            {
                cloneSpec.EquippingItems.Slots.Add(new EquipmentSlot()
                {
                    Type = slotItem.Type,
                    Equipment = slotItem.Equipment
                });
            }

            // Remove this so equip/unequip wont affect server or inventory
            RemoveComponent<EquipmentsNetworkController>();
            RemoveComponent<InventoryEquipmentsController>();
            
            _clonedHero.Init(cloneSpec);
        }

        private void RemoveComponent<T>() where T : CharacterComponentBase
        {
            var component = _clonedHero.GetComponent<T>();
            component.enabled = false;
            DestroyImmediate(component);
        }

        private void PreviewValue()
        {
            var attributeSystem = _clonedHero.AttributeSystem;

            foreach (var attributeValue in attributeSystem.AttributeValues)
            {
                if (!_cachedAttributes.TryGetValue(attributeValue.Attribute, out var cachedAttribute))
                    continue;

                cachedAttribute.AttributeUI.CompareValue(cachedAttribute.CurrentValue,
                    attributeValue.CurrentValue);
            }
        }
    }
}
