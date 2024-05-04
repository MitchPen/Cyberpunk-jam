using System;
using System.Collections.Generic;
using Main.Scripts.Level;
using Main.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Main.Scripts.Game_State_Machine
{
    public class StateMachineController : MonoBehaviour, IStateSwitcher
    {
        [Inject] private UiSystem _uiSystem;
        [Inject] private LevelLauncher _levelLauncher;
        private Dictionary<Type, BaseState> _states;
        private BaseState _currentState;
        private StateContext _context;

        private void Awake()
        {
            BuildStateContext();
            LoadStates();
           // SwitchState<InitialState>();
        }

        private void LoadStates()
        {
            _states = new Dictionary<Type, BaseState>()
            {
                // {typeof(InitialState), new InitialState(_context)},
                // {typeof(GameplayState), new GameplayState(_context)},
                // {typeof(LoseState), new LoseState(_context)},
                // {typeof(FinishState), new FinishState(_context)},
            };
        }

        private void BuildStateContext()
        {
            _context = new StateContext();
            _context.StateSwitcher = this;
            _context.LevelLauncher = _levelLauncher;
            _context.UiSystem = _uiSystem;
        }

        public void SwitchState<T>() where T: BaseState
        {
            Debug.Log(typeof(T));
            if (!_states.ContainsKey(typeof(T))) return;

            var nextState = _states[typeof(T)];

            if (_currentState != nextState)
            {
                if(_currentState!=null)
                    _currentState.Exit();

                _currentState = nextState;
                _currentState.Enter();
            }
        }
    }
}
