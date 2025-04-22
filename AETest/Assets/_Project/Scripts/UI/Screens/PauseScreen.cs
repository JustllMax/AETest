using AE._Project.Scripts.InputManagement;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Managers;
using AE._Project.Scripts.UI.CursorManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AE._Project.Scripts.UI.Screens
{
    public class PauseScreen : UIScreen, IAttachListeners
    {
        [FormerlySerializedAs("resumeButton")] [SerializeField]
        private Button _resumeButton;

        private bool _isPaused;
        private bool CanShowMenu => TimeManager.Instance ? TimeManager.Instance.CanPause : false;

        protected override void Awake()
        {
            base.Awake();
            _resumeButton.onClick.AddListener(ResumeGame);
        }

        public void AttachListeners()
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.AllTimeControls.Escape.performed += ShowPauseMenu;
            }
        }

        public void DetachListeners()
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.AllTimeControls.Escape.performed -= ShowPauseMenu;
            }
        }

        private void ShowPauseMenu(InputAction.CallbackContext context)
        {
            if (!CanShowMenu)
            {
                return;
            }

            ChangePauseState();
        }

        private void ChangePauseState()
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        private void PauseGame()
        {
            Show();
            TimeManager.Instance?.PauseTime();
            CursorManager.Instance?.SetCursorVisibility(true);
            CursorManager.Instance?.SetCursorLockState(false);
            AudioManager.Instance?.SetMusicLowPassFilterEnable(true);
            AudioManager.Instance?.SetVolume(50);
            _isPaused = true;
        }

        private void ResumeGame()
        {
            Hide();
            TimeManager.Instance?.ResumeTime();
            CursorManager.Instance?.SetCursorVisibility(false);
            CursorManager.Instance?.SetCursorLockState(true);
            AudioManager.Instance?.SetMusicLowPassFilterEnable(false);
            AudioManager.Instance?.SetVolume(80);

            _isPaused = false;
        }
    }
}