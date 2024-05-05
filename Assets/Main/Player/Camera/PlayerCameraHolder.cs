using System;
using UniRx.Triggers;
using UniRx;
using UnityEngine;

namespace Main.Player.Camera
{
    public class PlayerCameraHolder : MonoBehaviour
    {
        [SerializeField] private CameraComponent _cameraComponent;
        private IDisposable _orientationDisposable;

        private void Start()
        {
            _orientationDisposable = transform.UpdateAsObservable().Subscribe(x =>
            {
                transform.forward = _cameraComponent.CurrentOrientation.forward;
            });
        }

        public void GetCorrectedVectors(out Vector3 projectedForward, out Vector3 right)
        {
            projectedForward = Vector3.ProjectOnPlane(_cameraComponent.CurrentOrientation.forward, Vector3.up);
            right = _cameraComponent.CurrentOrientation.right;
        }

        private void OnDisable()
        {
            _orientationDisposable?.Dispose();
        }
        
        #if UNITY_EDITOR

        private void OnValidate()
        {
            _cameraComponent = FindObjectOfType<CameraComponent>();
        }

#endif
    }
}
