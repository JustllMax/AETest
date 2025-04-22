using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Managers;
using AE._Project.Scripts.Puzzles.SwordCoffinPuzzle;
using AE._Project.Scripts.UI.CursorManagement;

namespace AE._Project.Scripts.UI.Screens
{
    public class EndScreen : UIScreen, IAttachListeners
    {
        public void AttachListeners()
        {
            ScDarkScreen.OnDarkScreenComplete += OnAnimEnded;
        }

        public void DetachListeners()
        {
            ScDarkScreen.OnDarkScreenComplete -= OnAnimEnded;
        }

        private void OnAnimEnded()
        {
            Show();
            CursorManager.Instance.SetCursorVisibility(true);
            CursorManager.Instance.SetCursorLockState(false);
            AudioManager.Instance.StopAudioLoop();
        }
    }
}