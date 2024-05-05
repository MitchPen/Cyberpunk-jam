using System;
using System.Threading;
using Content.Audio;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace Main.Scripts.Player.Weapons.Projectile_Weapon
{
    public class ProjectileWeapon :MonoBehaviour, IWeapon
    {
        [SerializeField] protected Transform _shootingPoint;
        [SerializeField] protected LayerMask _raycastIgnore;
        [SerializeField] protected ParticleSystem _shootParticles;
        [SerializeField] private RecoilAnimationComponent _recoilAnimator;
        private ProjectileWeaponData _weaponData;
        private CancellationTokenSource _ctx;
        private Transform _raycastPoint;
        private bool _onCoolDown;
        private int _shootingDelay;
        private bool AbleToShoot => Enable && _onCoolDown == false;
        public GameObject GetObject => this.gameObject;
        public bool Enable { get; set; }

        public void Shoot()
        {
            if (AbleToShoot)
            {
                var bullet = LeanPool.Spawn(_weaponData.projectilePrefab);
                bullet.Setup(CalcBulletDirection(), _weaponData.bulletSpeed, _weaponData.damage);
                _recoilAnimator.Play(true);
                SoundController.Instance?.PlayClip(_weaponData.shotSound, customVolume: 1f);
                bullet.Enable(_shootingPoint);
                ApplyCooldown();
            }
        }
        
        protected virtual Vector3 CalcBulletDirection()
        {
            if (_raycastPoint == null) return _shootingPoint.forward;
            var direction = _shootingPoint.forward;

            if (!Physics.Raycast(_raycastPoint.position, _raycastPoint.forward, out var hit,
                Mathf.Infinity, _raycastIgnore)) return direction;
  
            if(Vector3.Distance(hit.point,_shootingPoint.position)>0.1f)
                direction = hit.point - _shootingPoint.position;
            return direction;
        }
        
        private async void ApplyCooldown()
        {
            if (_onCoolDown) return;
            _onCoolDown = true;
            await UniTask.Delay(_shootingDelay, cancellationToken: _ctx.Token).SuppressCancellationThrow();
            _onCoolDown = false;
        }

        public void Setup(WeaponData data, Transform point)
        {
            _weaponData = (ProjectileWeaponData)data;
            _recoilAnimator.Setup(transform.localPosition);
            _shootingDelay = Mathf.CeilToInt((1f / _weaponData.attackRate) * 1000);
            _onCoolDown = false;
            _raycastPoint = point;
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

            Enable = false;
            transform.DOKill(false);
            gameObject.SetActive(false);
        }
    }
    
    [Serializable]
    public class ProjectileWeaponData : WeaponData
    {
        public int bulletSpeed;
        public BaseProjectile projectilePrefab;
        public AudioClip shotSound;
    }
}
