﻿using CryptoQuest.Gameplay;
using CryptoQuest.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CryptoQuest.UI.Dialogs.RewardDialog
{
    public class UIRewardDialog : AbstractDialog
    {
        [Header("Child Components"), SerializeField]
        private InputMediatorSO _inputMediator;

        [SerializeField] private GameStateSO _gameStateSo;

        [field: Header("Transform"), SerializeField]
        public Transform TopContainer { get; private set; }

        [field: SerializeField] public Transform BottomContainer { get; private set; }

        [Header("UI"), SerializeField] private GameObject _topNone;

        [SerializeField] private GameObject _bottomNone;

        [field: Header("Components"), SerializeField]
        private Button _defaultSelectButton;

        [field: SerializeField] public UIRewardItem RewardItemPrefab { get; private set; }
        [field: SerializeField] private RewardScroll _scroll;
        [field: SerializeField] private InputAction _inputAction;

        private RewardDialogData _rewardDialogData;

        private void OnEnable()
        {
            _inputAction.Enable();
            _inputAction.performed += OnCloseImmediately;

            _inputMediator.MenuConfirmedEvent += OnCloseButtonPressed;
        }

        private void OnDisable()
        {
            _inputAction.Disable();
            _inputAction.performed -= OnCloseImmediately;

            _inputMediator.MenuConfirmedEvent -= OnCloseButtonPressed;
        }

        public void OnCloseButtonPressed() => Hide();

        private void OnCloseImmediately(InputAction.CallbackContext action)
        {
            if (!action.performed) return;
            Hide();
        }

        public override void Show()
        {
            base.Show();

            _defaultSelectButton.Select();

            if (!_rewardDialogData.IsValid()) return;
            _inputMediator.DisableAllInput();

            DisplayItemsReward();
        }

        public override void Hide()
        {
            base.Hide();

            if (_gameStateSo.CurrentGameState != EGameState.Field) return;
            _inputMediator.EnableMapGameplayInput();
        }

        private void DisplayItemsReward()
        {
            _rewardDialogData.RewardsInfos.ForEach(reward => reward.CreateUI(this));

            _topNone.SetActive(!(TopContainer.childCount > 0));
            _bottomNone.SetActive(!(BottomContainer.childCount > 0));

            _scroll.UpdateStep();
        }

        public UIRewardDialog SetReward(RewardDialogData rewardDialogData)
        {
            _rewardDialogData = rewardDialogData;
            return this;
        }

        public UIRewardItem InstantiateReward(Transform parent) => Instantiate(RewardItemPrefab, parent);
    }
}