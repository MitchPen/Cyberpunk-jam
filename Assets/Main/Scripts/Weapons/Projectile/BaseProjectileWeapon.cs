using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

namespace Main.Scripts.Weapons.Projectile
{
    public class BaseProjectileWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] protected Transform _shootPoint;
        [SerializeField] protected LayerMask _raycastIgnore;
        private ProjectileWeaponData _weaponData;
        private CancellationTokenSource _ctx;
        private int _shootingDelay;
        private int _reloadDelay;
        private bool _onCoolDown;
        private bool _onReload;
        private int _bulletCount;
        private Transform _raycastStart;

        public bool AbleToAttack => _onCoolDown == false && _onReload == false;
        public GameObject GetObject => this.gameObject;
        public WeaponType GetWeaponType => _weaponData.Type;

        public void SetRangeWeaponRaycastPosition(Transform raycastPos) =>
            _raycastStart = raycastPos;

        // ReSharper disable Unity.PerformanceAnalysis
        private void Shoot()
        {
            if (AbleToAttack == false) return;

            var direction = CalcBulletDirection();
            var bullet = LeanPool.Spawn(_weaponData.Projectile);
            bullet.transform.position = _shootPoint.position;
            bullet.Setup(direction, _weaponData.bulletSpeed, _weaponData.damage);
            bullet.Enable();
            _bulletCount--;
            ApplyCooldown();
        }

        protected Vector3 CalcBulletDirection()
        {
            if (_raycastStart == null) return _shootPoint.forward;
            var direction = _shootPoint.forward;

            if (!Physics.Raycast(_raycastStart.position, _raycastStart.forward, out var hit,
                Mathf.Infinity, _raycastIgnore)) return direction;
  
            if(Vector3.Distance(hit.point,_shootPoint.position)>0.5f)
                direction = hit.point - _shootPoint.position;
            return direction;
        }

        public void Attack()
        {
            if (_bulletCount > 0)
                Shoot();
            else
                Reload();
        }

        public async void Reload()
        {
            if (_onReload) return;
            _onReload = true;
            await UniTask.Delay(_reloadDelay, cancellationToken: _ctx.Token).SuppressCancellationThrow();
            if (!_ctx.IsCancellationRequested)
                _bulletCount = _weaponData.bulletCount;
            _onReload = false;
        }

        private async void ApplyCooldown()
        {
            if (_onCoolDown) return;
            _onCoolDown = true;
            await UniTask.Delay(_shootingDelay, cancellationToken: _ctx.Token).SuppressCancellationThrow();
            _onCoolDown = false;
        }

        public void Setup(WeaponData data)
        {
            _weaponData = (ProjectileWeaponData) data;
            _shootingDelay = Mathf.CeilToInt((1f / _weaponData.attackRate) * 1000);
            _reloadDelay = Mathf.CeilToInt((_weaponData.reloadTime) * 1000);
            _bulletCount = _weaponData.bulletCount;
            _onCoolDown = false;
            _onReload = false;
        }

        public void Show()
        {
            if (_ctx != null)
            {
                if (!_ctx.IsCancellationRequested)
                    _ctx.Cancel();
                _ctx.Dispose();
            }

            _ctx = new CancellationTokenSource();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_ctx != null)
            {
                if (!_ctx.IsCancellationRequested)
                    _ctx.Cancel();
                _ctx.Dispose();
            }

            gameObject.SetActive(false);
        }
        
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 10);
        }
    }

    [Serializable]
    public class ProjectileWeaponData : WeaponData
    {
        public BaseProjectile Projectile;
        public int bulletCount;
        public int bulletSpeed;
    }
}