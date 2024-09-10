using System;
using DG.Tweening;
using UnityEngine;

namespace Main.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _wrapper;
 
        public void Show(float duration, Action OnComplete = null)
        {
            _wrapper.DOFade(1, duration)
                .OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
        }
        
        
        public void Hide(float duration, Action OnComplete = null)
        {
            _wrapper.DOFade(0, duration)
                .OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
        }
    }
}
