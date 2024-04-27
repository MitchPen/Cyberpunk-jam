using System;
using Cinemachine;
using UnityEngine;

namespace Main.Player.Camera
{
    public class PlayerCameraComponent : MonoBehaviour
    {
        public Transform CurrentOrientation => _camera.transform;
        
        [SerializeField] private CinemachineVirtualCamera _vCam;
        [SerializeField] private UnityEngine.Camera _camera;
        
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
