using System;
using System.Threading;
using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace AE._Project.Scripts.Managers
{
    public class AudioManager : PersistentMonoSingleton<AudioManager>
    {
        [FormerlySerializedAs("MinDB")] [Header("Config")] [SerializeField]
        private float _minDB;

        [FormerlySerializedAs("MaxDB")] [SerializeField]
        private float _maxDB;

        [FormerlySerializedAs("delayBetweenClips")] [SerializeField]
        private float _delayBetweenClips = 0.5f;

        [FormerlySerializedAs("musicSounds")] [Header("Audio Clips")] [SerializeField]
        private Sound[] _musicSounds;

        [FormerlySerializedAs("sfxSounds")] [SerializeField]
        private Sound[] _sfxSounds;

        [FormerlySerializedAs("ambientSounds")] [SerializeField]
        private Sound[] _ambientSounds;

        [FormerlySerializedAs("AudioMixer")] [Header("References")] [SerializeField]
        private AudioMixer _audioMixer;

        [FormerlySerializedAs("musicSource")] [SerializeField]
        private AudioSource _musicSource;

        [FormerlySerializedAs("sfxSource")] [SerializeField]
        private AudioSource _sfxSource;

        [FormerlySerializedAs("ambientSource")] [SerializeField]
        private AudioSource _ambientSource;

        [FormerlySerializedAs("MusicAudioLowPassFilter")] [SerializeField]
        private AudioLowPassFilter _musicAudioLowPassFilter;

        [FormerlySerializedAs("AmbientAudioLowPassFilter")] [SerializeField]
        private AudioLowPassFilter _ambientAudioLowPassFilter;


        private CancellationTokenSource _cts;
        private AudioClip _lastPlayedClip;

        protected override void Awake()
        {
            base.Awake();

            _cts = new CancellationTokenSource();
            PlayRandomAudioLoop(_cts.Token).Forget();
        }

        public void PlayMusic(string clip)
        {
            var s = Array.Find(_musicSounds, x => x._name.Equals(clip, StringComparison.OrdinalIgnoreCase));

            if (s._name == default)
            {
                Debug.LogError($"Sound {clip} Not Found");
            }
            else
            {
                _musicSource.clip = s._clip;
                _musicSource.Play();
            }
        }


        public void PlayMusic(AudioClip clip)
        {
            StopMusic();

            _musicSource.clip = clip;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public void PlayAmbient(string clip)
        {
            var s = Array.Find(_ambientSounds, x => x._name.Equals(clip, StringComparison.OrdinalIgnoreCase));

            if (s._name == default)
            {
                Debug.LogError($"Sound {clip} Not Found");
            }
            else
            {
                _ambientSource.clip = s._clip;
                _ambientSource.Play();
            }
        }

        public void StopAmbient(string clip)
        {
            _ambientSource.Stop();
        }

        public void PlaySfx(string clip)
        {
            var s = Array.Find(_sfxSounds, x => x._name.Equals(clip, StringComparison.OrdinalIgnoreCase));

            if (s._name == default)
            {
                Debug.LogError($"Sound {clip} Not Found");
            }
            else
            {
                PlaySfx(s._clip);
            }
        }

        public void PlaySfx(AudioClip clip)
        {
            _sfxSource.PlayOneShot(clip);
        }


        public void PlaySfxAtSource(AudioClip clip, AudioSource source, float pitch = 1f)
        {
            source.Stop();
            source.volume = _sfxSource.volume;
            source.pitch = pitch;
            source.PlayOneShot(clip);
        }

        /// <summary>
        ///     Method tries to play SFX, but if its already playing given clip, it won't repeat
        /// </summary>
        public void PlaySFXAtSourceOnce(AudioClip clip, AudioSource source)
        {
            source.volume = _sfxSource.volume;
            if (source.clip != clip)
            {
                source.clip = clip;
                source.PlayOneShot(clip);
            }
            else if (source.clip == clip && source.isPlaying == false)
            {
                source.Play();
            }
        }

        public bool IsMusicPlayingClip(string clip)
        {
            var s = Array.Find(_musicSounds, x => x._name.Equals(clip, StringComparison.OrdinalIgnoreCase));
            if (s._name == default)
            {
                Debug.Log("Sound Not Found");
                return false;
            }

            return _musicSource.isPlaying && _musicSource.clip == s._clip;
        }


        /// <summary>
        ///     Sets main audio volume
        /// </summary>
        /// <remarks>Value passed down should be in range of 0 to 100, as in percentages</remarks>
        public void SetVolume(float volume)
        {
            volume = volume.Remap(0, 100, _minDB, _maxDB);

            _audioMixer?.SetFloat("Master", volume);
        }

        public void SetMusicLowPassFilterEnable(bool enable)
        {
            _musicAudioLowPassFilter.enabled = enable;
            _ambientAudioLowPassFilter.enabled = enable;
        }

        public void SetMusicPitch(float pitch)
        {
            _musicSource.pitch = pitch;
        }


        private async UniTaskVoid PlayRandomAudioLoop(CancellationToken token)
        {
            // Check if we have enough clips
            if (_musicSounds == null || _musicSounds.Length == 0)
            {
                Debug.LogWarning("No audio clips assigned to RandomAudioPlayer");
                return;
            }

            // If only one clip is available, just play it repeatedly
            if (_musicSounds.Length == 1)
            {
                while (!token.IsCancellationRequested)
                {
                    PlayMusic(_musicSounds[0]._clip);
                    await UniTask.WaitForSeconds(_musicSource.clip.length + _delayBetweenClips,
                        cancellationToken: token);
                }

                return;
            }

            // Loop forever until cancelled
            while (!token.IsCancellationRequested)
            {
                // Get a random clip different from the last one
                var randomClip = GetDifferentRandomClip();

                // Play the clip
                PlayMusic(randomClip);

                // Wait for clip to finish plus delay
                var waitTime = _musicSource.clip.length + _delayBetweenClips;
                try
                {
                    await UniTask.WaitForSeconds(waitTime, true, cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    Debug.LogWarning("Music stopped");
                    break;
                }
            }
        }

        private AudioClip GetDifferentRandomClip()
        {
            // Special case: if we have 0 or 1 clip, just return it
            if (_musicSounds.Length <= 1)
            {
                return _musicSounds.Length > 0 ? _musicSounds[0]._clip : null;
            }

            // Keep picking random clips until we get one different from the last played
            AudioClip newClip;
            do
            {
                var randomIndex = Random.Range(0, _musicSounds.Length);
                newClip = _musicSounds[randomIndex]._clip;
            } while (newClip == _lastPlayedClip);

            // Remember this clip for next time
            _lastPlayedClip = newClip;
            return newClip;
        }

        public void StopAudioLoop()
        {
            if (_cts == null)
            {
                return;
            }

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            StopAudioLoop();
        }

        [Serializable]
        internal struct Sound
        {
            [FormerlySerializedAs("name")] public string _name;
            [FormerlySerializedAs("clip")] public AudioClip _clip;
        }
    }
}