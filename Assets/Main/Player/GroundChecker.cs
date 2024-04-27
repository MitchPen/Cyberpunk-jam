using System;
using UniRx.Triggers;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Main.Player
{
    public class GroundChecker : MonoBehaviour
    {
        public bool IsGrounded { get;private set; }
        public Vector3 CurrentNormal { get; private set; }

        public event Action AboveGroundEvent;
        public event Action LandedGroundEvent;
        
        [SerializeField] private Transform _bottomPoint;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _groundMask;
        private IDisposable _raycastChecker;
        private IDisposable _groundedChecker;

        private void Start()
        {
            _raycastChecker = this.UpdateAsObservable().Subscribe(x =>
            {
                var isGrounded = Physics.Raycast(_bottomPoint.position, Vector3.down, _distance, _groundMask);
                if (IsGrounded!=isGrounded)
                {
                    IsGrounded = isGrounded;
                    if (IsGrounded)
                    {
                        LandedGroundEvent?.Invoke();
                    }
                    else
                    {
                        AboveGroundEvent?.Invoke();
                    }
                }
                // Debug.LogWarning(IsGrounded);

            });
        }

        private void OnDisable()
        {
            _raycastChecker?.Dispose();
        }

#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(_bottomPoint.position, _bottomPoint.position + (Vector3.down * _distance), 1f);
        }
        
#endif
    }
}
