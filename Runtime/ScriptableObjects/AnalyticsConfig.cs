using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Analytics configuration — toggle events, set flush intervals.
    /// Create via: Create → ProtoCasual → Config → Analytics Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Analytics Config")]
    public class AnalyticsConfig : ScriptableObject
    {
        [Header("General")]
        public bool enabled = true;

        [Tooltip("Log events to console for debugging.")]
        public bool debugLogging = true;

        [Header("Event Toggles")]
        public bool trackLevelEvents = true;
        public bool trackPurchaseEvents = true;
        public bool trackAdEvents = true;
        public bool trackTutorialEvents = true;
    }
}
