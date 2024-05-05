using Cysharp.Threading.Tasks;

namespace CyberpunkJam.Units.StateMachine
{
    public interface IUnitState
    {
        UniTask Enter();
        UniTask Exit();
    }
}
