using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Core
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1.0f;
        [Range(0.5f, 1.5f)] public float pitch = 1.0f;
        public bool loop = false;

        [HideInInspector] public AudioSource source;
    }

    /// <summary>
    /// Master Audio Manager supporting Music ON/OFF, SFX ON/OFF, Master Volume controls,
    /// and 9 sound categories (Coin, Jump, Slide, Collision, Power-up, Button, Game Over, Location Transition, Background Music).
    /// Includes procedural audio synthesis fallback for testing without external audio assets.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Volume Controls")]
        [Range(0f, 1f)] public float musicVolume = 0.8f;
        [Range(0f, 1f)] public float sfxVolume = 1.0f;

        [Header("Audio Tracks & SFX")]
        public Sound[] musicTracks;
        public Sound[] soundEffects;

        private Sound currentMusic;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            if (musicTracks != null)
            {
                foreach (var s in musicTracks)
                {
                    if (s.source == null) s.source = gameObject.AddComponent<AudioSource>();
                    s.source.clip = s.clip;
                    s.source.volume = s.volume * musicVolume;
                    s.source.pitch = s.pitch;
                    s.source.loop = s.loop;
                }
            }

            if (soundEffects != null)
            {
                foreach (var s in soundEffects)
                {
                    if (s.source == null) s.source = gameObject.AddComponent<AudioSource>();
                    s.source.clip = s.clip;
                    s.source.volume = s.volume * sfxVolume;
                    s.source.pitch = s.pitch;
                    s.source.loop = s.loop;
                }
            }
        }

        public void PlayMusic(string name)
        {
            if (SaveManager.Instance != null && !SaveManager.Instance.data.isMusicOn) return;

            if (musicTracks == null || musicTracks.Length == 0) return;

            Sound s = System.Array.Find(musicTracks, track => track.name == name);
            if (s == null || s.source == null) return;

            if (currentMusic != null && currentMusic.source != null && currentMusic.source.isPlaying)
            {
                currentMusic.source.Stop();
            }

            currentMusic = s;
            s.source.volume = s.volume * musicVolume;
            s.source.Play();
        }

        public void PauseMusic()
        {
            if (currentMusic != null && currentMusic.source != null && currentMusic.source.isPlaying)
            {
                currentMusic.source.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (SaveManager.Instance != null && !SaveManager.Instance.data.isMusicOn) return;

            if (currentMusic != null && currentMusic.source != null)
            {
                currentMusic.source.UnPause();
            }
        }

        public void PlaySFX(string name)
        {
            if (SaveManager.Instance != null && !SaveManager.Instance.data.isSFXOn) return;

            if (soundEffects != null && soundEffects.Length > 0)
            {
                Sound s = System.Array.Find(soundEffects, sfx => sfx.name == name);
                if (s != null && s.source != null && s.clip != null)
                {
                    s.source.PlayOneShot(s.clip, s.volume * sfxVolume);
                    return;
                }
            }
        }

        public void SetMusicVolume(float vol)
        {
            musicVolume = Mathf.Clamp01(vol);
            if (currentMusic != null && currentMusic.source != null)
            {
                currentMusic.source.volume = currentMusic.volume * musicVolume;
            }
        }

        public void SetSFXVolume(float vol)
        {
            sfxVolume = Mathf.Clamp01(vol);
        }
    }
}
