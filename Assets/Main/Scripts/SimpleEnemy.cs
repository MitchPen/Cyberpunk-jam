using System;
using System.Threading;
using Content.Audio;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using Main.Scripts.Player.Weapons.Projectile_Weapon;
using UnityEngine;
using Random = System.Random;

namespace Main.Scripts
{
    public class Enemy : MonoBehaviour
    {
        private const string ProjectileTag = "PlayerProjectile";

        [SerializeField] private ParticleSystem _deathParticles;
      //  [SerializeField] private ProjectileWeaponData _projectileData;
        [SerializeField] private LineRenderer _laser;

        private CancellationTokenSource _cts;
        private float _hp = 100;

        private Player.Player _player;
        private float _shootingTimer;
        private float _shootCooldown;
        
        private void Awake()
        {
            _player = FindObjectOfType<Player.Player>();
            _shootCooldown = 3f + UnityEngine.Random.Range(0, 7f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BaseProjectile component))
            {
                _hp -= component.Damage;
                if (_hp<=0)
                {
                    OnDeath();
                }
            }
        }
        
        private void Update()
        {
            _shootingTimer += Time.deltaTime;
            if (_shootingTimer>=_shootCooldown)
            {
                Shoot();
                _shootingTimer = 0f;
            }
            transform.LookAt(_player.transform.position);
        }

        private void OnDeath()
        {
            var p =Instantiate(_deathParticles, transform.position, Quaternion.identity);
            p.Play();
            gameObject.SetActive(false);
        }
        
        private async void Shoot()
        {
            _cts = new CancellationTokenSource();

            Physics.Raycast(_laser.transform.position, _laser.transform.forward, out var hit,
                Mathf.Infinity);

            Vector3 targetPos = Vector3.zero;
            if (hit.transform!=null)
            {
                targetPos = hit.point;
            }
            else
            {
                targetPos = _player.transform.position + Vector3.up/2;
            }
            
            _laser.SetPosition(0, _laser.transform.position);
            _laser.SetPosition(1, targetPos);
            _laser.enabled = true;

            var timeSpan = .75f;
            while (timeSpan>0f)
            {
                _laser.SetPosition(0, _laser.transform.position);
                _laser.SetPosition(1, targetPos);
                timeSpan -= Time.deltaTime;
                await UniTask.Yield(cancellationToken: _cts.Token);
            }
            
            _laser.enabled = false;
            //  var bullet = LeanPool.Spawn(_projectileData.projectilePrefab);
            // bullet.Setup(transform.forward, _projectileData.bulletSpeed, _projectileData.damage);
            //  bullet.Enable(transform);
        }

        private void OnDisable()
        {
            _cts?.Cancel();
        }
    }
}
