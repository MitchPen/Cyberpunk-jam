using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace CyberpunkJam.Units.StateMachine
{
    public class UnitStateMachine
    {
        private Dictionary<Type, IUnitState> registeredStates = new();

        public IUnitState CurrentState { get; private set; }

        public async void Enter<T>() where T : class, IUnitState
        {
            var newState = await ChangeState<T>();

            await newState.Enter();
        }

        public void RegistrateState(IUnitState state)
        {
            registeredStates[state.GetType()] = state;
        }

        private async UniTask<T> ChangeState<T>() where T : class, IUnitState
        {
            if (CurrentState is not null)
            {
                await CurrentState.Exit();

                CurrentState = null;
            }

            CurrentState = GetState<T>();

            return CurrentState as T;
        }

        private T GetState<T>() where T : class, IUnitState
        {
            return registeredStates[typeof(T)] as T;
        }
    }
}