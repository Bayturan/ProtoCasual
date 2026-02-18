using System;
using UnityEngine;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Rewards
{
    /// <summary>
    /// Daily login-streak reward service backed by <see cref="PlayerData.DailyReward"/>.
    /// Auto-saves through <see cref="PlayerDataProvider"/>.
    /// Register as <see cref="IDailyRewardService"/> in ServiceLocator.
    /// </summary>
    public class DailyRewardService : IDailyRewardService
    {
        public event Action<int, RewardEntry[]> OnDailyRewardClaimed;

        private readonly DailyRewardConfig config;
        private readonly DailyRewardData data;
        private readonly PlayerDataProvider dataProvider;
        private readonly IRewardService rewardService;

        public int CurrentStreak => data.Streak;
        public DateTime LastClaimTime => DateTime.FromBinary(data.LastClaimTimeTicks);

        public bool CanClaimToday
        {
            get
            {
                if (data.LastClaimTimeTicks == 0) return true;
                var last = DateTime.FromBinary(data.LastClaimTimeTicks);
                return DateTime.UtcNow.Date > last.Date;
            }
        }

        public DailyRewardService(DailyRewardConfig config, PlayerDataProvider dataProvider,
            IRewardService rewardService)
        {
            this.config = config;
            this.dataProvider = dataProvider;
            this.rewardService = rewardService;
            data = dataProvider.Data.DailyReward;
            ValidateStreak();
        }

        public RewardEntry[] PeekTodayReward()
        {
            if (config == null || config.days == null || config.days.Length == 0)
                return Array.Empty<RewardEntry>();

            int dayIndex = data.Streak % config.cycleDays;
            dayIndex = Mathf.Clamp(dayIndex, 0, config.days.Length - 1);
            return config.days[dayIndex].rewards;
        }

        public bool TryClaim()
        {
            if (!CanClaimToday || config == null) return false;

            int dayIndex = data.Streak % config.cycleDays;
            dayIndex = Mathf.Clamp(dayIndex, 0, config.days.Length - 1);

            var rewards = config.days[dayIndex].rewards;
            rewardService?.GrantRewards(rewards);

            data.Streak++;
            data.LastClaimTimeTicks = DateTime.UtcNow.ToBinary();
            dataProvider.Save();

            OnDailyRewardClaimed?.Invoke(data.Streak, rewards);
            Debug.Log($"[DailyRewardService] Claimed day {data.Streak}.");
            return true;
        }

        public void Reset()
        {
            data.Streak = 0;
            data.LastClaimTimeTicks = 0;
            dataProvider.Save();
        }

        private void ValidateStreak()
        {
            if (data.LastClaimTimeTicks == 0) return;

            var last = DateTime.FromBinary(data.LastClaimTimeTicks);
            var hoursSince = (DateTime.UtcNow - last).TotalHours;

            if (hoursSince > (config != null ? config.streakExpiryHours : 48))
            {
                Debug.Log("[DailyRewardService] Streak expired — resetting.");
                data.Streak = 0;
                dataProvider.Save();
            }
        }
    }
}
