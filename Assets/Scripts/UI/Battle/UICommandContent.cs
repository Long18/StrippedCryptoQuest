﻿using CryptoQuest.Gameplay.Battle.Core.Components.BattleUnit;
using CryptoQuest.UI.Battle.CommandsMenu;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace CryptoQuest.UI.Battle
{
    public class UICommandContent : ContentItemMenu
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private Button _button;

        private IObjectPool<UICommandContent> _objectPool;

        public IObjectPool<UICommandContent> ObjectPool
        {
            set => _objectPool = value;
        }

        public override void Init(ButtonInfo info)
        {
            _label.text = info.Name;
            _value.text = info.Value;
            _button.onClick.AddListener(info.Callback.Invoke);
        }

        public void ReleaseToPool()
        {
            _objectPool?.Release(this);
        }
    }
}