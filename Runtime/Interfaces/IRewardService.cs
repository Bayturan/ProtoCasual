using System;

namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Reward descriptor returned by reward services.
    /// </summary>
    [Serializable]
    public class RewardEntry
    {
        public string RewardId;
        public RewardType Type;
        public int Amount;
    }

    public enum RewardType { SoftCurrency, HardCurrency, Item }

    /// <summary>
    /// Centralized reward granting — level-complete, ad-watch, daily login, etc.
    /// Delegates actual granting to CurrencyService / InventoryService.
    /// </summary>
    public interface IRewardService
    {
        event Action<RewardEntry[]> OnRewardsGranted;

        void GrantRewards(params RewardEntry[] rewards);
        void GrantLevelReward(int levelIndex);
    }
}
