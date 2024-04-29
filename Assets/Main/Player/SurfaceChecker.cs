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
        [SerializeField] private Collider _movementCollider;
        private IDisposable _raycastChecker;
        private IDisposable _enterCollidersChecker;
        private IDisposable _exitCollidersChecker;
        private Vector3 _currentNormal = Vector3.up;
        private List<Collider> _currentGrounds;
        private Collider _suitableGround;
        private CancellationTokenSource _cts;

        private void Start()
        {
            InitGroundsCollector();
            InitGroundChecker();
        }

        private void InitGroundChecker()
        {
            _raycastChecker = this.FixedUpdateAsObservable().Subscribe(x =>
            {
                Ray ray = new Ray(_bottomPoint.position, ValidateSlope()?-_currentNormal: Vector3.down);
                
                var isGrounded = Physics.Raycast(ray, _distance, _groundMask);
                IsTouchingGround = isGrounded;
                if (isGrounded==false)
                {
                    AboveGroundEvent?.Invoke();
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

        private void SetSuitableGround()
        {
            if (_currentGrounds.Count != 0)
            {
                _suitableGround = _currentGrounds.OrderByDescending(x => Vector3.Dot(x.transform.up, Vector3.up)).First();
                Debug.LogWarning("SELECTED NEW GROUND, DOT: "+Vector3.Dot(_suitableGround.transform.up, Vector3.up));
                _currentNormal = _suitableGround.transform.up;
            }
        }
        
        public bool ValidateSlope()
        {
            return Vector3.Angle(Vector3.up, _currentNormal)<=_slopeAngleLimit && _currentGrounds.Count==1;
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
                Gizmos.DrawLine(_bottomPoint.position, _bottomPoint.position+ (ValidateSlope()?-_currentNormal: Vector3.down) *_distance);
            }
            else
            {
                Handles.color = Color.yellow;
                Handles.DrawLine(_bottomPoint.position, _bottomPoint.position + (-_bottomPoint.up.normalized * _distance), 1f);
            }
            
            Handles.color = Color.yellow;
        }
        
#endif
    }
}
