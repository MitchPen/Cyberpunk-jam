using System;
using System.Collections.Generic;
using Main.Scripts.Weapons.Projectile;
using UnityEngine;

namespace Main.Scripts.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Transform WeaponPoint;
        [SerializeField] private ProjectileWeaponConfig _testConfig;
        [SerializeField] private Camera _fpsCamera;
        [SerializeField] private LayerMask _raycastIgnore;
        private List<IWeapon> _weapons = new List<IWeapon>();
        private IWeapon _currentWeapon;
        private int _currentIndex;

        void Start()
        {
            var test = new List<(IWeapon weapon, WeaponData data)>();
            foreach (var w in _testConfig.WeaponSetups)
            {
                var wiii = (w.Weapon, w.Data);
                test.Add(wiii);
            }

            Initialize(test);
        }

        public void Initialize(IEnumerable<(IWeapon weapon, WeaponData data)> weapons)
        {
            foreach (var (weapon, data) in weapons)
            {
                var newWeapon = Instantiate(weapon.GetObject, WeaponPoint.position, Quaternion.identity)
                    .GetComponent<IWeapon>();
                newWeapon.Setup(data);
                if (newWeapon.GetWeaponType == WeaponType.Range)
                    newWeapon.SetRangeWeaponRaycastPosition(_fpsCamera.transform);
                newWeapon.Hide();
                _weapons.Add(newWeapon);
            }

            ChangeWeaponByIndex(0);
        }

        void Update()
        {
            //Test input
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeWeaponByDirection(WeaponChangeDirection.NEXT);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangeWeaponByDirection(WeaponChangeDirection.PREV);
            }
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
                if (_currentWeapon == _weapons[_currentIndex]) return;

                _currentWeapon.Hide();
            }

            _currentWeapon = _weapons[_currentIndex];
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