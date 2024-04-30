using UnityEngine;

namespace Main.Scripts.Weapons
{
    public interface IWeapon
    {
        public void Attack();
        public virtual void AlternativeAction() { }
        public void Reload();
        public void Setup(WeaponData data);

        public void Show();
        public void Hide();
        public GameObject GetObject { get; }
        public WeaponType GetWeaponType { get; }
        public void SetRangeWeaponRaycastPosition(Transform raycastPos);
    }

    public abstract class WeaponData
    {
        public float damage;
        public int attackRate;
        public float reloadTime;
    }

    public enum WeaponType
    {
        Range,Melee
    }
}