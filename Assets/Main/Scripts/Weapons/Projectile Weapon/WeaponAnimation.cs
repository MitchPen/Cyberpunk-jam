using System;
using DG.Tweening;
using UnityEngine;

namespace Main.Scripts.Weapons.Projectile_Weapon
{
    public class WeaponAnimation : MonoBehaviour
    {
        [SerializeField] private Ease _recoilEase;
        [SerializeField] private Ease _returnEase;
        [SerializeField][Range(0f, 1f)] private float _animationBalanceRation;
        [SerializeField][Range(0f, 1f)] private float _recoilValue;
        [SerializeField] private float _animationDuration;
        private Vector3 _defaultPos;
        private float _recoilTiming;
        private float _returnTiming;

        public void Init()
        {
            _defaultPos = transform.localPosition;
            _recoilTiming = Mathf.Lerp(0, _animationDuration, _animationBalanceRation);
            _returnTiming = _animationDuration - _recoilTiming;
        }

        public void PlayShotAnimation(bool breakPrevAnimation)
        {
            if (breakPrevAnimation)
                transform.DOKill(true);
            transform.DOLocalMoveZ(_defaultPos.z - _recoilValue, _recoilTiming)
                .SetEase(_recoilEase)
                .OnComplete(() =>
                {
                    transform.DOLocalMoveZ(_defaultPos.z , _returnTiming)
                        .SetEase(_returnEase);
                });
        }
        
        public void PlayReloadAnimation()
        {
            
            
        }

        public void PlayShowAnimation(Action OnComplete = null)
        {
            
        }

        public void PlayHideAnimation(Action onComplete = null)
        {
            transform.DOKill(false);
        }
    }
}
