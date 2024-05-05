using System;
using Main.Scripts.Level;
using Main.Scripts.UI;

namespace Main.Scripts.Game_State_Machine
{
    public abstract class BaseState
    {
        protected StateContext _stateContext;
        protected BaseState(StateContext context) => _stateContext = context;
        public virtual void Enter() { }
        
        public virtual void Exit() { }
    }

    [Serializable]
    public class StateContext
    {
        public IStateSwitcher StateSwitcher;
        public LevelLauncher LevelLauncher;
        public UiSystem UiSystem;
    }
}
