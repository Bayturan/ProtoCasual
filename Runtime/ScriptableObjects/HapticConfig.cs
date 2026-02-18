using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Configuration for haptic feedback patterns.
    /// Create via: Create → ProtoCasual → Config → Haptic Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Haptic Config")]
    public class HapticConfig : ScriptableObject
    {
        [Header("General")]
        [Tooltip("Enable haptic feedback globally.")]
        public bool enabled = true;

        [Header("Durations (ms) — Android only")]
        public long lightDuration = 20;
        public long mediumDuration = 40;
        public long heavyDuration = 80;
        public long selectionDuration = 10;
        public long successDuration = 50;
        public long warningDuration = 60;
        public long errorDuration = 100;
    }
}
