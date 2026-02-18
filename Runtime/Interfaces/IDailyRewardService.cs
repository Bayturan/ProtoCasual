using System;

namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Daily reward / login-streak service.
    /// </summary>
    public interface IDailyRewardService
    {
        event Action<int, RewardEntry[]> OnDailyRewardClaimed;

        int CurrentStreak { get; }
        bool CanClaimToday { get; }
        DateTime LastClaimTime { get; }

        RewardEntry[] PeekTodayReward();
        bool TryClaim();
        void Reset();
    }
}
