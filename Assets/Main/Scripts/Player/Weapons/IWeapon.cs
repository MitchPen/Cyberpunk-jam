using UnityEngine;

namespace Main.Scripts.Player.Weapons
{
    public interface IWeapon
    {
        public bool Enable { get; set; }
        public void Shoot();

        public void Setup(WeaponData data, Transform raycastPos);

        public void Show();
        public void Hide();
        public GameObject GetObject { get; }
    }

    public abstract class WeaponData
    {
        public float damage;
        public int attackRate;
    }
 
}