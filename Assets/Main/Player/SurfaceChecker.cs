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

        public event Action AboveGroundEvent;
        public event Action LandedGroundEvent;

        [SerializeField] private Transform _topPoint;
        [SerializeField] private Transform _bottomPoint;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _slopeAngleLimit;
        [SerializeField] private Collider _movementCollider;
        private IDisposable _raycastChecker;
        private IDisposable _enterCollidersChecker;
        private IDisposable _exitCollidersChecker;
        private Vector3 _currentNormal = Vector3.up;
        private List<Collider> _currentGrounds;
        private Collider _suitableGround;

        private void Start()
        {
            AboveGroundEvent?.Invoke();
            
            InitGroundsCollector();
            InitGroundChecker();
        }

        private void InitGroundChecker()
        {
            _raycastChecker = this.UpdateAsObservable().Subscribe(x =>
            {
                Ray ray2 = new Ray(_bottomPoint.position, ValidateSlope()?-_currentNormal: Vector3.down);
                
                var isGrounded = Physics.Raycast(ray2, _distance, _groundMask);
                
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
            }).AddTo(this);
        }

        private void InitGroundsCollector()
        {
            _currentGrounds = new List<Collider>();
            _enterCollidersChecker = _movementCollider.OnCollisionEnterAsObservable().RepeatUntilDisable(transform).Subscribe(collision =>
            {
                if (_currentGrounds.Contains(collision.collider)==false)
                {
                    _currentGrounds.Add(collision.collider);
                    SetSuitableGround();
                }
            }).AddTo(this);
            
            _exitCollidersChecker = _movementCollider.OnCollisionExitAsObservable().RepeatUntilDisable(transform).Subscribe(collision =>
            {
                if (_currentGrounds.Contains(collision.collider))
                {
                    _currentGrounds.Remove(collision.collider);
                    SetSuitableGround();
                }
            }).AddTo(this);
        }

        private void SetSuitableGround()
        {
            if (_currentGrounds.Count != 0)
            {
                _suitableGround = _currentGrounds.OrderByDescending(x => Vector3.Dot(x.transform.up, Vector3.up)).First();
                Debug.LogWarning("SELECTED NEW GROUND, DOT: "+Vector3.Dot(_suitableGround.transform.up, Vector3.up));
                _currentNormal = _suitableGround.transform.up;
            }
        }
        
        public bool ValidateSpaceAbove()
        {
            return Physics.Raycast(_topPoint.position, Vector3.up, _distance)==false;
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
            _enterCollidersChecker?.Dispose();
            _exitCollidersChecker?.Dispose();
        }

#if UNITY_EDITOR
        [SerializeField] private List<Collider> EDITOR_physicsColliders;
        [SerializeField] private float EDITOR_distanceBetweenPoints;
        private Vector3 EDITOR_normalDir;
        
        [Button]
        private void SetupSurfaceAdditionals()
        {
            if (EDITOR_physicsColliders != null && EDITOR_physicsColliders.Count != 0)
            {
                var minY = EDITOR_physicsColliders.Min(x => x.bounds.min.y) + EDITOR_distanceBetweenPoints;
                var maxY = EDITOR_physicsColliders.Max(x => x.bounds.max.y) - EDITOR_distanceBetweenPoints;
                
                _topPoint.position = new Vector3(transform.position.x, maxY, transform.position.z);
                _bottomPoint.position = new Vector3(transform.position.x, minY, transform.position.z);
            }
        }

        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            if (EDITOR_normalDir!=Vector3.zero)
            {
                Handles.DrawLine(transform.position, transform.position+EDITOR_normalDir, 3f);
                Gizmos.color = Color.yellow;   
                Gizmos.DrawLine(_bottomPoint.position, _bottomPoint.position+ (ValidateSlope()?-_currentNormal: Vector3.down) *_distance);
            }
            else
            {
                Handles.color = Color.yellow;
                Handles.DrawLine(_bottomPoint.position, _bottomPoint.position + (-_bottomPoint.up.normalized * _distance), 1f);
            }
            
            Handles.color = Color.yellow;
            Handles.DrawLine(_topPoint.position, _topPoint.position + (_topPoint.up.normalized * _distance), 1f);
        }
        
#endif
    }
}
