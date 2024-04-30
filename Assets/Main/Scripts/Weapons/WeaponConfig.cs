using Main.Scripts.Weapons.Projectile;
using UnityEngine;

namespace Main.Scripts.Weapons
{
    [CreateAssetMenu( fileName = "Weapon_CFG", menuName = "Configuration/Weapons/New WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        public ProjectileWeaponConfig ProjectileWeapons;
    }
}
