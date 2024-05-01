using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Main.Player
{
    public class PlayerSurfaceHelperComponent : MonoBehaviour
    {
        public bool IsTouchingGround { get;private set; }

        public event Action AboveGroundedEvent;
        public event Action LandedEvent;
        
        [SerializeField] private Transform _groundCheckerRaycastTr;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _slopeAngleLimit;
        private Vector3 _currentNormal = Vector3.up;
        private CancellationTokenSource _cts;
        private float _hitDistance;
        
        public bool CastChecker()
        {
            Ray ray = new Ray(_groundCheckerRaycastTr.position,Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit,_distance, _groundMask))
            {
                IsTouchingGround = true;
                _currentNormal = hit.normal;
                 _hitDistance = hit.distance;
                 return true;
                 //_hitDistance = Mathf.Abs(_groundCheckerRaycastTr.position.y - hit.point.y);
            }
            else
            {
                IsTouchingGround = false;
                AboveGroundedEvent?.Invoke();
                _currentNormal = Vector3.up;
                _hitDistance = 0f;
            }

            return false;
            Debug.LogWarning(IsTouchingGround);
        }

        public float GetGroundDistanceDifference()
        {
            var diff = (_distance*0.9f) - _hitDistance;
            return diff ;
        }
        
        public async void ProcessingJumpCheck() //safety check  
        {
            _cts = new CancellationTokenSource();
            float safetySec = 1f;
            while (safetySec > 0)
            {
                var canceled = await UniTask.WaitForFixedUpdate(_cts.Token).SuppressCancellationThrow();
                if (canceled || IsTouchingGround==false)
                {
                    return;
                }

                safetySec -= Time.fixedDeltaTime;
            }
            AboveGroundedEvent?.Invoke();
        }
        
        public bool ValidateSlope()
        {
            return Vector3.Angle(Vector3.up, _currentNormal)<=_slopeAngleLimit;
        }
        
        public Vector3 GetNormalProjectedVector(Vector3 dir, bool normalized =false)
        {
            var finalVector = Vector3.ProjectOnPlane(dir, normalized? _currentNormal.normalized : _currentNormal);
            
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
            _cts?.Dispose();
        }

#if UNITY_EDITOR
        
        private Vector3 EDITOR_normalDir;

        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            if (EDITOR_normalDir!=Vector3.zero)
            {
                Handles.DrawLine(transform.position, transform.position+EDITOR_normalDir, 3f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_groundCheckerRaycastTr.position, _groundCheckerRaycastTr.position+ (ValidateSlope()?-_currentNormal: Vector3.down) *_distance);
            }
            else
            {             
                Gizmos.color = Color.yellow;
                Handles.DrawLine(_groundCheckerRaycastTr.position, _groundCheckerRaycastTr.position + (-_groundCheckerRaycastTr.up.normalized * _distance), 1f);
            }
        }
        
#endif
    }
}
