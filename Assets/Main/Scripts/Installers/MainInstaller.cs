using Main.Scripts.Input;
using Main.Scripts.Player;
using Main.Scripts.Player.Weapons.Projectile_Weapon;
using UnityEngine;
using Zenject;

namespace Main.Scripts.Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private Input.InputSystem _inputSystem;
        [Space(10)] [Header("Configs")]
        [SerializeField] private InputConfig _inputConfig;
        [SerializeField] private MovementConfig _movementConfig;
        [SerializeField] private ProjectileWeaponConfig _projectileWeapon;

        public override void InstallBindings()
        {
            InstallConfigs();
            InstallServices();
        }

        public void InstallConfigs()
        {
            Container.Bind<InputConfig>()
                .FromInstance(_inputConfig)
                .AsSingle()
                .NonLazy();
            Container.Bind<MovementConfig>()
                .FromInstance(_movementConfig)
                .AsSingle()
                .NonLazy();
            Container.Bind<ProjectileWeaponConfig>()
                .FromInstance(_projectileWeapon)
                .AsSingle()
                .NonLazy();
        }

        public void InstallServices()
        {
            Container.Bind<Input.InputSystem>()
                .FromInstance(_inputSystem)
                .AsSingle()
                .NonLazy();
        }
    }
}