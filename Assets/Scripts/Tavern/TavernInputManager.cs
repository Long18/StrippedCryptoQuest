﻿using CryptoQuest.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CryptoQuest.Tavern
{
    public class TavernInputManager : MonoBehaviour, InputActions.ITavernActions
    {
        [SerializeField] private InputMediatorSO _inputMediatorSO;

        public event UnityAction CancelEvent;
        public event UnityAction SubmitEvent;
        public event UnityAction<Vector2> NavigateEvent;
        public event UnityAction ResetEvent;
        public event UnityAction ExecuteEvent;
        public event UnityAction InteractEvent;


        private void OnEnable()
        {
            _inputMediatorSO.InputActions.Tavern.SetCallbacks(this);
        }

        private void OnDisable()
        {
            _inputMediatorSO.InputActions.Tavern.RemoveCallbacks(this);
        }

        public void EnableInput()
        {
            _inputMediatorSO.DisableAllInput();
            _inputMediatorSO.InputActions.Tavern.Enable();
        }

        public void DisableInput()
        {
            _inputMediatorSO.InputActions.Tavern.Disable();
            _inputMediatorSO.EnableMapGameplayInput();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed) CancelEvent?.Invoke();
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            if (context.performed) NavigateEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.performed) SubmitEvent?.Invoke();
        }

        public void OnReset(InputAction.CallbackContext context)
        {
            if (context.performed) ResetEvent?.Invoke();
        }

        public void OnExecute(InputAction.CallbackContext context)
        {
            if (context.performed) ExecuteEvent?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed) InteractEvent?.Invoke();
        }
    }
}