using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Leaderboard configuration.
    /// Create via: Create → ProtoCasual → Config → Leaderboard Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Leaderboard Config")]
    public class LeaderboardConfig : ScriptableObject
    {
        [Header("Leaderboards")]
        public LeaderboardDefinition[] leaderboards;
    }

    [System.Serializable]
    public class LeaderboardDefinition
    {
        public string leaderboardId;
        public string displayName;
        public Sprite icon;

        [Tooltip("Maximum entries to cache locally.")]
        public int maxEntries = 100;

        [Tooltip("Sort order: true = highest first, false = lowest first.")]
        public bool descending = true;
    }
}
