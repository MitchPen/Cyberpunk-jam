using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

namespace CyberpunkJam.Units.StateMachine
{
    public class LazerAttackState : LookAtTargetSubState
    {
        private const float LAZER_LENGTH = 10f;
        private const float LAZER_LIFE_DURATION = 0.6f;
        private const float LAZER_PREPARE_DURATION = 2f;
        private const float LAZER_AFTER_PREPARE_DELAY = 0.5f;

        private const float LAZER_START_WIDTH = 0.05f;
        private const float LAZER_FINAL_WIDTH = 0.45f;
        private const float LAZER_CHARGE_END_WIDTH = 0.2f;

        private UnitStateMachine stateMachine;
        private CancellationTokenSource prepareCancellationTokenSource;

        public LazerAttackState(UnitContext unitContext, UnitStateMachine stateMachine) : base(unitContext)
        {
            this.stateMachine = stateMachine;
        }

        public override async UniTask Enter()
        {
            prepareCancellationTokenSource = new CancellationTokenSource();

            await base.Enter();

            try
            {
                context.VerticalMovementTween.Kill();
                context.VerticalRotationTween.Kill();

                await UniTask.WaitUntil(() => angleBetweenTarget < 40f, cancellationToken: prepareCancellationTokenSource.Token);

                await PrepareLazerAttack(prepareCancellationTokenSource.Token);

                await LazerAttack(prepareCancellationTokenSource.Token);

                stateMachine.Enter<SeekState>();
            }
            catch { }
        }

        public override async UniTask Exit()
        {
            if (prepareCancellationTokenSource != null)
            {
                prepareCancellationTokenSource.Cancel();
            }

            await base.Exit();

            context.lazer.enabled = false;
        }

        private async UniTask PrepareLazerAttack(CancellationToken token)
        {
            var lazer = context.lazer;
            var container = context.attackPoint;
            var target = context.TargetToAttack;
            var time = 0f;

            lazer.enabled = true;

            while (time < LAZER_PREPARE_DURATION)
            {
                if (context.TargetToAttack == null)
                {
                    break;
                }

                var width = LAZER_START_WIDTH + LAZER_CHARGE_END_WIDTH * time / LAZER_PREPARE_DURATION;

                lazer.startWidth = width;
                lazer.endWidth = width;

                lazer.SetPosition(0, container.position);

                var direction = container.forward;

                var distanceToTarget = Vector3.Distance(container.position, target.position);
                distanceToTarget = distanceToTarget > LAZER_LENGTH ? LAZER_LENGTH : distanceToTarget;
                lazer.SetPosition(1, container.position + (direction * distanceToTarget));

                await UniTask.Yield(cancellationToken: token);

                time += Time.fixedDeltaTime;
            }

            StopLookAt();

            await UniTask.Delay(TimeSpan.FromSeconds(LAZER_AFTER_PREPARE_DELAY), cancellationToken: prepareCancellationTokenSource.Token);
        }

        private async UniTask LazerAttack(CancellationToken token)
        {
            var lazer = context.lazer;

            lazer.startWidth = LAZER_FINAL_WIDTH;
            lazer.endWidth = LAZER_FINAL_WIDTH;

            await UniTask.Delay(TimeSpan.FromSeconds(LAZER_LIFE_DURATION), cancellationToken: token);

            lazer.enabled = false;
        }
    }
}