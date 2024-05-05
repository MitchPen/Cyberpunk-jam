using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CyberpunkJam.Units.Animation
{
    public class YTransformAnimation
    {
        private const float DEGRESS_360 = 360f;

        private readonly Transform rotatedAroundContainer;
        private readonly Transform verticalMovementContainer;

        private AnimationCurve verticalMovementCurve = null;

        private float time = 0f;
        private float range = 0f;
        private float acceleration = 1f;
        private float verticalMovementDuration = 0f;
        private float fullCircleRotationDuration = 0f;
        private bool verticalMovementSetupped = false;

        public YTransformAnimation(Transform rotatedAroundContainer, Transform verticalMovementContainer)
        {
            this.rotatedAroundContainer = rotatedAroundContainer;
            this.verticalMovementContainer = verticalMovementContainer;
        }

        public async UniTask ChangeRotationAccelerationAsync(float value, float duration)
        {
            await DOVirtual.Float(acceleration, value, duration, OnAccelerationChanged)
                .SetLink(rotatedAroundContainer.gameObject)
                .AsyncWaitForCompletion();
        }

        public async void ChangeRotationAcceleration(float value, float duration = 0f)
        {
            await ChangeRotationAccelerationAsync(value, duration);
        }

        public YTransformAnimation SetVerticalMovementSettings(float height, float duration, AnimationCurve curve)
        {
            range = height * 2f;
            verticalMovementCurve = curve;
            verticalMovementDuration = duration;

            verticalMovementSetupped = true;

            return this;
        }

        public YTransformAnimation SetFullCircleRotationDuration(float duration)
        {
            fullCircleRotationDuration = duration;

            return this;
        }

        public void Execute()
        {
            RotateAround();
            VerticalMovement();
        }

        private void RotateAround()
        {
            if (acceleration == 0f || fullCircleRotationDuration == 0f)
            {
                return;
            }

            var speed = DEGRESS_360 / fullCircleRotationDuration;
            speed *= Time.deltaTime;
            speed *= acceleration;

            rotatedAroundContainer.Rotate(speed * Vector3.up);
        }

        private void VerticalMovement()
        {
            if (verticalMovementSetupped == false)
            {
                return;
            }

            var curveTime = time / verticalMovementDuration;
            var heightValue = verticalMovementCurve.Evaluate(curveTime);
            var height = (range * heightValue) - heightValue;
            var verticalPosition = new Vector3(0f, height, 0f);

            verticalMovementContainer.transform.localPosition = verticalPosition;

            time += Time.deltaTime;
            time = time > verticalMovementDuration ? 0f : time;
        }

        private void OnAccelerationChanged(float value)
        {
            acceleration = value;
        }
    }
}