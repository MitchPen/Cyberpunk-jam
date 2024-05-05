using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts.Player.Weapons.Projectile_Weapon
{
    [CreateAssetMenu(fileName = "ProjectileWeapon_CFG", menuName = "Configuration/Player/Create new WeaponConfig")]
    public class ProjectileWeaponConfig : ScriptableObject
    {
        public List<ProjectileWeaponSetup> WeaponSetups = new List<ProjectileWeaponSetup>();
    }
    
    [Serializable]
    public class ProjectileWeaponSetup
    {
        public ProjectileWeapon Weapon;
        public ProjectileWeaponData weaponData;
    }
}