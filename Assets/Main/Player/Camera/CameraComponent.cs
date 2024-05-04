using Cinemachine;
using Main.Scripts.Input;
using UnityEngine;
using Zenject;

namespace Main.Player.Camera
{
    public class CameraComponent : MonoBehaviour
    {
        public Transform CurrentOrientation => _camera.transform;
        
        [SerializeField] private CinemachineVirtualCamera _vCam;
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private float ms;
        private InputSystem _inputSystem;
        private CinemachinePOV _cmp;

        [Inject]
        public void Initializa(InputSystem inputSystem) => _inputSystem = inputSystem;
      
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _cmp = _vCam.GetCinemachineComponent<CinemachinePOV>();
            UpdateCameraSpeed(_inputSystem.MouseSensitivity);
            _inputSystem.OnMouseSensitivityChanged += UpdateCameraSpeed;
        }

        private void UpdateCameraSpeed(float speed)
        {
            _cmp.m_VerticalAxis.m_MaxSpeed = speed;
            _cmp.m_HorizontalAxis.m_MaxSpeed = speed;
        }
        
        #if UNITY_EDITOR
        
        private bool Editor_Freezed;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)&&Editor_Freezed==false)
            {
                Freeze();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0)&&Editor_Freezed)
            {
                UnFreeze();
            }
        }

        private void Freeze()
        {
            _vCam.enabled = false;
            Editor_Freezed = true;
        }

        private void UnFreeze()
        {
            if (Application.isFocused)
            {
                _vCam.enabled = true;
                Editor_Freezed = false;
            }
        }
        
        #endif
    }
}
