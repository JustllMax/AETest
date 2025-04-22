using System;
using System.Collections.Generic;
using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Managers
{
    /// <summary>
    ///     Handles managing time scale
    /// </summary>
    public class TimeManager : MonoBehaviourSingleton<TimeManager>
    {
        [FormerlySerializedAs("pauseBlockers")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private List<GameObject> _pauseBlockers = new();
        
        public Action OnTimePaused;
        public Action OnTimeResumed;
        public bool IsTimePaused { get; private set; }

        public bool CanPause => _pauseBlockers.Count <= 0;

        public void AddPauseBlocker(GameObject pauseBlocker)
        {
            _pauseBlockers.AddUnique(pauseBlocker);
        }

        public void RemovePauseBlocker(GameObject pauseBlocker)
        {
            _pauseBlockers.RemoveElement(pauseBlocker);
        }

        public void PauseTime()
        {
            if (!CanPause)
            {
                return;
            }

            IsTimePaused = true;
            Time.timeScale = 0;
            OnTimePaused?.Invoke();
        }

        public void ResumeTime()
        {
            IsTimePaused = false;
            Time.timeScale = 1;
            OnTimeResumed?.Invoke();
        }
    }
}