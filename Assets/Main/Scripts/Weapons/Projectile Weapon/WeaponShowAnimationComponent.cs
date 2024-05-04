using System;
using DG.Tweening;
using UnityEngine;

namespace Main.Scripts.Weapons.Projectile_Weapon
{
    public class WeaponShowAnimationComponent : MonoBehaviour
    {
        [SerializeField] private Ease _ease;
        [SerializeField] [Range(0f,1f)]private float _duration;
        private Transform _startPoint;

        public void Setup(Transform startPoint)
        {
            _startPoint = startPoint;
        }
        
        public void PlayShowAnimation(Transform weapon, Action OnComplete = null)
        {
            weapon.DOKill(true);
            weapon.DOLocalMove(_startPoint.localPosition, 0f);
            weapon.DOLocalRotate(_startPoint.localRotation.eulerAngles, 0f);
            weapon.DOLocalMove(Vector3.zero, _duration);
            weapon.DOLocalRotate(Vector3.zero, _duration)
                .OnComplete(() => { OnComplete?.Invoke(); });
        }
    }
}
