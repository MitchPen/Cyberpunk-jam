using System;
using System.Collections.Generic;
using Main.Scripts.Weapons.Projectile_Weapon;
using UnityEngine;

namespace Main.Scripts.Weapons.Projectile
{
    [CreateAssetMenu(fileName = "ProjectileWeapon_CFG", menuName = "Configuration/Weapons/New ProjectileWeaponConfig")]
    public class ProjectileWeaponConfig : ScriptableObject
    {
        public List<ProjectileWeaponSetup> WeaponSetups = new List<ProjectileWeaponSetup>();
    }
    
    [Serializable]
    public class ProjectileWeaponSetup
    {
        public WeaponType Type;
        public ProjectileWeapon Weapon;
        public ProjectileWeaponData weaponData;
    }
}