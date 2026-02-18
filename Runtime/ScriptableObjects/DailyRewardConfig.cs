using UnityEngine;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Configuration for the daily reward / login-streak system.
    /// Create via: Create → ProtoCasual → Economy → Daily Reward Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Economy/Daily Reward Config")]
    public class DailyRewardConfig : ScriptableObject
    {
        [Header("Streak Settings")]
        [Tooltip("Number of days in the reward cycle before it loops.")]
        public int cycleDays = 7;

        [Tooltip("Hours before the streak resets if player misses a day.")]
        public int streakExpiryHours = 48;

        [Header("Daily Rewards")]
        [Tooltip("Rewards for each day. Index 0 = Day 1. Loops if streak > array length.")]
        public DayReward[] days;
    }

    [System.Serializable]
    public class DayReward
    {
        public string label;
        public Sprite icon;
        public RewardEntry[] rewards;
    }
}
