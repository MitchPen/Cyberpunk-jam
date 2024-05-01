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
        [SerializeField] private float _maxVelocity = 10f;
        [SerializeField] private float _velocityMultiplier = 6f;
        [Header("Jumping")]
        [SerializeField] private float _jumpForce = 2f;
        [SerializeField] private float _airMultiplier = 0.8f;
        [SerializeField] private float _gravity = -15f;
        [SerializeField] private float _steepSlopesGravityMultiplier = 40f;

        private float _horizontalInput;
        private float _verticalInput;
        
        private bool _preparingToJump;
        private bool _jumpKeyPressed;
        private Vector3 _currentCalculatedVelocity;
        private float _lastJumpTime;
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void FixedUpdate() //order matters
        {
            _currentCalculatedVelocity = Vector3.zero;

            var isGrounded = RetrieveGroundedInfo();
            
            TrySetGroundDistance(isGrounded); //most important method
            
            HandleMovementVelocity(isGrounded);
            
            HandleJumping(isGrounded);
            
            //CheckLandedGroundInfo(); //if need to set Y to 0 on landed

            Move(_currentCalculatedVelocity,isGrounded);
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKey(KeyCode.Space))
            {
                _jumpKeyPressed = true;
            }
        }

        private void Move(Vector3 velocity, bool isGrounded)
        {
            velocity = ValidateVelocity(velocity, isGrounded);
            _rb.AddForce(velocity,ForceMode.Impulse);
        }

        private void HandleMovementVelocity(bool isGrounded)
        {
            var velocity = GetInputNormalizedDirection() * _velocityMultiplier;

            if (isGrounded==false)
            {
                velocity += SimulateGravity();
                velocity = new Vector3(velocity.x * _airMultiplier, velocity.y, velocity.z*_airMultiplier);
            }
            else
            {
                velocity = _playerSurfaceHelperComponent.GetNormalProjectedVector(velocity);
                
                if (_playerSurfaceHelperComponent.ValidateSlope()==false) //too steep slope
                {
                    velocity = new Vector3(velocity.x * _airMultiplier/3, velocity.y, velocity.z*_airMultiplier/3);
            
                    velocity += _playerSurfaceHelperComponent.GetNormalProjectedVector(-transform.up, true)
                                * _steepSlopesGravityMultiplier*_velocityMultiplier *Time.fixedDeltaTime;
                }
            }

            _currentCalculatedVelocity += velocity;
        }

        private bool RetrieveGroundedInfo()
        {
            if (_preparingToJump)
            {
                if (_playerSurfaceHelperComponent.JustAboveGround || IsJumpPreparingTimeOut())
                {
                    _preparingToJump = false;
                }
            }
            
            return _playerSurfaceHelperComponent.CastChecker() && _preparingToJump==false;
        }

        private void CheckLandedGroundInfo()
        {
            if (_playerSurfaceHelperComponent.JustTouchedGround)
            {
                CompleteResetY();
            }
        }
        
        private void HandleJumping(bool isGrounded)
        {
            if (_jumpKeyPressed)
            {
                _jumpKeyPressed = false;
                
                if (isGrounded && _playerSurfaceHelperComponent.ValidateSlope())
                {
                    _preparingToJump = true;
                    
                    CheckLandedGroundInfo();
                    _currentCalculatedVelocity += Vector3.up *(_velocityMultiplier * _jumpForce - _rb.velocity.y);
                    _lastJumpTime = Time.time;
                }
            }
        }
        
        private bool IsJumpPreparingTimeOut() //safety check  
        {
            if (Time.time-(_lastJumpTime+1f)>0)
            {
                return true;
            }

            return false;
        }

        private void CompleteResetY()
        {
            _currentCalculatedVelocity.y = 0f;
            var velocity = _rb.velocity;
            velocity = new Vector3(velocity.x, 0f, velocity.z);
            _rb.velocity = velocity;
        }

        private Vector3 GetInputNormalizedDirection()
        {
            _cameraHolder.GetCorrectedVectors(out var forwardVector, out var rightVector);
            
            return (forwardVector * _verticalInput +rightVector * _horizontalInput).normalized;
        } 
        
        private void TrySetGroundDistance(bool isGrounded)
        {
            if (isGrounded==false || _playerSurfaceHelperComponent.ValidateSlope()==false)
            {
                return;
            }
            var diff = _playerSurfaceHelperComponent.GetGroundDistanceDifference();

            if (diff>0.03f)
            {
                var velocity = _rb.velocity;
                velocity = new Vector3(velocity.x, diff / Time.fixedDeltaTime, velocity.z); //use this instead of AddForce to get physics-instant effect 
                _rb.velocity = velocity;
            }
        }

        private Vector3 SimulateGravity()
        {
            var yVelocityByDrag = _rb.velocity.y * _rb.drag;
            var dragCoef = (1 - Time.deltaTime * _rb.drag);

            return Vector3.up * ((yVelocityByDrag + _gravity) / dragCoef * _rb.mass)*Time.fixedDeltaTime;
        }
        
        private Vector3 ValidateVelocity(Vector3 velocity, bool isGrounded)
        {
            var flatVel = new Vector3(velocity.x, 0f, velocity.z);
            
            if (flatVel.magnitude>_maxVelocity)
            {
                var limitedVel = flatVel.normalized * _maxVelocity*(isGrounded?_airMultiplier:1f); // worth it?
                return new Vector3(limitedVel.x, velocity.y, limitedVel.z);
            }

            return velocity;
        }
    }
}
