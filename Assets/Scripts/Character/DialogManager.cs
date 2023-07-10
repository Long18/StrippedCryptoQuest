﻿using CryptoQuest.Events.UI;
using CryptoQuest.Gameplay.Quest.Dialogue.ScriptableObject;
using CryptoQuest.Input;
using CryptoQuest.UI;
using CryptoQuest.UI.Dialogs;
using UnityEngine;

namespace CryptoQuest.Character
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField] private InputMediatorSO _inputMediator;
        [SerializeField] private DialogEventChannelSO _dialogEventSO;

        private IDialog _speechDialog = NullDialog.Instance;

        private void Awake()
        {
            _speechDialog = GetComponentInChildren<IDialog>();
            // UIChatDialog.Create<UIChatDialog>();
        }

        private void OnEnable()
        {
            _dialogEventSO.ShowEvent += ShowDialog;
            _dialogEventSO.HideEvent += HideDialog;
        }

        private void OnDisable()
        {
            _dialogEventSO.ShowEvent -= ShowDialog;
            _dialogEventSO.HideEvent -= HideDialog;
        }

        private void ShowDialog(DialogueScriptableObject dialogue)
        {
            _inputMediator.EnableDialogueInput();
            _speechDialog.SetData(new SpeechDialogArgs()
            {
                DialogueSO = dialogue
            });
            _speechDialog.Show();
        }

        private void HideDialog()
        {
            _inputMediator.EnableMapGameplayInput();
        }
    }
}