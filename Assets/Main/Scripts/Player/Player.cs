using System;
using Main.Player;
using Main.Scripts.Player.Weapons;
using Main.Scripts.Player.Weapons.Projectile_Weapon;
using UnityEngine;
using Zenject;

namespace Main.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        public event Action OnPlayerDied;
        [SerializeField] private PlayerWeaponSystem _weaponSystem;
        [SerializeField] private PlayerMovementComponent _movementComponent;
        private ProjectileWeaponConfig _weapons;
        private MovementConfig _movementConfig;
        private Input.InputSystem _inputSystem;
        
        [Inject]
        public void Initialize(Input.InputSystem inputSystem,ProjectileWeaponConfig weapons,MovementConfig movementConfig)
        {
            _inputSystem = inputSystem;
            _weapons = weapons;
            _movementConfig = movementConfig;
        }

        public void Start()
        {
            Application.targetFrameRate = 60;
            Setup();
            Enable();
        }

        public void Setup()
        {
            _weaponSystem.Initialize(_weapons, _inputSystem);
            _movementComponent.Initialize(_inputSystem, _movementConfig);
        }

        public void Enable()
        {
            _inputSystem.Enable();
            _movementComponent.Enable();
            _weaponSystem.Enable();
        }

        public void Disable()
        {
            _inputSystem.Disable();
            _movementComponent.Disable();
            _weaponSystem.Disable();
        }
    }
}