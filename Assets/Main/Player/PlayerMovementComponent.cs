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

        private float _horizontalInput;
        private float _verticalInput;
        private float _defaultDrag;
        private bool _isInJump=true;

        private float deltatime;
        
        private void Awake()
        {
            _defaultDrag = _rb.drag;
            Application.targetFrameRate = 60;
        }
        
        private void OnEnable()
        {
            _surfaceChecker.AboveGroundEvent += OnAboveGround;
            _surfaceChecker.LandedGroundEvent += OnLandedGround;
        }

        private void OnDisable()
        {
            _surfaceChecker.AboveGroundEvent -= OnAboveGround;
            _surfaceChecker.LandedGroundEvent -= OnLandedGround;
        }
        
        private void FixedUpdate()
        {
            Move();
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKey(KeyCode.Space) && _isInJump==false)
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

            var velocity =  ( forwardVector * _verticalInput +rightVector * _horizontalInput).normalized
                            *_velocityMultiplier;
            
            if (_surfaceChecker.IsGrounded==false || _isInJump)
            {
                velocity *= _airMultiplier;
                
                float yVelocityByDrag = _rb.velocity.y * _rb.drag;
                float dragCoef = (1 - Time.deltaTime * _rb.drag);
                
                _rb.AddForce(new Vector3 (0, ((yVelocityByDrag + _gravity) / dragCoef) * _rb.mass, 0));
            }

            var finalVelocity = Vector3.ProjectOnPlane(velocity, _surfaceChecker.CurrentNormal);
            _rb.AddForce(finalVelocity,ForceMode.Impulse);
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
            _isInJump = true;

            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * _velocityMultiplier * _jumpForce, ForceMode.Impulse);
        }

        private void OnAboveGround()
        {
            
        }

        private void OnLandedGround()
        {
            _rb.drag = _defaultDrag;
            _isInJump = false;
        }
    }
}
