using System;
using AE.Core.Generics;
using AE.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AE.UI
{
    public class UIScreen : InGameMonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button exitButton;
        
        private bool canPause = false;

        protected virtual void Awake()
        {
            exitButton.onClick.AddListener(CloseGame);
        }

        private void CloseGame() => Application.Quit();
        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false; 
        }
        
    }
}
