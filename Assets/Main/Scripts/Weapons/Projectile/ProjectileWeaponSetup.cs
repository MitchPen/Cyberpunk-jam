using UnityEngine;

namespace Main.Scripts.Weapons.Projectile
{
    [CreateAssetMenu(fileName = "ProjectileWeaponSetup", menuName = "CONFIGS/WEAPONS/New ProjectileWeaponSetup")]
    public class ProjectileWeaponSetup : ScriptableObject
    {
        public DefaultProjectileWeapon Weapon;
        public ProjectileWeaponData Data;
    }
}