using AE.Core.Generics;
using UnityEngine;

namespace AE.Managers
{

    /// <summary>
    /// Handles management of the cursor
    /// </summary>
    public class CursorManager : PersistentMonoSingleton<CursorManager>
    {

        public void SetCursorVisibility(bool isVisible)
        {
            Cursor.visible = isVisible;
        }

        public void SetCursorLockState(bool isLocked)
        {
            if (isLocked) Cursor.lockState = CursorLockMode.Locked;

            else Cursor.lockState = CursorLockMode.Confined;
        }
        
    }
}