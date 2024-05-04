using System;
using DG.Tweening;
using UnityEngine;

namespace Main.Scripts.Weapons.Melee
{
    public class MeleeWeaponAnimator : MonoBehaviour
    {
        [SerializeField] private float _showDuration;
        [SerializeField] private Ease _attackEase;
        private Vector3 _defaultPos;
        private Transform _startPosition;
        
        public void Init(Transform startPosition)
        {
            _defaultPos = transform.localPosition;
           
            _startPosition = startPosition;
        }
        public void PlayMeeleAttackAnimation(Action onComplete)
        {
            
        }
        
        public void PlayShowAnimation(Action OnComplete = null)
        {
            transform.DOKill(true);
            transform.DOLocalMove(_startPosition.localPosition, 0f);
            transform.DOLocalRotate(_startPosition.localRotation.eulerAngles, 0f);
            transform.DOLocalMove(_defaultPos, _showDuration);
            transform.DOLocalRotate(Vector3.zero, _showDuration)
                .OnComplete(() => { OnComplete?.Invoke(); });
        }

        public void DisableAllAnimation(Action onComplete)
        {
            transform.DOKill(true);
            onComplete?.Invoke();
        }
    }
}
