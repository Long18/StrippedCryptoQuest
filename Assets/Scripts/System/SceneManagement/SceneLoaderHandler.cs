using System.Collections;
using CryptoQuest.Events;
using CryptoQuest.UI.SpiralFX;
using IndiGames.Core.Events.ScriptableObjects;
using IndiGames.Core.SceneManagementSystem;
using IndiGames.Core.SceneManagementSystem.Events.ScriptableObjects;
using IndiGames.Core.SceneManagementSystem.ScriptableObjects;
using IndiGames.Core.UI.FadeController;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace CryptoQuest.System.SceneManagement
{
    public class SceneLoaderHandler : LinearGameSceneLoader
    {
        [SerializeField] private SceneLoaderBus _sceneLoadBus;
        [SerializeField] private ConfigSOEventChannelSO _onSetConfigEventChannel;
        public SceneScriptableObject CurrentLoadedScene => _currentLoadedScene;

        private void Awake()
        {
            _sceneLoadBus.SceneLoader = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _onSetConfigEventChannel.EventRaised += SetFadeConfig;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _onSetConfigEventChannel.EventRaised -= SetFadeConfig;
        }

        private void SetFadeConfig(FadeConfigSO configSo)
        {
            _currentConfigUsed = configSo;
        }
    }
}