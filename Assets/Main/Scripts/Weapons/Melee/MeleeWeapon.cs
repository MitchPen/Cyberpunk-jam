using System;
using Main.Scripts.Weapons.Projectile_Weapon;
using UnityEngine;

namespace Main.Scripts.Weapons.Melee
{
    public class MeleeWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] private Collider _collider;
        [SerializeField] public MeleeWeaponAnimator _animator;
        private MeleeWeaponData _data;
        private bool _attacking = false;
        protected bool _onAnimation;
        public GameObject GetObject => this.gameObject;

        public bool Enable { get; set; }

        public void Attack()
        {
            if (_attacking || _onAnimation) return;

            _collider.enabled = true;
            _attacking = true;
            _animator.PlayMeeleAttackAnimation(() =>
            {
                _collider.enabled = false;
                _attacking = false;
            });
        }

        public void Setup(WeaponData data)
        {
            _data = (MeleeWeaponData) data;
            _animator.Init(transform);
            _collider.enabled = false;
            _attacking = false;
            _onAnimation = false;
        }

        public void Show()
        {
            _onAnimation = true;
            gameObject.SetActive(true);
            _animator.PlayShowAnimation(() => { _onAnimation = false; });
        }

        public void Hide()
        {
            _collider.enabled = false;
            _attacking = false;
            gameObject.SetActive(false);
            _animator.DisableAllAnimation(() => { _onAnimation = false; });
        }

        public void OnTriggerEnter(Collider other)
        {
           
        }
    }

    [Serializable]
    public class MeleeWeaponData : WeaponData
    {
        public AudioClip ShotSound;
        public int attackSpeed;
    }
}