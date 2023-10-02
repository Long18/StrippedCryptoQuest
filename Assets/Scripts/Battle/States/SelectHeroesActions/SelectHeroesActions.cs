using System.Collections.Generic;
using CryptoQuest.Battle.Commands;
using CryptoQuest.Battle.Components;
using CryptoQuest.Battle.UI.SelectCommand;
using CryptoQuest.Character.Tag;
using CryptoQuest.Gameplay.PlayerParty;
using CryptoQuest.System;
using UnityEngine;

namespace CryptoQuest.Battle.States.SelectHeroesActions
{
    public abstract class StateBase
    {
        public StateBase(HeroBehaviour hero, SelectHeroesActions fsm)
        {
            Hero = hero;
            Fsm = fsm;
        }

        protected SelectHeroesActions Fsm { get; private set; }

        /// <summary>
        /// The hero that is currently in this state
        ///
        /// we could have 2 <see cref="SelectCommand"/> in the stack but the hero is different
        /// </summary>
        protected HeroBehaviour Hero { get; private set; }

        public abstract void OnEnter();
        public abstract void OnExit();
    }

    public class StateFactory
    {
        public static StateBase CreateState<T>() where T : StateBase, new()
        {
            return new T();
        }
    }

    /// <summary>
    /// Select the command of all the heroes in party
    ///
    /// only exit this state after selected commands for all heroes
    ///
    /// This class has a Push down state machine for each member/hero in party
    /// </summary>
    public class SelectHeroesActions : IState
    {
        public struct HeroCommand
        {
            public HeroBehaviour HeroBehaviour;
            public ICommand Command;
        }

        private UISelectCommand _selectCommandUI;
        public UISelectCommand SelectCommandUI => _selectCommandUI;
        private BattleStateMachine _battleStateMachine;
        public BattleStateMachine BattleStateMachine => _battleStateMachine;
        private IPartyController _party;
        private EnemyPartyManager _enemyPartyManager;
        public EnemyPartyManager EnemyPartyManager => _enemyPartyManager;
        private int _currentHeroIndex = 0;
        public StateFactory StateFactory { get; private set; } = new();

        private readonly Stack<StateBase> _stateStack = new Stack<StateBase>();
        private BattlePresenter _presenter;

        public void OnEnter(BattleStateMachine battleStateMachine)
        {
            _battleStateMachine = battleStateMachine;
            _enemyPartyManager = battleStateMachine.GetComponent<EnemyPartyManager>();
            _selectCommandUI = battleStateMachine.CommandUI;
            _party = ServiceProvider.GetService<IPartyController>();
            _presenter = battleStateMachine.GetComponent<BattlePresenter>();
            _presenter.CommandPanel.SetActive(true);

            _currentHeroIndex = 0;

            battleStateMachine.BattleUI.SetActive(true);
            PushState(new SelectCommand(_party.Slots[_currentHeroIndex].HeroBehaviour, this));
        }

        public void OnExit(BattleStateMachine battleStateMachine)
        {
            // battleStateMachine.BattleUI.SetActive(false);
            while (_stateStack.Count > 0)
            {
                _stateStack.Pop()?.OnExit();
            }
        }

        public void PushState(StateBase state)
        {
            if (_stateStack.Count > 0) _stateStack.Peek()?.OnExit();
            _stateStack.Push(state);
            state.OnEnter();
        }

        public void PopState()
        {
            _stateStack.Peek()?.OnExit();
            _stateStack.Pop();
            if (_stateStack.Count > 0)
                _stateStack.Peek()?.OnEnter();
        }

        public void PopToLastSelectCommandState()
        {
            var currentState = _stateStack.Pop();
            do
            {
                currentState.OnExit();
                currentState = _stateStack.Pop();
            } while (currentState is not SelectCommand && _stateStack.Count > 0);

            currentState.OnEnter();
        }

        public bool TryGetComponent<T>(out T component) where T : Component
            => _battleStateMachine.TryGetComponent(out component);

        public bool GetNextAliveHero(out HeroBehaviour hero)
        {
            hero = null;
            if (++_currentHeroIndex >= _party.Size) return false;
            bool canHeroFunctions;
            do
            {
                hero = _party.Slots[_currentHeroIndex].HeroBehaviour;
                canHeroFunctions = hero != null && hero.IsValid() && !hero.HasTag(TagsDef.Dead);
            } while (canHeroFunctions == false);

            return true;
        }

        public void GoToPresentState()
        {
            _battleStateMachine.ChangeState(new Present());
        }

        /// <summary>
        /// Try to get next alive hero and push <see cref="SelectCommand"/> state, otherwise go to present state
        /// </summary>
        public void GoToNextState()
        {
            if (GetNextAliveHero(out var hero))
                PushState(new SelectCommand(hero, this));
            else
            {
                Debug.Log("Battle::SelectHeroesActions cannot get next alive hero" +
                          "\nThis could be because we at the end of the party list or all heroes are dead");
                GoToPresentState();
            }
        }
    }
}