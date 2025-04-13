using System;
using AE.Core.Generics;
using AE.Interfaces;
using AE.Managers;
using UnityEngine;

namespace AE
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioSourceComponent : InGameMonoBehaviour,IAttachListeners
    {
        [SerializeField]private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void AttachListeners()
        {
            if (TimeManager.Instance)
            {
                TimeManager.Instance.OnTimePaused += PauseAudio;
                TimeManager.Instance.OnTimeResumed += ResumeAudio;
            }

        }
        private void PauseAudio()
        {
            audioSource.Pause();
        }

        private void ResumeAudio()
        {
            audioSource.UnPause();
        }
        public void DetachListeners()
        {
            if (TimeManager.Instance)
            {
                TimeManager.Instance.OnTimePaused -= PauseAudio;
                TimeManager.Instance.OnTimeResumed -= ResumeAudio;
            }

        }
    }
}
