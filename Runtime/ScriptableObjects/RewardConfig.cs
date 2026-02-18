using UnityEngine;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Defines a single reward tier (e.g., level-complete reward).
    /// Create via: Create → ProtoCasual → Economy → Reward Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Economy/Reward Config")]
    public class RewardConfig : ScriptableObject
    {
        [Header("Identity")]
        public string rewardId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Contents")]
        public RewardEntry[] rewards;

        [Header("Level Mapping (optional)")]
        [Tooltip("If set, maps level indices to reward tiers. Index = level, value = this config.")]
        public bool isLevelReward;
        public int levelIndex;
    }
}
