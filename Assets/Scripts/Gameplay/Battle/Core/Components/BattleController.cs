using CryptoQuest.Gameplay.Battle.Core.ScriptableObjects;
using CryptoQuest.Input;
using CryptoQuest.UI.Battle;
using IndiGames.Core.Events.ScriptableObjects;
using UnityEngine;

namespace CryptoQuest.Gameplay.Battle.Core.Components
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private BattleBus _battleBus;
        [SerializeField] private InputMediatorSO _inputMediator;

        [Header("UI")]
        [SerializeField] private CharacterList _heroesUI;
        [SerializeField] private CharacterList _monstersUI;
        [SerializeField] private GameObject _batteMenu;

        [Header("Listen Events")]
        [SerializeField] private VoidEventChannelSO _newTurnEventChannel;
        [SerializeField] private VoidEventChannelSO _battleStartEventChannel;
        [SerializeField] private VoidEventChannelSO _endStrategyPhaseEventChannel;
        
        private void OnEnable()
        {
            _newTurnEventChannel.EventRaised += OnNewTurn;
            _battleStartEventChannel.EventRaised += SetupBattleUIs;
            _endStrategyPhaseEventChannel.EventRaised += OnStategyPhaseEnd;
        }

        private void OnDisable()
        {
            _newTurnEventChannel.EventRaised -= OnNewTurn;
            _battleStartEventChannel.EventRaised -= SetupBattleUIs;
            _endStrategyPhaseEventChannel.EventRaised -= OnStategyPhaseEnd;
            _inputMediator.EnableMapGameplayInput();
        }

        private void OnNewTurn()
        {
            _batteMenu.SetActive(true);
            _heroesUI.gameObject.SetActive(true);
            _inputMediator.EnableMenuInput();
        }

        private void SetupBattleUIs()
        {
            _heroesUI.InitUI(_battleBus.BattleManager.BattleTeam1.BattleUnits);
            _monstersUI.InitUI(_battleBus.BattleManager.BattleTeam2.BattleUnits);
            _heroesUI.gameObject.SetActive(false);
            _batteMenu.SetActive(false);
        }

        private void OnStategyPhaseEnd()
        {
            _batteMenu.SetActive(false);
        }
    }   
}