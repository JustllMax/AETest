using AE._Project.Scripts.Core.Generics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AE._Project.Scripts.UI.Screens
{
    public class UIScreen : InGameMonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [FormerlySerializedAs("exitButton")] [SerializeField]
        private Button _exitButton;
        
        protected virtual void Awake()
        {
            _exitButton.onClick.AddListener(CloseGame);
        }

        private void CloseGame()
        {
            Application.Quit();
        }

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