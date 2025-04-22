using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Components
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioSourceComponent : InGameMonoBehaviour, IAttachListeners
    {
        [FormerlySerializedAs("audioSource")] [SerializeField] private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void AttachListeners()
        {
            if (TimeManager.Instance)
            {
                TimeManager.Instance.OnTimePaused += PauseAudio;
                TimeManager.Instance.OnTimeResumed += ResumeAudio;
            }
        }

        public void DetachListeners()
        {
            if (TimeManager.Instance)
            {
                TimeManager.Instance.OnTimePaused -= PauseAudio;
                TimeManager.Instance.OnTimeResumed -= ResumeAudio;
            }
        }

        private void PauseAudio()
        {
            _audioSource.Pause();
        }

        private void ResumeAudio()
        {
            _audioSource.UnPause();
        }
    }
}