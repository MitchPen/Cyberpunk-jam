using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace CyberpunkJam.Units.StateMachine
{
    [HideLabel]
    [Serializable]
    public class UnitContext
    {
        [Title("References")]
        public Transform unitPoint;
        public Transform attackPoint;
        public LineRenderer lazer;
        public NavMeshMovement navMeshMovement;

        public Transform TargetToAttack { get; private set; }
        public Tween VerticalMovementTween { get; set; }
        public Tween VerticalRotationTween { get; set; }

        public void SetTarget(Transform target)
        {
            TargetToAttack = target;
        }
    }
}