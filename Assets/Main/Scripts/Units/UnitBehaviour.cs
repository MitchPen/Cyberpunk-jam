using CyberpunkJam.Battle;
using CyberpunkJam.Units.Animation;
using CyberpunkJam.Units.StateMachine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace CyberpunkJam.Units
{
    [RequireComponent(typeof(NavMeshMovement))]
    public class UnitBehaviour : MonoBehaviour
    {
        [SerializeField] private UnitContext context;
        [SerializeField] private Transform attackTarget;

        private UnitStateMachine unitStateMachine;
        private CompositeDisposable disposables = new();

        private int skippedFrames = 0;

        public UnitStateMachine UnitStateMachine => unitStateMachine;

        private void Awake()
        {
            SetupStateMachine();
            LaunchObervers();
        }

        private void Start()
        {
            unitStateMachine.Enter<IdleState>();

            context.SetTarget(attackTarget);
        }

        private void SetupStateMachine()
        {
            unitStateMachine = new();
            unitStateMachine.RegistrateState(new IdleState(context, unitStateMachine));
            unitStateMachine.RegistrateState(new LazerAttackState(context, unitStateMachine));
            unitStateMachine.RegistrateState(new SeekState(context, unitStateMachine));
            unitStateMachine.RegistrateState(new DeathState(context));
        }

        private void LaunchObervers()
        {
            Observable.EveryFixedUpdate()
                .Subscribe(_ => AttackObserver())
                .AddTo(disposables);
        }

        private void AttackObserver()
        {
            if (skippedFrames == 2)
            {
                skippedFrames = 0;
                return;
            }

            skippedFrames++;

            if (unitStateMachine.CurrentState is LazerAttackState)
            {
                return;
            }

            var direction = (context.TargetToAttack.position - context.unitPoint.position).normalized;

            if (Physics.Raycast(context.attackPoint.position, direction, out var hitInfo, 10f))
            {
                if (hitInfo.transform.TryGetComponent<TargetToAttack>(out _))
                {
                    unitStateMachine.Enter<LazerAttackState>();

                    return;
                }
            }

            if (unitStateMachine.CurrentState is SeekState == false)
            {
                unitStateMachine.Enter<SeekState>();
            }
        }
    }
}