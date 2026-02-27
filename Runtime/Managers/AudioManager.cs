using UnityEngine;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Utilities;

namespace ProtoCasual.Core.Managers
{
    /// <summary>
    /// Manages audio playback for SFX, music, and UI sounds.
    /// Loads AudioConfig from FrameworkConfig via GameBootstrap.ApplyConfig().
    /// Provides convenience methods for all pre-configured clips.
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

        private AudioConfig audioConfig;

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

        // ─── Config ─────────────────────────────────────────────────────

        /// <summary>
        /// Called by GameBootstrap to apply AudioConfig settings.
        /// </summary>
        public void ApplyConfig(AudioConfig config)
        {
            audioConfig = config;
            if (config == null) return;

            defaultMusicVolume = config.musicVolume * config.masterVolume;
            defaultSfxVolume = config.sfxVolume * config.masterVolume;

            if (musicSource != null)
                musicSource.volume = defaultMusicVolume;
            if (uiSource != null)
                uiSource.volume = config.uiVolume * config.masterVolume;
        }

        // ─── Core Playback ──────────────────────────────────────────────

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

        // ─── Convenience: Music ─────────────────────────────────────────

        public void PlayMenuMusic()
        {
            if (audioConfig != null) PlayMusic(audioConfig.menuMusic);
        }

        public void PlayGameplayMusic()
        {
            if (audioConfig != null) PlayMusic(audioConfig.gameplayMusic);
        }

        public void PlayWinMusic()
        {
            if (audioConfig != null) PlayMusic(audioConfig.winMusic);
        }

        public void PlayLoseMusic()
        {
            if (audioConfig != null) PlayMusic(audioConfig.loseMusic);
        }

        // ─── Convenience: Game SFX ──────────────────────────────────────

        public void PlayScorePoint()
        {
            if (audioConfig != null) PlaySFX(audioConfig.scorePoint);
        }

        public void PlayCollectItem()
        {
            if (audioConfig != null) PlaySFX(audioConfig.collectItem);
        }

        public void PlayLevelComplete()
        {
            if (audioConfig != null) PlaySFX(audioConfig.levelComplete);
        }

        public void PlayLevelFail()
        {
            if (audioConfig != null) PlaySFX(audioConfig.levelFail);
        }

        public void PlayCountdown()
        {
            if (audioConfig != null) PlaySFX(audioConfig.countdown);
        }

        // ─── Convenience: UI SFX ────────────────────────────────────────

        public void PlayButtonClick()
        {
            if (audioConfig != null) PlayUI(audioConfig.buttonClick);
        }

        public void PlayPopupOpen()
        {
            if (audioConfig != null) PlayUI(audioConfig.popupOpen);
        }

        public void PlayPopupClose()
        {
            if (audioConfig != null) PlayUI(audioConfig.popupClose);
        }

        // ─── Settings ───────────────────────────────────────────────────

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
