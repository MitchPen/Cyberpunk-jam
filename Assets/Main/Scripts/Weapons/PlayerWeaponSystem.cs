using System;
using System.Collections.Generic;
using Main.Scripts.Weapons.Melee;
using Main.Scripts.Weapons.Projectile;
using Main.Scripts.Weapons.Projectile_Weapon;
using Unity.VisualScripting;
using UnityEngine;

namespace Main.Scripts.Weapons
{
    public class PlayerWeaponSystem : MonoBehaviour
    {
        [SerializeField] private Camera _fpsCamera;
        [SerializeField] private Transform _handPoint;
        [SerializeField] private Transform _weaponShowPoint;
        [SerializeField] private WeaponShowAnimationComponent _showAnimator;
        [SerializeField] private WeaponConfig _testConfig;
        private List<IWeapon> _weaponOrder = new List<IWeapon>();
        private IWeapon _currentWeapon;
        private int _currentIndex;

        void Start()
        {
            Initialize(_testConfig);
        }

        public void Initialize(WeaponConfig weapons)
        {
            _showAnimator.Setup(_weaponShowPoint);
            SetupRangeWeapons(weapons.ProjectileWeapons.WeaponSetups);
            ChangeWeaponByIndex(0);
        }

        private void SetupRangeWeapons( IEnumerable<ProjectileWeaponSetup> weapons)
        {
            foreach (var item in weapons)
            {
                var newWeapon = Instantiate(item.Weapon.GetObject, _handPoint)
                    .GetComponent<IWeapon>();
                newWeapon.GetObject.transform.localPosition = Vector3.zero;
                newWeapon.Setup(item.weaponData);
                newWeapon.SetRangeWeaponRaycastPosition(_fpsCamera.transform);
                newWeapon.Hide();
                _weaponOrder.Add(newWeapon);
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (_currentWeapon == null) return;
                _currentWeapon.Attack();
            }

            if(Input.mouseScrollDelta.y<0f)
                ChangeWeaponByDirection(WeaponChangeDirection.NEXT);
            
            if(Input.mouseScrollDelta.y>0f)
                ChangeWeaponByDirection(WeaponChangeDirection.PREV);
        }
        
        
        private void ChangeWeapon()
        {
            if (_currentWeapon != null)
            {
                if (_currentWeapon == _weaponOrder[_currentIndex]) return;

                _currentWeapon.Hide();
                _currentWeapon.Enable = false;
            }

            _currentWeapon = _weaponOrder[_currentIndex];
            _currentWeapon.Enable = false;
            _currentWeapon.Show();
            _showAnimator.PlayShowAnimation(_currentWeapon.GetObject.transform, () =>
            {
                _currentWeapon.Enable = true;
            });
        }

        private void ChangeWeaponByIndex(int index)
        {
            if (_weaponOrder.Count <= index) return;
            _currentIndex = index;
            ChangeWeapon();
        }

        private void ChangeWeaponByDirection(WeaponChangeDirection direction)
        {
            switch (direction)
            {
                case WeaponChangeDirection.NEXT:
                {
                    if (_currentIndex + 1 >= _weaponOrder.Count)
                        _currentIndex = 0;
                    else
                        _currentIndex++;
                    break;
                }
                case WeaponChangeDirection.PREV:
                {
                    if (_currentIndex - 1 < 0)
                        _currentIndex = _weaponOrder.Count - 1;
                    else
                        _currentIndex--;
                    break;
                }
            }

            ChangeWeapon();
        }

        private enum WeaponChangeDirection
        {
            NEXT,
            PREV
        }
    }
}