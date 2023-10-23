﻿using System.Collections;
using CryptoQuest.Gameplay.Battle.Core.ScriptableObjects.Data;
using CryptoQuest.Item;
using CryptoQuest.Menu;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Components;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace CryptoQuest.UI.Menu.Panels.Item
{
    public class UIConsumableItem : MonoBehaviour
    {
        public delegate void UIConsumableItemEvent(UIConsumableItem item);

        public static event UIConsumableItemEvent Inspecting;
        public static event UIConsumableItemEvent Using;
        [SerializeField] private Image _icon;
        [SerializeField] private LocalizeStringEvent _name;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _charaterX;
        [SerializeField] private Text _quantity;
        [SerializeField] private MultiInputButton _button;
        [SerializeField] private GameObject _selectedBackground;
        [SerializeField] private Color _disabledColor;
        [SerializeField] private Color _enabledColor;
        private ConsumableInfo _consumable;
        public ConsumableInfo Consumable => _consumable;
        private bool _canClick;

        public bool Interactable
        {
            get => _button.interactable;
            set => _button.interactable = value;
        }

        private void OnEnable()
        {
            _button.Selected += OnInspectingItem;
            _button.DeSelected += OnDeselectItem;
            _button.onClick.AddListener(OnUse);
        }


        private void OnDisable()
        {
            _button.Selected -= OnInspectingItem;
            _button.DeSelected -= OnDeselectItem;
            _button.onClick.RemoveListener(OnUse);
        }

        public void OnUse()
        {
            if (!_canClick) return;
            Using?.Invoke(this);
            _consumable.Consuming(); // this will show a correct UI
        }

        private void OnInspectingItem()
        {
            _selectedBackground.SetActive(true);
            Inspecting?.Invoke(this);
        }

        private void OnDeselectItem()
        {
            _selectedBackground.SetActive(false);
        }

        public void Init(UIConsumables consumables, ConsumableInfo item)
        {
            _consumable = item;
            StartCoroutine(consumables.GetConsumableAvatar(item.DataId, item.Icon, avatar => _icon.sprite = avatar));
            _name.StringReference = item.DisplayName;
            SetQuantityText(item);

            var allowedInField = (_consumable.Data.UsageScenario & EAbilityUsageScenario.Field) > 0;
            SetColorText(allowedInField);
        }

        public void SetQuantityText(ConsumableInfo item)
        {
            _quantity.text = item.Quantity.ToString();
        }

        private void SetColorText(bool allowed = false)
        {
            var color = allowed ? _enabledColor : _disabledColor;

            _nameText.color = color;
            _charaterX.color = color;
            _quantity.color = color;

            _canClick = allowed;
        }


        public void Inspect()
        {
            StartCoroutine(CoSelectButton());
            OnInspectingItem();
        }

        IEnumerator CoSelectButton()
        {
            yield return null;
            _button.Select();
        }    
    }
}