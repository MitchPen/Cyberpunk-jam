using Content.Audio;
using Main.Player.Camera;
using Main.Scripts.Input;
using Main.Scripts.InputSystem;
using Main.Scripts.Player;
using UnityEngine;

namespace Main.Player
{
    public class PlayerMovementComponent : MonoBehaviour
    {
        [Header("General")] [SerializeField] private PlayerCameraHolder _cameraHolder;
        [SerializeField] private PlayerSurfaceHelperComponent _playerSurfaceHelperComponent;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private AudioSource _jumpSound;
        private MovementConfig _movementConfig;
        private InputSystem _inputSystem;
        private float _horizontalInput;
        private float _verticalInput;

        private bool _preparingToJump;
        private bool _jumpKeyPressed;
        private Vector3 _currentCalculatedVelocity;
        private float _lastJumpTime;
        private bool _enabled;

        public void Initialize(InputSystem inputSystem, MovementConfig movementConfig)
        {
            _inputSystem = inputSystem;
            _movementConfig = movementConfig;
        }

        public void Enable()
        {
            _enabled = true;
            _inputSystem.SubscribeOnInputEvent(KeyEvents.JUMP, () => { _jumpKeyPressed = true; });
        }

        public void Disable()
        {
            _enabled = false;
            _inputSystem.ClearEventHandlerOn(KeyEvents.JUMP);
        }

        private void FixedUpdate() //order matters
        {
            if(_enabled==false) return;
            _currentCalculatedVelocity = Vector3.zero;

            var isGrounded = RetrieveGroundedInfo();

            TrySetGroundDistance(isGrounded); //most important method

            HandleMovementVelocity(isGrounded);

            HandleJumping(isGrounded);

            CheckOnJustLanded();

            Move(_currentCalculatedVelocity, isGrounded);
        }

        private void Update()
        {
            if (_enabled == false) return;

            _horizontalInput = _inputSystem.GetGeyboardAxisRaw().x;
            _verticalInput = _inputSystem.GetGeyboardAxisRaw().y;
        }

        private void Move(Vector3 velocity, bool isGrounded)
        {
            velocity = ValidateVelocity(velocity, isGrounded);
            _rb.AddForce(velocity, ForceMode.Impulse);
        }

        private void HandleMovementVelocity(bool isGrounded)
        {
            var velocity = GetInputNormalizedDirection() * _movementConfig.velocityMultiplier;

            if (isGrounded == false)
            {
                velocity += SimulateGravity();
                velocity = new Vector3(velocity.x * _movementConfig.airMultiplier, velocity.y,
                    velocity.z * _movementConfig.airMultiplier);
            }
            else
            {
                velocity = _playerSurfaceHelperComponent.GetNormalProjectedVector(velocity);

                if (_playerSurfaceHelperComponent.ValidateSlope() == false) //too steep slope
                {
                    velocity = new Vector3(velocity.x * _movementConfig.airMultiplier / 3, velocity.y,
                        velocity.z * _movementConfig.airMultiplier / 3);

                    velocity += _playerSurfaceHelperComponent.GetNormalProjectedVector(-transform.up, true) *
                                (_movementConfig.steepSlopesGravityMultiplier * _movementConfig.velocityMultiplier *
                                 Time.fixedDeltaTime);
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

            return _playerSurfaceHelperComponent.CastChecker() && _preparingToJump == false;
        }

        private bool CheckLandedGroundInfo(bool withResetY = true)
        {
            if (_playerSurfaceHelperComponent.JustTouchedGround)
            {
                if (withResetY)
                {
                    CompleteResetY();
                }
                return true;
            }

            return false;
        }

        private void CheckOnJustLanded()
        {
            if (CheckLandedGroundInfo(false) && _rb.velocity.y <= -5f)
            {
                _jumpSound.Play();
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
                    _currentCalculatedVelocity += Vector3.up *
                                                  (_movementConfig.velocityMultiplier * _movementConfig.jumpForce -
                                                   _rb.velocity.y);
                    _lastJumpTime = Time.time;
                    _jumpSound.Play();
                }
            }
        }

        private bool IsJumpPreparingTimeOut() //safety check  
        {
            if (Time.time - (_lastJumpTime + 1f) > 0)
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

            return (forwardVector * _verticalInput + rightVector * _horizontalInput).normalized;
        }

        private void TrySetGroundDistance(bool isGrounded)
        {
            if (isGrounded == false || _playerSurfaceHelperComponent.ValidateSlope() == false)
            {
                return;
            }

            var diff = _playerSurfaceHelperComponent.GetGroundDistanceDifference();

            if (diff > 0.03f)
            {
                var velocity = _rb.velocity;
                velocity = new Vector3(velocity.x, diff / Time.fixedDeltaTime,
                    velocity.z); //use this instead of AddForce to get physics-instant effect 
                _rb.velocity = velocity;
            }
        }

        private Vector3 SimulateGravity()
        {
            var yVelocityByDrag = _rb.velocity.y * _rb.drag;
            var dragCoef = (1 - Time.deltaTime * _rb.drag);

            return Vector3.up *
                   ((yVelocityByDrag + _movementConfig.gravity) / dragCoef * _rb.mass * Time.fixedDeltaTime);
        }

        private Vector3 ValidateVelocity(Vector3 velocity, bool isGrounded)
        {
            var flatVel = new Vector3(velocity.x, 0f, velocity.z);

            if (flatVel.magnitude > _movementConfig.maxVelocity)
            {
                var limitedVel = flatVel.normalized *
                                 (_movementConfig.maxVelocity *
                                  (isGrounded ? _movementConfig.airMultiplier : 1f)); // worth it?
                return new Vector3(limitedVel.x, velocity.y, limitedVel.z);
            }

            return velocity;
        }
    }
}