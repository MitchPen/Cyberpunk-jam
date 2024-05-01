using System;
using System.Collections.Generic;
using Main.Scripts.Weapons.Projectile;
using UnityEngine;

namespace Main.Scripts.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Transform WeaponPoint;
        [SerializeField] private WeaponConfig _testConfig;
        [SerializeField] private Camera _fpsCamera;
        private Dictionary<WeaponType, IWeapon> _weapons = new Dictionary<WeaponType, IWeapon>();
        private List<WeaponType> _weaponOrder = new List<WeaponType>();
        private IWeapon _currentWeapon;
        private int _currentIndex;

        void Start()
        {
      
            Initialize(_testConfig);
        }

        public void Initialize(WeaponConfig weapons)
        {
            foreach (var weapon in weapons.ProjectileWeapons.WeaponSetups)
            {
                var newWeapon = Instantiate(weapon.Weapon.GetObject, WeaponPoint.position, Quaternion.identity)
                    .GetComponent<IWeapon>();
                newWeapon.GetObject.transform.SetParent(WeaponPoint);
                newWeapon.Setup(weapon.Data);
                newWeapon.SetRangeWeaponRaycastPosition(_fpsCamera.transform);
                newWeapon.Hide();
                _weapons.Add(weapon.Type, newWeapon);
                _weaponOrder.Add(weapon.Type);
            }
            
            ChangeWeaponByIndex(0);
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                CastWeaponAction(WeaponActionType.ATTACK);
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                CastWeaponAction(WeaponActionType.ALT);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                CastWeaponAction(WeaponActionType.RELOAD);
            }

            if(Input.mouseScrollDelta.y<0f)
                ChangeWeaponByDirection(WeaponChangeDirection.PREV);
            
            if(Input.mouseScrollDelta.y>0f)
                ChangeWeaponByDirection(WeaponChangeDirection.NEXT);
        }

        private void CastWeaponAction(WeaponActionType type)
        {
            if (_currentWeapon == null) return;
            switch (type)
            {
                case WeaponActionType.ATTACK:
                {
                    _currentWeapon.Attack();
                    break;
                }
                case WeaponActionType.ALT:
                    _currentWeapon.AlternativeAction();
                    break;
                case WeaponActionType.RELOAD:
                    _currentWeapon.Reload();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        private void ChangeWeapon()
        {
            if (_currentWeapon != null)
            {
                if (_currentWeapon == _weapons[_weaponOrder[_currentIndex]]) return;

                _currentWeapon.Hide();
            }

            _currentWeapon = _weapons[_weaponOrder[_currentIndex]];
            _currentWeapon.Show();
        }

        private void ChangeWeaponByIndex(int index)
        {
            if (_weapons.Count <= index) return;
            _currentIndex = index;
            ChangeWeapon();
        }

        private void ChangeWeaponByDirection(WeaponChangeDirection direction)
        {
            switch (direction)
            {
                case WeaponChangeDirection.NEXT:
                {
                    if (_currentIndex + 1 >= _weapons.Count)
                        _currentIndex = 0;
                    else
                        _currentIndex++;
                    break;
                }
                case WeaponChangeDirection.PREV:
                {
                    if (_currentIndex - 1 < 0)
                        _currentIndex = _weapons.Count - 1;
                    else
                        _currentIndex--;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            ChangeWeapon();
        }

        private enum WeaponActionType
        {
            ATTACK,
            ALT,
            RELOAD
        }

        private enum WeaponChangeDirection
        {
            NEXT,
            PREV
        }

        public void OnDrawGizmos()
        {
            if (WeaponPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(WeaponPoint.position, .2f);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * 10);
        }
    }
}