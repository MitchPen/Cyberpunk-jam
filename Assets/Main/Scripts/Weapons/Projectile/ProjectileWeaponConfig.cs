using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts.Weapons.Projectile
{
    [CreateAssetMenu(fileName = "ProjectileWeapon_CFG", menuName = "Configuration/Weapons/New ProjectileWeaponConfig")]
    public class ProjectileWeaponConfig : ScriptableObject
    {
        public List<ProjectileWeaponSetup> WeaponSetups = new List<ProjectileWeaponSetup>();
    }
}