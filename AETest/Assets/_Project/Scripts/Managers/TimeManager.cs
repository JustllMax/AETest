using System.Collections.Generic;
using AE.Core.Generics;
using AE.Interfaces;
using NaughtyAttributes;
using UnityEngine;

namespace AE.Managers
{
    public class TimeManager : MonoBehaviourSingleton<TimeManager>
    {
        private bool isTimePaused = false;
        
        private bool _canPause = true;
        public bool IsTimePaused => isTimePaused;
        public bool CanPause => pauseBlockers.Count <= 0;

        [Foldout("Debug"),ReadOnly] [SerializeField]
        List<GameObject> pauseBlockers = new List<GameObject>();

        public void AddPauseBlocker(GameObject pauseBlocker)
        {
            pauseBlockers.AddUnique(pauseBlocker);
        }

        public void RemovePauseBlocker(GameObject pauseBlocker)
        {
            pauseBlockers.RemoveElement(pauseBlocker);
        }
        
        public void PauseTime()
        {
            if(!CanPause) return;
            isTimePaused = true;
            Time.timeScale = 0;
        }

        public void ResumeTime()
        {
            isTimePaused = false;
            Time.timeScale = 1;
        }
        
    }
}
