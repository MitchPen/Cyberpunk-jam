using System;
using Lean.Pool;
using UnityEngine;

namespace Main.Scripts.Weapons.Projectile
{
    public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        private float _damage;
        private float _speed;
        private Vector3 _direction;

        public void Setup(Vector3 direction, float speed, float damage)
        {
            _direction = direction;
            _speed = speed;
            _damage = damage;
        }

        public void OnCollisionEnter(Collision other)
        {
            Disable();
            LeanPool.Despawn(this);
        }

        public void Enable()
        {
            _collider.enabled = true;
            _rigidbody.velocity = _direction.normalized * _speed;
        }

        public void Disable()
        {
            _collider.enabled = false;
            _rigidbody.velocity = Vector3.zero;
        }
    }
}