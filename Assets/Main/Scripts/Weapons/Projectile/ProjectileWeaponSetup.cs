using UnityEngine;

namespace Main.Scripts.Weapons.Projectile
{
    [CreateAssetMenu(fileName = "ProjectileWeaponSetup", menuName = "CONFIGS/WEAPONS/New ProjectileWeaponSetup")]
    public class ProjectileWeaponSetup : ScriptableObject
    {
        public BaseProjectileWeapon Weapon;
        public ProjectileWeaponData Data;
    }
}