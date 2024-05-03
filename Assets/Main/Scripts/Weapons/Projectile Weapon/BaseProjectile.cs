using System;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Rendering;

namespace Main.Scripts.Weapons.Projectile
{
    public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private ParticleSystem _collisionEffect;
        private float _damage;
        private float _speed;
        private Vector3 _direction;
        public void Setup(Vector3 direction, float speed, float damage)
        {
            gameObject.SetActive(false);
            _direction = direction;
            _speed = speed;
            _damage = damage;
        }

        public void OnCollisionEnter(Collision other)
        {
            Disable();
            PlayCollisionEffect(other);
            LeanPool.Despawn(this);
        }

        public async void PlayCollisionEffect(Collision other)
        {
            var particles = LeanPool.Spawn(_collisionEffect);
            particles.transform.position = other.contacts[0].point;
            particles.transform.forward = other.contacts[0].normal;
            particles.Play();
            await UniTask.Delay(1000);
            LeanPool.Despawn(particles);
        }

        public async void Enable(Transform startPosition)
        {
            await UniTask.WaitForEndOfFrame();
            gameObject.SetActive(true);
            _collider.enabled = true;
            transform.position =startPosition.position ;
            _rigidbody.velocity = _direction.normalized * _speed;
        }
        

        public void Disable()
        {
            _collider.enabled = false;
            _rigidbody.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}