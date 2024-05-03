using Main.Scripts.Weapons.Projectile_Weapon;
using UnityEngine;

namespace Main.Scripts.Weapons.Projectile
{
    [CreateAssetMenu(fileName = "ProjectileWeaponSetup", menuName = "Configuration/Weapons/New ProjectileWeaponSetup")]
    public class ProjectileWeaponSetup : ScriptableObject
    {
        public WeaponType Type;
        public DefaultProjectileWeapon Weapon;
        public ProjectileWeaponData Data;
    }
}