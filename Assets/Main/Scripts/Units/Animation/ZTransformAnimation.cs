using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CyberpunkJam.Units.Animation
{
    public class ZTransformAnimation
    {
        private readonly Transform container;

        private Tween rotationTween = null;

        private float rotationAngle = 0f;

        public ZTransformAnimation(Transform container)
        {
            this.container = container;
        }

        public async UniTask RotateZAsync(float angle, float duration)
        {
            if (rotationTween.IsActive())
            {
                rotationTween.Kill();
            }

            rotationTween = DOVirtual.Float(rotationAngle, angle, duration, OnRotationAngleChanged)
                .SetLink(container.gameObject);

            await rotationTween.AsyncWaitForCompletion();
        }

        public async void RotateZ(float angle, float duration)
        {
            await RotateZAsync(angle, duration);
        }

        public void Execute()
        {
            RotateAround();
        }

        private void RotateAround()
        {
            var localRotation = container.transform.localRotation;
            container.transform.localRotation = Quaternion.Euler(localRotation.x, localRotation.y, localRotation.z + rotationAngle);
        }

        private void OnRotationAngleChanged(float value)
        {
            rotationAngle = value;
        }
    }
}