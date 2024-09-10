using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts.UI.Main_Menu
{
    public class MainMenuItems : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _logo;
        [SerializeField] private Button _start;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _exit;
        [SerializeField] private AudioSource _clickSound;

        public void ShowMenu(float speed, Action OnComplete)
        {
            _logo.DOFade(speed, speed).OnComplete(() =>
            {
                _canvasGroup.DOFade(speed, speed).OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
            });
        }

        public void HideMenu(float speed,Action OnComplete)
        {
            _logo.DOFade(speed, speed).OnComplete(() =>
            {
                _canvasGroup.DOFade(speed, speed).OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
            });
        }
    }
}
