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
    /// Master Audio Manager supporting Music ON/OFF, SFX ON/OFF, Vibration ON/OFF, Master Volume controls,
    /// and sound categories (Coin, Jump, Slide, Collision, PowerUp, Button, GameOver, DistrictUnlocked).
    /// Includes procedural audio synthesis so all game feedback plays seamlessly even without external audio clips!
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
        private AudioSource dedicatedSfxSource;
        private Dictionary<string, AudioClip> proceduralClipCache = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            dedicatedSfxSource = gameObject.AddComponent<AudioSource>();
            dedicatedSfxSource.playOnAwake = false;

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

            if (musicTracks != null && musicTracks.Length > 0)
            {
                Sound s = System.Array.Find(musicTracks, track => track.name == name);
                if (s != null && s.source != null && s.clip != null)
                {
                    if (currentMusic != null && currentMusic.source != null && currentMusic.source.isPlaying)
                    {
                        currentMusic.source.Stop();
                    }

                    currentMusic = s;
                    s.source.volume = s.volume * musicVolume;
                    s.source.Play();
                    return;
                }
            }
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
                    TriggerHapticFeedback(name);
                    return;
                }
            }

            AudioClip clip = GetOrCreateProceduralClip(name);
            if (clip != null && dedicatedSfxSource != null)
            {
                dedicatedSfxSource.PlayOneShot(clip, sfxVolume);
            }

            TriggerHapticFeedback(name);
        }

        private void TriggerHapticFeedback(string sfxName)
        {
            if (SaveManager.Instance != null && !SaveManager.Instance.data.isVibrationOn) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            if (sfxName == "Collision" || sfxName == "GameOver")
            {
                Handheld.Vibrate();
            }
#endif
        }

        private AudioClip GetOrCreateProceduralClip(string soundName)
        {
            if (proceduralClipCache.TryGetValue(soundName, out AudioClip cached) && cached != null)
            {
                return cached;
            }

            AudioClip clip = GenerateProceduralClip(soundName);
            if (clip != null)
            {
                proceduralClipCache[soundName] = clip;
            }
            return clip;
        }

        private AudioClip GenerateProceduralClip(string soundName)
        {
            int sampleRate = 44100;
            float duration = 0.2f;

            switch (soundName)
            {
                case "Coin":
                    duration = 0.15f;
                    return CreateChimeClip("Procedural_Coin", sampleRate, duration, 987f, 1318f);

                case "Jump":
                    duration = 0.22f;
                    return CreateSweepClip("Procedural_Jump", sampleRate, duration, 220f, 580f);

                case "Slide":
                    duration = 0.25f;
                    return CreateSweepClip("Procedural_Slide", sampleRate, duration, 320f, 120f);

                case "Collision":
                case "GameOver":
                    duration = 0.45f;
                    return CreateNoiseCrashClip("Procedural_Collision", sampleRate, duration);

                case "PowerUp":
                    duration = 0.35f;
                    return CreateArpeggioClip("Procedural_PowerUp", sampleRate, duration);

                case "DistrictUnlocked":
                    duration = 0.50f;
                    return CreateFanfareClip("Procedural_DistrictUnlocked", sampleRate, duration);

                case "ButtonClick":
                default:
                    duration = 0.05f;
                    return CreateChimeClip("Procedural_Click", sampleRate, duration, 880f, 880f);
            }
        }

        private AudioClip CreateChimeClip(string name, int sampleRate, float duration, float f1, float f2)
        {
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float freq = t < duration * 0.5f ? f1 : f2;
                float env = 1f - (t / duration);
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.45f;
            }
            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateSweepClip(string name, int sampleRate, float duration, float startFreq, float endFreq)
        {
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float progress = t / duration;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, progress);
                float env = Mathf.Sin(progress * Mathf.PI);
                data[i] = Mathf.Sin(2f * Mathf.PI * currentFreq * t) * env * 0.40f;
            }
            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateNoiseCrashClip(string name, int sampleRate, float duration)
        {
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float progress = t / duration;
                float env = Mathf.Exp(-5f * progress);
                float noise = (Random.value * 2f - 1f) * 0.5f;
                float lowTone = Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.5f;
                data[i] = (noise + lowTone) * env * 0.5f;
            }
            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateArpeggioClip(string name, int sampleRate, float duration)
        {
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };
            float noteDuration = duration / notes.Length;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                int noteIndex = Mathf.Clamp(Mathf.FloorToInt(t / noteDuration), 0, notes.Length - 1);
                float noteT = t - (noteIndex * noteDuration);
                float env = 1f - (noteT / noteDuration);
                data[i] = Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * t) * env * 0.45f;
            }
            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateFanfareClip(string name, int sampleRate, float duration)
        {
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            float[] notes = { 587.33f, 739.99f, 880f, 1174.66f };
            float noteDuration = duration / notes.Length;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                int noteIndex = Mathf.Clamp(Mathf.FloorToInt(t / noteDuration), 0, notes.Length - 1);
                float noteT = t - (noteIndex * noteDuration);
                float env = 1f - (noteT / noteDuration);
                data[i] = Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * t) * env * 0.45f;
            }
            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
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
