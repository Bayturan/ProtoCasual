using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Audio configuration — default volumes, audio clips for common game events.
    /// Create via: Create → ProtoCasual → Config → Audio Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Audio Config")]
    public class AudioConfig : ScriptableObject
    {
        [Header("Volume Defaults")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.5f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float uiVolume = 1f;

        [Header("Music")]
        public AudioClip menuMusic;
        public AudioClip gameplayMusic;
        public AudioClip winMusic;
        public AudioClip loseMusic;

        [Header("UI SFX")]
        public AudioClip buttonClick;
        public AudioClip popupOpen;
        public AudioClip popupClose;

        [Header("Game SFX")]
        public AudioClip scorePoint;
        public AudioClip collectItem;
        public AudioClip levelComplete;
        public AudioClip levelFail;
        public AudioClip countdown;

        [Header("Settings")]
        [Tooltip("Fade duration in seconds when switching music tracks.")]
        public float musicFadeDuration = 0.5f;
        [Tooltip("Maximum simultaneous SFX sounds.")]
        public int maxSimultaneousSFX = 8;
    }
}
