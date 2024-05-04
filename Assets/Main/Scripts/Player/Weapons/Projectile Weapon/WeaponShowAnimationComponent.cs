using System;
using DG.Tweening;
using UnityEngine;

namespace Main.Scripts.Player.Weapons.Projectile_Weapon
{
    public class WeaponShowAnimationComponent : MonoBehaviour
    {
        [SerializeField] private Ease _ease;
        [SerializeField] [Range(0f,1f)] private float _duration;
        [SerializeField] [Range(0f,0.1f)] private float _handShiftValue;
        [SerializeField] [Range(0f,3f)] private float _handShiftDuration;
        private Transform _startPoint;
        private Transform _hand;
        private float _handDefaultYPos;

        public void Setup(Transform startPoint, Transform hand)
        {
            _startPoint = startPoint;
            _hand = hand;
            _handDefaultYPos = _hand.localPosition.y;
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

        public void AnimateHand()
        {
            _hand.DOLocalMoveY(_handDefaultYPos + _handShiftValue, _handShiftDuration/2)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    _hand.DOLocalMoveY(_handDefaultYPos -  _handShiftValue, _handShiftDuration)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() =>
                        {
                            _hand.DOLocalMoveY(_handDefaultYPos, _handShiftDuration/2)
                                .SetEase(Ease.InSine)
                                .OnComplete(() =>
                                {
                                    AnimateHand();
                                });
                        });
                });
        }
    }
}
