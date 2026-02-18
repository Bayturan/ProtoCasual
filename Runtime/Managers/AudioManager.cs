using UnityEngine;
using ProtoCasual.Core.Utilities;

namespace ProtoCasual.Core.Managers
{
    /// <summary>
    /// Manages audio playback for SFX, music, and UI sounds.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;

        [Header("Settings")]
        [SerializeField] private float defaultMusicVolume = 0.5f;
        [SerializeField] private float defaultSfxVolume = 1f;

        public bool IsMusicEnabled { get; private set; } = true;
        public bool IsSfxEnabled { get; private set; } = true;

        protected override void Awake()
        {
            base.Awake();

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = defaultMusicVolume;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (uiSource == null)
            {
                uiSource = gameObject.AddComponent<AudioSource>();
                uiSource.playOnAwake = false;
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            if (!IsMusicEnabled || clip == null || musicSource == null) return;

            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (!IsSfxEnabled || clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip, defaultSfxVolume);
        }

        public void PlayUI(AudioClip clip)
        {
            if (!IsSfxEnabled || clip == null || uiSource == null) return;
            uiSource.PlayOneShot(clip);
        }

        public void SetMusicEnabled(bool enabled)
        {
            IsMusicEnabled = enabled;
            if (musicSource != null)
                musicSource.mute = !enabled;
        }

        public void SetSfxEnabled(bool enabled)
        {
            IsSfxEnabled = enabled;
            if (sfxSource != null)
                sfxSource.mute = !enabled;
            if (uiSource != null)
                uiSource.mute = !enabled;
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
                musicSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSfxVolume(float volume)
        {
            defaultSfxVolume = Mathf.Clamp01(volume);
        }
    }
}
