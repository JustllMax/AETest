using System;
using System.Threading;
using AE.Core.Generics;
using AE.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace AE.Managers
{
    public class AudioManager : PersistentMonoSingleton<AudioManager>
    {

        [Serializable]
        internal struct Sound
        {
            public string name;
            public AudioClip clip;
        }

        [Header("Config")] [SerializeField] private float MinDB;
        [SerializeField] private float MaxDB;
        [SerializeField] private float delayBetweenClips = 0.5f;

        [Header("Audio Clips")]
        [SerializeField] private Sound[] musicSounds;
        [SerializeField] private Sound[] sfxSounds;
        [SerializeField] private Sound[] ambientSounds;

        [Header("References")] [SerializeField]
        private AudioMixer AudioMixer;

        [SerializeField] private AudioSource musicSource, sfxSource, ambientSource;
        [SerializeField] private AudioLowPassFilter MusicAudioLowPassFilter;
        [SerializeField] private AudioLowPassFilter AmbientAudioLowPassFilter;



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
            Sound s = Array.Find(musicSounds, x => x.name.Equals(clip, StringComparison.OrdinalIgnoreCase));

            if (s.name == default)
            {
                Debug.LogError($"Sound {clip} Not Found");
            }
            else
            {
                musicSource.clip = s.clip;
                musicSource.Play();
            }
        }


        public void PlayMusic(AudioClip clip)
        {
            StopMusic();

            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlayAmbient(string clip)
        {
            Sound s = Array.Find(ambientSounds, x => x.name.Equals(clip, StringComparison.OrdinalIgnoreCase));

            if (s.name == default)
            {
                Debug.LogError($"Sound {clip} Not Found");
            }
            else
            {
                ambientSource.clip = s.clip;
                ambientSource.Play();
            }
        }

        public void StopAmbient(string clip)
        {
            ambientSource.Stop();
        }

        public void PlaySFX(string clip)
        {
            Sound s = Array.Find(sfxSounds, x => x.name.Equals(clip, StringComparison.OrdinalIgnoreCase));

            if (s.name == default)
            {
                Debug.LogError($"Sound {clip} Not Found");
            }
            else
            {
                PlaySFX(s.clip);
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }


        public void PlaySFXAtSource(AudioClip clip, AudioSource source, float pitch = 1f)
        {
            source.Stop();
            source.volume = sfxSource.volume;
            source.pitch = pitch;
            source.PlayOneShot(clip);
        }

        /// <summary>
        /// Method tries to play SFX, but if its already playing given clip, it won't repeat
        /// </summary>
        public void PlaySFXAtSourceOnce(AudioClip clip, AudioSource source)
        {
            source.volume = sfxSource.volume;
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
            Sound s = Array.Find(musicSounds, x => x.name.Equals(clip, StringComparison.OrdinalIgnoreCase));
            if (s.name == default)
            {
                Debug.Log("Sound Not Found");
                return false;
            }

            return musicSource.isPlaying && musicSource.clip == s.clip;
        }


        /// <summary>
        /// Sets main audio volume
        /// </summary>
        /// <remarks>Value passed down should be in range of 0 to 100, as in percentages</remarks>
        public void SetVolume(float volume)
        {
            volume = volume.Remap(0, 100, MinDB, MaxDB);

            AudioMixer?.SetFloat("Master", volume);
        }

        public void SetMusicLowPassFilterEnable(bool enable)
        {
            MusicAudioLowPassFilter.enabled = enable;
            AmbientAudioLowPassFilter.enabled = enable;
        }

        public void SetMusicPitch(float pitch)
        {
            musicSource.pitch = pitch;
        }


        private async UniTaskVoid PlayRandomAudioLoop(CancellationToken token)
        {
            // Check if we have enough clips
            if (musicSounds == null || musicSounds.Length == 0)
            {
                Debug.LogWarning("No audio clips assigned to RandomAudioPlayer");
                return;
            }

            // If only one clip is available, just play it repeatedly
            if (musicSounds.Length == 1)
            {
                while (!token.IsCancellationRequested)
                {
                    PlayMusic(musicSounds[0].clip);
                    await UniTask.WaitForSeconds(musicSource.clip.length + delayBetweenClips, cancellationToken: token);
                }

                return;
            }

            // Loop forever until cancelled
            while (!token.IsCancellationRequested)
            {
                // Get a random clip different from the last one
                AudioClip randomClip = GetDifferentRandomClip();

                // Play the clip
                PlayMusic(randomClip);

                // Wait for clip to finish plus delay
                float waitTime = musicSource.clip.length + delayBetweenClips;
                try
                {
                    await UniTask.WaitForSeconds(waitTime, ignoreTimeScale:true, cancellationToken: token);
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
            if (musicSounds.Length <= 1)
                return musicSounds.Length > 0 ? musicSounds[0].clip : null;

            // Keep picking random clips until we get one different from the last played
            AudioClip newClip;
            do
            {
                int randomIndex = UnityEngine.Random.Range(0, musicSounds.Length);
                newClip = musicSounds[randomIndex].clip;
            } while (newClip == _lastPlayedClip);

            // Remember this clip for next time
            _lastPlayedClip = newClip;
            return newClip;
        }

        public void StopAudioLoop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            StopAudioLoop();
        }
    }
}
