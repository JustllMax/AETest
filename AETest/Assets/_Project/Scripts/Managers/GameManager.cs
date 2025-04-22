using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.UI.CursorManagement;

namespace AE._Project.Scripts.Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>, IWithSetUp
    {
        public void SetUp()
        {
            CursorManager.Instance?.SetCursorLockState(true);
            CursorManager.Instance?.SetCursorVisibility(false);
        }

        public void TearDown()
        {
            //noop
        }
    }
}