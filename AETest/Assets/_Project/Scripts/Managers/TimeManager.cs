using System;
using System.Collections.Generic;
using AE.Core.Generics;
using AE.Interfaces;
using NaughtyAttributes;
using UnityEngine;

namespace AE.Managers
{
    /// <summary>
    /// Handles managing time scale
    /// </summary>
    public class TimeManager : MonoBehaviourSingleton<TimeManager>
    {
                
        [Foldout("Debug"),ReadOnly] [SerializeField]
        List<GameObject> pauseBlockers = new List<GameObject>();
        
        private bool isTimePaused = false;
        
        private bool _canPause = true;
        public bool IsTimePaused => isTimePaused;
        public bool CanPause => pauseBlockers.Count <= 0;
        


        public Action OnTimePaused;
        public Action OnTimeResumed;
        
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
            OnTimePaused?.Invoke();
        }

        public void ResumeTime()
        {
            isTimePaused = false;
            Time.timeScale = 1;
            OnTimeResumed?.Invoke();
        }
        
    }
}
