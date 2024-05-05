using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CyberpunkJam.Units.StateMachine
{
    public class DeathState : IUnitState
    {
        private UnitContext context;

        public DeathState(UnitContext unitContext)
        {
            context = unitContext;
        }

        public async UniTask Enter()
        {
            context.VerticalMovementTween.Kill();
            context.VerticalRotationTween.Kill();

            context.SetTarget(null);

            await UniTask.CompletedTask;
        }

        public async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }
    }
}