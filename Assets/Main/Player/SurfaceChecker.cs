using System;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UniRx.Triggers;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Main.Player
{
    public class SurfaceChecker : MonoBehaviour
    {
        public bool IsGrounded { get;private set; }
        public Vector3 CurrentNormal { get; private set; }

        public event Action AboveGroundEvent;
        public event Action LandedGroundEvent;

        [SerializeField] private Transform _topPoint;
        [SerializeField] private Transform _bottomPoint;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _groundMask;
        private IDisposable _raycastChecker;

        private void Start()
        {
            AboveGroundEvent?.Invoke();
            _raycastChecker = this.UpdateAsObservable().Subscribe(x =>
            {
                var isGrounded = Physics.Raycast(_bottomPoint.position, Vector3.down, out RaycastHit hit, _distance, _groundMask);
                CurrentNormal = hit.normal;
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

        public bool ValidateSpaceAbove()
        {
            return Physics.Raycast(_topPoint.position, Vector3.up, _distance)==false;
        }

        private void OnDisable()
        {
            _raycastChecker?.Dispose();
        }

#if UNITY_EDITOR
        [SerializeField] private List<Collider> EDITOR_physicsColliders;

        [Button]
        private void SetTopBotPoints()
        {
            if (EDITOR_physicsColliders != null && EDITOR_physicsColliders.Count != 0)
            {
                var minY = EDITOR_physicsColliders.Min(x => x.bounds.min.y) + 0.2f;
                var maxY = EDITOR_physicsColliders.Max(x => x.bounds.max.y) - 0.2f;
                
                _topPoint.position = new Vector3(transform.position.x, maxY, transform.position.z);
                _bottomPoint.position = new Vector3(transform.position.x, minY, transform.position.z);
            }
        }

        private void OnDrawGizmos()
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(_bottomPoint.position, _bottomPoint.position + (-_bottomPoint.up.normalized * _distance), 1f);
            Handles.DrawLine(_topPoint.position, _topPoint.position + (_topPoint.up.normalized * _distance), 1f);
        }
        
#endif
    }
}
