using AE.Core.Generics;
using AE.InputManagement;
using AE.Interfaces;
using AE.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AE.UI.Screens
{
    public class PauseScreen : UIScreen, IAttachListeners
    {
        [SerializeField] private Button resumeButton;

        private bool isPaused = false;
        private bool CanShowMenu => TimeManager.Instance? TimeManager.Instance.CanPause : false;

        protected override void Awake()
        {
            base.Awake();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        public void AttachListeners()
        {
            if (InputManager.Instance)
                InputManager.Instance.AllTimeControls.Escape.performed += ShowPauseMenu;
        }

        private void ShowPauseMenu(InputAction.CallbackContext context)
        {
            if(!CanShowMenu) return;

            ChangePauseState();
        }

        private void ChangePauseState()
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        private void PauseGame()
        {
            Show();
            TimeManager.Instance?.PauseTime();
            CursorManager.Instance?.SetCursorVisibility(true);
            CursorManager.Instance?.SetCursorLockState(false);
            AudioManager.Instance?.SetMusicLowPassFilterEnable(true);
            AudioManager.Instance?.SetVolume(50);
            isPaused = true;
        }
        private void ResumeGame()
        {
            Hide();
            TimeManager.Instance?.ResumeTime();
            CursorManager.Instance?.SetCursorVisibility(false);
            CursorManager.Instance?.SetCursorLockState(true);
            AudioManager.Instance?.SetMusicLowPassFilterEnable(false);
            AudioManager.Instance?.SetVolume(80);

            isPaused = false;
        }
        
        public void DetachListeners()
        {
            if (InputManager.Instance)
                InputManager.Instance.AllTimeControls.Escape.performed -= ShowPauseMenu;
        }
    }
}
