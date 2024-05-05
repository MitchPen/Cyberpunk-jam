using CyberpunkJam.Units.Animation;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace CyberpunkJam.Units.StateMachine
{
    public class SeekState : IUnitState
    {
        private const int FRAME_INTERVAL_FOR_REPATH = 10;
        private const float ROTATE_FORWARD_DURATION = 0.4f;

        private UnitContext context;
        private Tween rotationToTarget;
        private UnitStateMachine stateMachine;
        private NavMeshMovement navMeshMovement;
        private CompositeDisposable disposables;

        public SeekState(UnitContext unitContext, UnitStateMachine unitStateMachine)
        {
            context = unitContext;
            stateMachine = unitStateMachine;

            navMeshMovement = context.navMeshMovement;
        }

        public async UniTask Enter()
        {
            disposables = new();

            Observable.IntervalFrame(FRAME_INTERVAL_FOR_REPATH)
                .Subscribe(_ => ObservTargetAndFollow())
                .AddTo(disposables);

            SetPath();
            RotateForward();

            await UniTask.CompletedTask;
        }

        public async UniTask Exit()
        {
            navMeshMovement.Stop();
            rotationToTarget.Kill();

            disposables.Dispose();

            await UniTask.CompletedTask;
        }

        private void RotateForward()
        {
            rotationToTarget = context.unitPoint.DOLocalRotateQuaternion(Quaternion.identity, ROTATE_FORWARD_DURATION);
        }

        private void ObservTargetAndFollow()
        {
            SetPath();
        }

        private void SetPath()
        {
            var path = navMeshMovement.GetPathToTarget(context.TargetToAttack);

            if (path != null && path.status == NavMeshPathStatus.PathComplete)
            {
                navMeshMovement.SetPath(path);
            }
            else
            {
                stateMachine.Enter<IdleState>();
            }
        }
    }
}