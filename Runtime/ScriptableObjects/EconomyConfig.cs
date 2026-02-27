using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Economy configuration — initial currency values, limits, and economy settings.
    /// Create via: Create → ProtoCasual → Config → Economy Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Economy Config")]
    public class EconomyConfig : ScriptableObject
    {
        [Header("Starting Currency")]
        [Tooltip("Soft currency the player starts with on first launch.")]
        public int startingSoftCurrency = 100;

        [Tooltip("Hard currency the player starts with on first launch.")]
        public int startingHardCurrency = 0;

        [Header("Currency Limits")]
        [Tooltip("Maximum soft currency. 0 = unlimited.")]
        public int maxSoftCurrency = 0;

        [Tooltip("Maximum hard currency. 0 = unlimited.")]
        public int maxHardCurrency = 0;

        [Header("Level Rewards")]
        [Tooltip("Base soft currency reward for completing a level.")]
        public int baseLevelReward = 50;

        [Tooltip("Multiplier per level index (reward = base + level * multiplier).")]
        public float levelRewardMultiplier = 1.2f;

        [Header("Ad Rewards")]
        [Tooltip("Soft currency granted for watching a rewarded ad.")]
        public int rewardedAdCurrencyAmount = 50;

        [Tooltip("Multiplier applied to level reward when watching double-reward ad.")]
        public float doubleRewardMultiplier = 2f;
    }
}
