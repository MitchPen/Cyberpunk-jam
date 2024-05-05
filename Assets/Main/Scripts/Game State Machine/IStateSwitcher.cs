namespace Main.Scripts.Game_State_Machine
{
    public interface IStateSwitcher
    {
        public void SwitchState<T>() where T:BaseState;
    }
}
