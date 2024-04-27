using System;
using Main.Player.Camera;
using UnityEngine;

namespace Main.Player
{
    public class PlayerMovementComponent : MonoBehaviour
    {
        [SerializeField] private PlayerCameraComponent _cameraComponent;
        [SerializeField] private GroundChecker _groundChecker;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _maxMovementSpeed = 5f;
        [SerializeField] private float _velocityMultiplier = 80f;
        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private float _airMultiplier = 0.3f;
        private float _horizontalInput;
        private float _verticalInput;
        private float _defaultDrag;
        private bool _isReadyToJump=true;

        private float deltatime;
        
        private void Awake()
        {
            _defaultDrag = _rb.drag;
            Application.targetFrameRate = 60;
        }
        

        private void FixedUpdate()
        {
            Move();
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKey(KeyCode.Space) && _groundChecker.IsGrounded && _isReadyToJump)
            {
                Jump();
                _groundChecker.LandedGroundEvent += OnLandedGround;
            }
            
            ValidateSpeed();
            Debug.LogWarning(1/Time.deltaTime);
        }

        private void Move()
        {
            var forwardVector =
                Vector3.ProjectOnPlane(_cameraComponent.CurrentOrientation.forward, Vector3.up);
            var rightVector = _cameraComponent.CurrentOrientation.right;
            
            var velocity =  ( forwardVector * _verticalInput +rightVector * _horizontalInput).normalized
                             *_velocityMultiplier*Time.fixedDeltaTime;
            
            if (_groundChecker.IsGrounded==false)
            {
                velocity *= _airMultiplier;
            }

            _rb.AddForce(velocity,ForceMode.Impulse);
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
            _isReadyToJump = false;
            _rb.drag = 0f;

            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * _velocityMultiplier * _jumpForce *Time.fixedDeltaTime, ForceMode.Impulse);
        }

        private void OnAboveGround()
        {
        }

        private void OnLandedGround()
        {
            _groundChecker.LandedGroundEvent -= OnLandedGround;
            _rb.drag = _defaultDrag;
            _isReadyToJump = true;
        }

        private void OnDisable()
        {
            _groundChecker.AboveGroundEvent -= OnAboveGround;
            _groundChecker.LandedGroundEvent -= OnLandedGround;
        }
    }
}
