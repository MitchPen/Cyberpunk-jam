using System.Collections.Generic;
using DG.Tweening;
using Main.Scripts.InputSystem;
using Main.Scripts.Player.Weapons.Projectile_Weapon;
using UnityEngine;

namespace Main.Scripts.Player.Weapons
{
    public class PlayerWeaponSystem : MonoBehaviour
    {
        [SerializeField] private Camera _fpsCamera;
        [SerializeField] private Transform _handPoint;
        [SerializeField] private Transform _weaponShowPoint;
        [SerializeField] private WeaponShowAnimationComponent _weaponAnimator;
        private List<IWeapon> _weaponOrder = new List<IWeapon>();
        private IWeapon _currentWeapon;
        private int _currentIndex;
        private Input.InputSystem _inputSystem;

        public void Initialize(ProjectileWeaponConfig weapons, Input.InputSystem inputSystem)
        {
            _weaponAnimator.Setup(_weaponShowPoint, _handPoint);
            _inputSystem = inputSystem;
            SetupWeapons(weapons.WeaponSetups);
            ChangeWeaponByIndex(0);
            _weaponAnimator.AnimateHand();
        }

        public void Enable()
        {
            _inputSystem.SubscribeOnInputEvent(KeyEvents.SHOOT, Shoot);
            _inputSystem.SubscribeOnInputEvent(KeyEvents.M_SCROLL_UP,
                () => ChangeWeaponByDirection(WeaponChangeDirection.PREV));
            _inputSystem.SubscribeOnInputEvent(KeyEvents.M_SCROLL_DOWN,
                () => ChangeWeaponByDirection(WeaponChangeDirection.NEXT));
            _inputSystem.SubscribeOnInputEvent(KeyEvents.PICK_FIRST_WEAPON, () => ChangeWeaponByIndex(0));
            _inputSystem.SubscribeOnInputEvent(KeyEvents.PICK_SECOND_WEAPON, () => ChangeWeaponByIndex(1));
            _inputSystem.SubscribeOnInputEvent(KeyEvents.PICK_THIRD_WEAPON, () => ChangeWeaponByIndex(2));
        }

        public void Disable()
        {
            _inputSystem.UnsubscribeFromInputEvent(KeyEvents.SHOOT, Shoot);
            _inputSystem.ClearEventHandlerOn(KeyEvents.M_SCROLL_UP);
            _inputSystem.ClearEventHandlerOn(KeyEvents.M_SCROLL_DOWN);
            _inputSystem.ClearEventHandlerOn(KeyEvents.PICK_FIRST_WEAPON);
            _inputSystem.ClearEventHandlerOn(KeyEvents.PICK_SECOND_WEAPON);
            _inputSystem.ClearEventHandlerOn(KeyEvents.PICK_THIRD_WEAPON);
        }

        private void SetupWeapons(IEnumerable<ProjectileWeaponSetup> weapons)
        {
            foreach (var item in weapons)
            {
                var newWeapon = Instantiate(item.Weapon.GetObject, _handPoint)
                    .GetComponent<IWeapon>();
                newWeapon.GetObject.transform.localPosition = Vector3.zero;
                newWeapon.Setup(item.weaponData, _fpsCamera.transform);
                newWeapon.Hide();
                _weaponOrder.Add(newWeapon);
            }
        }

        private void Shoot()
        {
            if (_currentWeapon == null) return;
            _currentWeapon.Shoot();
        }

        private void ChangeWeapon()
        {
            if (_currentWeapon != null)
            {
                if (_currentWeapon == _weaponOrder[_currentIndex]) return;

                _currentWeapon.Hide();
            }

            _currentWeapon = _weaponOrder[_currentIndex];
            _currentWeapon.Enable = false;
            _currentWeapon.Show();
            _weaponAnimator.PlayShowAnimation(_currentWeapon.GetObject.transform,
                () => { _currentWeapon.Enable = true; });
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