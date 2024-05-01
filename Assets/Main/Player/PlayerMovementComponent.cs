using System;
using Main.Player.Camera;
using UnityEngine;

namespace Main.Player
{
    public class PlayerMovementComponent : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private PlayerCameraHolder _cameraHolder;
        [SerializeField] private PlayerSurfaceHelperComponent _playerSurfaceHelperComponent;
        [SerializeField] private Rigidbody _rb;
        [Header("Movement")]
        [SerializeField] private float _maxMovementSpeed = 10f;
        [SerializeField] private float _velocityMultiplier = 6f;
        [Header("Jumping")]
        [SerializeField] private float _jumpForce = 1.5f;
        [SerializeField] private float _airMultiplier = 0.3f;
        [SerializeField] private float _gravity;
        [SerializeField] private float _steepSlopesGravityMultiplier = 15f;

        private float _horizontalInput;
        private float _verticalInput;
        private bool _preparingToJump=true;
        private bool InJumpingProcess => _playerSurfaceHelperComponent.IsTouchingGround == false || _preparingToJump;
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
        
        private void OnEnable()
        {
            _playerSurfaceHelperComponent.AboveGroundedEvent += OnAboveGrounded;
        }

        private void OnDisable()
        {
            _playerSurfaceHelperComponent.AboveGroundedEvent -= OnAboveGrounded;
        }
        
        private void FixedUpdate()
        {
            Move();
            ValidateSpeed();
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKey(KeyCode.Space) && InJumpingProcess==false)
            {
                Jump();
            }
        }

        private void Move()
        {
            _playerSurfaceHelperComponent.CastChecker();
            
            _cameraHolder.GetCorrectedVectors(out Vector3 forwardVector, out Vector3 rightVector);
            
            var velocity =  ( forwardVector * _verticalInput +rightVector * _horizontalInput).normalized
                            *_velocityMultiplier;
            
            if (InJumpingProcess)
            {
                velocity += SimulateGravity();

                velocity = new Vector3(velocity.x * _airMultiplier, velocity.y, velocity.z*_airMultiplier);
            }
            else
            {
                velocity = _playerSurfaceHelperComponent.GetNormalProjectedVector(velocity);

                if (_playerSurfaceHelperComponent.ValidateSlope()==false)
                {
                    velocity = new Vector3(velocity.x * _airMultiplier/2, velocity.y, velocity.z*_airMultiplier/2);

                    velocity += _playerSurfaceHelperComponent.GetNormalProjectedVector(-transform.up, true)
                                * _steepSlopesGravityMultiplier*_velocityMultiplier *Time.fixedDeltaTime;
                }
                else
                {
                    var vel = _rb.velocity;
                    vel.y = 0f;
                    _rb.velocity = vel;
                }
                
                velocity += TryFloatPositionRb();
            }

            _rb.AddForce(velocity,ForceMode.Impulse);
        }
        
        private Vector3 TryFloatPositionRb()
        {
            if (InJumpingProcess==false)
            {
                var diff = _playerSurfaceHelperComponent.GetGroundDistanceDifference();
                if (Mathf.Approximately(diff,0)==false)
                {
                    return transform.up * (diff / Time.fixedDeltaTime);
                }
            }
            return Vector3.zero;
        }

        private Vector3 SimulateGravity()
        {
            float yVelocityByDrag = _rb.velocity.y * _rb.drag;
            float dragCoef = (1 - Time.deltaTime * _rb.drag);

            return Vector3.up * ((yVelocityByDrag + _gravity) / dragCoef * _rb.mass)*Time.fixedDeltaTime;
        }
        
        private void ValidateSpeed()
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            
            if (flatVel.magnitude>_maxMovementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _maxMovementSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }

        private void Jump() //TODO Restrict jump while sloping
        {
            _preparingToJump = true;

            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * _velocityMultiplier * _jumpForce, ForceMode.Impulse);
            _playerSurfaceHelperComponent.ProcessingJumpCheck();
        }

        private void OnAboveGrounded()
        {
            _preparingToJump = false;
        }

        private void CheckJumpInfo() //remove constant AboveGround event
        {
            if (_playerSurfaceHelperComponent)
            {
                
            }
        }
    }
}
