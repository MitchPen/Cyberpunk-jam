using System;
using DG.Tweening;
using Main.Scripts.UI;
using Main.Scripts.UI.Main_Menu;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Main.Scripts.Main_Menu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Main-Menu")] [SerializeField] private MainMenuItems _mainMenuItems;
        [SerializeField] private FadeScreen _fadeScreen;
        [SerializeField] private AudioSource _ambientSound;
        [SerializeField] private Texture2D _cursor;
        private Vector2 _cursorHotspot;
      
        // Start is called before the first frame update
        void Awake()
        {
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _cursorHotspot = Vector2.zero;
            Cursor.SetCursor(_cursor,_cursorHotspot,CursorMode.Auto);
            _fadeScreen.Show(0);
            _mainMenuItems.HideMenu(0, () =>
            {
                _ambientSound.Play();
                _fadeScreen.Hide(3f,()=>
                {
                    _mainMenuItems.ShowMenu(2f, () =>
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    });
                });
            });
        }
    }
}
