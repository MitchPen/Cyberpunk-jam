using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace CyberpunkJam.Units.StateMachine
{
    public class LookAtTargetSubState : IUnitState
    {
        private const float ROTATION_SPEED = 250.0f;

        protected UnitContext context;
        protected float angleBetweenTarget;

        private CompositeDisposable disposables;

        public LookAtTargetSubState(UnitContext unitContext)
        {
            context = unitContext;
        }

        public virtual async UniTask Enter()
        {
            disposables = new();

            Observable.EveryUpdate().Subscribe(_ => StartFollowObject())
                .AddTo(disposables);

            await UniTask.CompletedTask;
        }

        public virtual async UniTask Exit()
        {
            StopLookAt();

            await UniTask.CompletedTask;
        }

        protected void StopLookAt()
        {
            if (disposables.IsDisposed)
            {
                return;
            }

            disposables.Dispose();
        }

        private void StartFollowObject()
        {
            var container = context.unitPoint;
            var direction = context.TargetToAttack.position - container.position;
            var quaternionDirection = Quaternion.LookRotation(direction);

            container.rotation = Quaternion.RotateTowards(container.rotation, quaternionDirection, Time.deltaTime * ROTATION_SPEED);

            angleBetweenTarget = Quaternion.Angle(container.rotation, quaternionDirection);
        }
    }
}