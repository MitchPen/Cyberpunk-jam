using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Main.Player
{
    public class SurfaceChecker : MonoBehaviour
    {
        public bool IsTouchingGround { get;private set; }

        public event Action AboveGroundEvent;

        [SerializeField] private Transform _bottomPoint;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _slopeAngleLimit;
        private IDisposable _raycastChecker;
        private Vector3 _currentNormal = Vector3.up;
        private CancellationTokenSource _cts;

        private void Start()
        {
            InitGroundChecker();
        }

        private void InitGroundChecker()
        {
            _raycastChecker = this.FixedUpdateAsObservable().Subscribe(x =>
            {
                // Ray rayBottom = new Ray(_bottomPoint.position, ValidateSlope()?-_currentNormal: Vector3.down);

                bool isGrounded = false;
                for (int i = 0; i < 5; i++)
                {
                    Vector3 direction = (Quaternion.Euler(0f, 90f*i, 0f))* Vector3.forward;
                    Ray ray = new Ray(_bottomPoint.position,  i==4? Vector3.down: direction);
                    
                    if (Physics.Raycast(ray, out  RaycastHit hit, _distance, _groundMask))
                    {
                        isGrounded = true;
                        _currentNormal = hit.normal;
                        break;
                    }
                }
                
                // var isGrounded = Physics.Raycast(rayBottom, _distance, _groundMask);
                IsTouchingGround = isGrounded;
                if (IsTouchingGround==false)
                {
                    AboveGroundEvent?.Invoke();
                }
            }).AddTo(this);
        }

        public async void ProcessingJumpCheck()
        {
            _cts = new CancellationTokenSource();
            float safetySec = 1f;
            while (safetySec > 0)
            {
                var canceled = await UniTask.WaitForFixedUpdate(_cts.Token).SuppressCancellationThrow();
                if (canceled || IsTouchingGround)
                {
                    return;
                }

                safetySec -= Time.fixedDeltaTime;
            }
            AboveGroundEvent?.Invoke();
        }
        
        public bool ValidateSlope()
        {
            return Vector3.Angle(Vector3.up, _currentNormal)<=_slopeAngleLimit;
        }
        
        
        public Vector3 GetNormalProjectedVector(Vector3 dir)
        {
            var finalVector = Vector3.ProjectOnPlane(dir, _currentNormal);
            
#if UNITY_EDITOR
            if (dir != Vector3.zero)
            {
                EDITOR_normalDir = finalVector;
            }
#endif
            return finalVector;
        }

        private void OnDisable()
        {
            _raycastChecker?.Dispose();
            _cts?.Dispose();
        }

#if UNITY_EDITOR
        
        private Vector3 EDITOR_normalDir;
        [SerializeField] private bool EDITOR_raycastsVisual;
        
        
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            if (EDITOR_normalDir!=Vector3.zero)
            {
                Handles.DrawLine(transform.position, transform.position+EDITOR_normalDir, 3f);
            }

            if (EDITOR_raycastsVisual)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(_bottomPoint.position, Vector3.forward*_distance);
                Gizmos.DrawRay(_bottomPoint.position, Vector3.right*_distance);
                Gizmos.DrawRay(_bottomPoint.position, Vector3.back*_distance);
                Gizmos.DrawRay(_bottomPoint.position, Vector3.left*_distance);
                Gizmos.DrawRay(_bottomPoint.position, Vector3.down*_distance);
            }

        }
        
#endif
    }
}
