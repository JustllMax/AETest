using System;
using AE.Core.Generics;
using AE.Extensions;
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

        [Header("Config")]
        [SerializeField] private float MinDB;
        [SerializeField] private float MaxDB;

        [Header("Audio Clips")]
        [SerializeField]
        private Sound[] musicSounds, sfxSounds, ambientSounds;

        [Header("References")]
        [SerializeField] private AudioMixer AudioMixer;
        [SerializeField] private AudioSource musicSource, sfxSource, ambientSource;
        [SerializeField] private AudioLowPassFilter MusicAudioLowPassFilter;
        [SerializeField] private AudioLowPassFilter AmbientAudioLowPassFilter;
        

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
            source.volume = sfxSource.volume;
            source.pitch = pitch;
            source.PlayOneShot(clip);
        }
        
        /// <summary>
        /// Method tries to play SFX, but if its already playing given clip, it won't repeat
        /// </summary>
        public void PlaySFXAtSourceOnce(AudioClip clip, AudioSource source)
        {
            if (source.clip != clip)
            {
                source.clip = clip;
                source.volume = sfxSource.volume;
                source.PlayOneShot(clip);
            }
            else if (source.clip == clip && source.isPlaying == false)
            {
                source.volume = sfxSource.volume;
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


        public void SetVolume(float volume)
        {
            volume.Remap(0,100, MinDB, MaxDB);
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
    }
}
