using AE.Core.Generics;
using AE.Interfaces;
using UnityEngine;

namespace AE.Managers
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
