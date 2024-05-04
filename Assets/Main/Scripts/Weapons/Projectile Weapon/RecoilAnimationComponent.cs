using DG.Tweening;
using UnityEngine;

namespace Main.Scripts.Weapons.Projectile_Weapon
{
    public class RecoilAnimationComponent : MonoBehaviour
    {
        [SerializeField] private Ease _recoilEase;
        [SerializeField] private Ease _returnEase;
        [SerializeField][Range(0f, 1f)] private float _recoilBalanceRation;
        [SerializeField][Range(0f, 0.5f)] private float _recoilImpact;
        [SerializeField] private float _recoilDuration;
        private Vector3 _defaultPos;
        private float _recoilTiming;
        private float _returnTiming;
        
        public void Setup(Vector3 defaultPos)
        {
            _defaultPos = Vector3.zero;
            _recoilTiming = Mathf.Lerp(0, _recoilDuration, _recoilBalanceRation);
            _returnTiming = _recoilDuration - _recoilTiming;
        }
        
        public void Play(bool breakAnimation)
        {
            if (breakAnimation)
                transform.DOKill(false);
            transform.DOLocalMoveZ(_defaultPos.z - _recoilImpact, _recoilTiming)
                .SetEase(_recoilEase)
                .OnComplete(() =>
                {
                    transform.DOLocalMoveZ(_defaultPos.z , _returnTiming)
                        .SetEase(_returnEase);
                });
        }
    }
}
