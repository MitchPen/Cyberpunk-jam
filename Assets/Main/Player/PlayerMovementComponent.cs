using System;
using Main.Player.Camera;
using UnityEngine;

namespace Main.Player
{
    public class PlayerMovementComponent : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private PlayerCameraHolder _cameraHolder;
        [SerializeField] private SurfaceChecker _surfaceChecker;
        [SerializeField] private Rigidbody _rb;
        [Header("Movement")]
        [SerializeField] private float _maxMovementSpeed = 10f;
        [SerializeField] private float _velocityMultiplier = 6f;
        [Header("Jumping")]
        [SerializeField] private float _jumpForce = 1.5f;
        [SerializeField] private float _airMultiplier = 0.3f;
        [SerializeField] private float _gravity;
        [SerializeField] private float _slopesGravityMultiplier = 15f;

        private float _horizontalInput;
        private float _verticalInput;
        private bool _preparingToJump=true;

        private float deltatime;
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
        
        private void OnEnable()
        {
            _surfaceChecker.AboveGroundEvent += OnAboveGround;
        }

        private void OnDisable()
        {
            _surfaceChecker.AboveGroundEvent -= OnAboveGround;
        }
        
        private void FixedUpdate()
        {
            Move();
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKey(KeyCode.Space) && _surfaceChecker.IsTouchingGround && _preparingToJump==false)
            {
                if (_surfaceChecker.ValidateSpaceAbove())
                {
                    Jump();
                }
            }

            ValidateSpeed();
            // Debug.LogWarning(1/Time.deltaTime);
        }

        
        private void Move()
        {
            _cameraHolder.GetCorrectedVectors(out Vector3 forwardVector, out Vector3 rightVector);

            if (_surfaceChecker.ValidateSlope()==false && _surfaceChecker.IsTouchingGround)
            {
                _rb.AddForce(Vector3.down*_slopesGravityMultiplier, ForceMode.Impulse);
            }
            
            var velocity =  ( forwardVector * _verticalInput +rightVector * _horizontalInput).normalized
                            *_velocityMultiplier;
            
            if (_surfaceChecker.IsTouchingGround==false || _preparingToJump)
            { 
                SimulateGravity();

                velocity = new Vector3(velocity.x * _airMultiplier, velocity.y, velocity.z*_airMultiplier);
            }
            else
            {
                velocity = _surfaceChecker.GetNormalProjectedVector(velocity);
            }
            
            _rb.AddForce(velocity,ForceMode.Impulse);
        }

        private void SimulateGravity()
        {
            float yVelocityByDrag = _rb.velocity.y * _rb.drag;
            float dragCoef = (1 - Time.deltaTime * _rb.drag);
                
            _rb.AddForce(Vector3.up * ((yVelocityByDrag + _gravity) / dragCoef * _rb.mass));
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

        private void Jump()
        {
            _preparingToJump = true;

            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * _velocityMultiplier * _jumpForce, ForceMode.Impulse);
            _surfaceChecker.ProcessingJumpCheck();
        }

        private void OnAboveGround()
        {
            _preparingToJump = false;
        }
    }
}
