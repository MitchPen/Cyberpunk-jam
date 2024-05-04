using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts.Weapons.Melee
{
    [CreateAssetMenu(fileName = "MeleeWeaponConfig", menuName = "Configuration/Weapons/New MeleeWeaponConfig")]
    public class MeleeWeaponConfig : ScriptableObject
    {
        public List<MeleeWeaponSetup> MeleeWeaponSetups = new List<MeleeWeaponSetup>();
    }

    [Serializable]
    public class MeleeWeaponSetup
    {
        public WeaponType Type;
        public MeleeWeapon Weapon;
        public MeleeWeaponData Data;
    }
}
