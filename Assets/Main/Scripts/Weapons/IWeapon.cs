using System;
using UnityEngine;

namespace Main.Scripts.Weapons
{
    public interface IWeapon
    {
        public bool Enable { get; set; }
        public void Shoot();

        public void Setup(WeaponData data);

        public void Show();
        public void Hide();
        public GameObject GetObject { get; }

        public virtual void SetRangeWeaponRaycastPosition(Transform raycastPos)
        {
        }
    }

    public abstract class WeaponData
    {
        public float damage;
        public int attackRate;
    }

    public enum WeaponAttackType
    {
        Range,
        Melee
    }

    public enum WeaponType
    {
        Pistol,
        Rifle,
        AutomaticRifle
    }
}