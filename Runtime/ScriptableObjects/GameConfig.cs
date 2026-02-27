using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Performance")]
        public int targetFPS = 60;
        public bool multithreadedRendering = true;
        public bool neverSleep = true;

        [Header("Gameplay")]
        [Tooltip("Default Time.timeScale at boot.")]
        public float defaultTimeScale = 1f;
        [Tooltip("Physics.gravity.y multiplier.")]
        public float gravityMultiplier = 1f;

        [Header("Settings")]
        public bool vibrationEnabled;
        public bool soundEnabled = true;

        [Header("Debug")]
        public bool showFPSCounter;
        public bool verboseLogging;
    }
}
