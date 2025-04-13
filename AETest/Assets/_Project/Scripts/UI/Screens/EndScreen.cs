using AE.Interfaces;
using AE.Managers;
using AE.Puzzles.SwordCoffinPuzzle;
using UnityEngine;

namespace AE.UI.Screens
{
    public class EndScreen : UIScreen, IAttachListeners
    {
        public void AttachListeners()
        {
            SCDarkScreen.OnDarkScreenComplete += OnAnimEnded;
        }

        private void OnAnimEnded()
        {
            Show();
            CursorManager.Instance.SetCursorVisibility(true);
            CursorManager.Instance.SetCursorLockState(false);
            AudioManager.Instance.StopAudioLoop();
        }

        public void DetachListeners()
        {
            SCDarkScreen.OnDarkScreenComplete -= OnAnimEnded;

        }
    }
}
