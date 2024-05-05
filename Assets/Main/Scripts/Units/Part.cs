using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CyberpunkJam.Units
{
    public class Part : MonoBehaviour
    {
        private Transform targetTransform;

        private Vector3 followOffset;
        private float followSpeed;

        private void Update()
        {
            FollowTarget();
        }

        public void SetTarget(Transform targetTransform)
        {
            this.targetTransform = targetTransform;
        }

        public void SetFollowTargetSpeed(float speed)
        {
            followSpeed = speed;
        }

        public void SetFollowTargetOffset(Vector3 offset)
        {
            followOffset = offset;
        }

        public async UniTask ShowAsync(float duration)
        {
            await transform.DOScale(Vector3.one, duration).AsyncWaitForCompletion();
        }

        public async void Show(float duration)
        {
            await ShowAsync(duration);
        }

        public async UniTask HideAsync(float duration)
        {
            await transform.DOScale(Vector3.zero, duration).AsyncWaitForCompletion();
        }

        public async void Hide(float duration)
        {
            await HideAsync(duration);
        }

        public async UniTask LocalMoveAsync(Vector3 position, float duration)
        {
            await transform.DOLocalMove(position, duration).AsyncWaitForCompletion();
        }

        public async void LocalMove(Vector3 position, float duration)
        {
            await LocalMoveAsync(position, duration);
        }

        private void FollowTarget()
        {
            if (targetTransform == null)
            {
                return;
            }

            var forward = targetTransform.position - transform.position + followOffset;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward), followSpeed * Time.deltaTime);
        }
    }
}