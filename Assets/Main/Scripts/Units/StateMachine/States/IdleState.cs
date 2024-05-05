using CyberpunkJam.Units.StateMachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

namespace CyberpunkJam.Units.Animation
{
    [Serializable]
    public class IdleState : IUnitState
    {
        private const float VERTICAL_MOVEMENT_HEIGHT = 0.35f;
        private const float VERTICAL_MOVEMENT_DURATION = 2f;
        private const float VERTICAL_ROTATION_DURATION = 1f;

        private UnitContext context;
        private UnitStateMachine stateMachine;

        public IdleState(UnitContext animationContext, UnitStateMachine unitStateMachine)
        {
            context = animationContext;
            stateMachine = unitStateMachine;
        }

        public async UniTask Enter()
        {
            PlayRotationAroundLoop();
            PlayVerticalLoopMovement();

            await UniTask.Delay(TimeSpan.FromSeconds(5f));

            stateMachine.Enter<SeekState>();
        }

        public async UniTask Exit()
        {
            await UniTask.CompletedTask;
        }

        private void PlayVerticalLoopMovement()
        {
            if (context.VerticalMovementTween.IsActive())
            {
                return;
            }

            var container = context.unitPoint;
            var startPosition = container.localPosition.y;
            var stepDuration = VERTICAL_MOVEMENT_DURATION / 4f;

            //context.VerticalMovementTween = DOTween.Sequence(container)
            //    .Append(VerticalMovement(container, startPosition + VERTICAL_MOVEMENT_HEIGHT, stepDuration))
            //    .Append(VerticalMovement(container, startPosition, stepDuration))
            //    .Append(VerticalMovement(container, startPosition - VERTICAL_MOVEMENT_HEIGHT, stepDuration))
            //    .Append(VerticalMovement(container, startPosition, stepDuration))
            //    .SetEase(Ease.Linear)
            //    .SetLoops(-1);
        }

        private void PlayRotationAroundLoop()
        {
            if (context.VerticalRotationTween.IsActive())
            {
                return;
            }

            var container = context.unitPoint;

            //context.VerticalRotationTween = container.DOLocalRotate(new(0f, 360f, 0f), VERTICAL_ROTATION_DURATION, RotateMode.FastBeyond360)
            //    .SetEase(Ease.Linear)
            //    .SetLoops(-1);
        }

        private Tween VerticalMovement(Transform container, float value, float duration)
        {
            return container.DOLocalMoveY(value, duration)
                .SetEase(Ease.Linear);
        }
    }
}