using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Main.Player
{
    public class PlayerSurfaceHelperComponent : MonoBehaviour
    {

        public bool JustAboveGround { get; private set; }
        public bool JustTouchedGround { get; private set; }
        
        [SerializeField] private Transform _groundCheckerRaycastTr;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _slopeAngleLimit;
        private Vector3 _currentNormal = Vector3.up;
        private float _hitDistance;
        
        private bool _isTouchingGround;

        
        public bool CastChecker()
        {
            ResetFlags();
            
            Ray ray = new Ray(_groundCheckerRaycastTr.position,Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, _distance, _groundMask))
            {
                if (_isTouchingGround==false)
                {
                    JustTouchedGround = true;
                }
                _currentNormal = hit.normal;
                 _hitDistance = hit.distance;
                 
                 _isTouchingGround = true;
            }
            else
            {
                if (_isTouchingGround)
                {
                    JustAboveGround = true;
                }
                
                _isTouchingGround = false;
            }

            return _isTouchingGround;
        }

        private void ResetFlags()
        {
            JustTouchedGround = false;
            JustAboveGround = false;
            _currentNormal = Vector3.up;
            _hitDistance = 0f;
        }

        public float GetGroundDistanceDifference()
        {
            var diff = (_distance *0.9f - _hitDistance);
            return diff ;
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
