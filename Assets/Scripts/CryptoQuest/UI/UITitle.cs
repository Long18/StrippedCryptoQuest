using Core.Runtime.Events.ScriptableObjects;
using Core.Runtime.SaveSystem;
using Core.Runtime.SceneManagementSystem.Events.ScriptableObjects;
using Core.Runtime.SceneManagementSystem.ScriptableObjects;
using CryptoQuest.Input;
using CryptoQuest.System.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace CryptoQuest.UI
{
    public class UITitle : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _playerNamePlaceholder;
        [SerializeField] private LocalizeStringEvent _validationText;
        [SerializeField] private GameObject _panelInputName;
        [SerializeField] private TextAsset _textAsset;

        [Space(10), SerializeField] private SceneScriptableObject _sceneToLoad;
        [SerializeField] private InputMediatorSO _inputMediatorSO;
        [SerializeField] private SaveSystemSO _saveSystemSo;

        [Header("Listen on")]
        [SerializeField] private VoidEventChannelSO _sceneLoaded;

        [Header("Raise on")]
        [SerializeField] private LoadSceneEventChannelSO _loadMapEvent;

        private StringValidator _stringValidator;

        private void Awake()
        {
            _stringValidator = new StringValidator(_textAsset);
        }

        private void OnEnable()
        {
            _sceneLoaded.EventRaised += SceneLoadedEvent_Raised;
            _inputMediatorSO.MenuConfirmClicked += CheckPlayerName;
        }

        private void OnDisable()
        {
            _sceneLoaded.EventRaised -= SceneLoadedEvent_Raised;
            _inputMediatorSO.MenuConfirmClicked += CheckPlayerName;
        }

        private void SceneLoadedEvent_Raised()
        {
            _inputMediatorSO.EnableMenuInput();
        }

        private void CheckPlayerName()
        {
            if (IsPlayerNull())
            {
                ShowInputPanel();
                return;
            }

            StartGame();
        }

        private void ShowInputPanel()
        {
            if (!string.IsNullOrEmpty(_playerNamePlaceholder.text)) SetPlayerName();
            if (_panelInputName.activeSelf == true) return;
            _panelInputName.SetActive(true);
        }

        public void SetPlayerName()
        {
            if (!IsValidData()) return;

            _saveSystemSo.PlayerName = _playerNamePlaceholder.text;
            StartGame();
        }

        private bool IsValidData()
        {
            bool isValid = _stringValidator.Validate(_playerNamePlaceholder.text);
            if (!isValid) return false;

            _panelInputName.SetActive(true);
            return true;
        }

        private void StartGame()
        {
            _loadMapEvent.RequestLoad(_sceneToLoad);
        }

        private bool IsPlayerNull()
        {
            return string.IsNullOrEmpty(_saveSystemSo.PlayerName);
        }

        public void ValidateData()
        {
            bool isValid = _stringValidator.Validate(_playerNamePlaceholder.text);
            _validationText.gameObject.SetActive(!isValid);
            _validationText.StringReference.TableEntryReference = "TITLE_VALIDATE_" + _stringValidator.WarningText();
        }
    }
}