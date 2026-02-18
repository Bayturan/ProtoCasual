using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Rewards
{
    /// <summary>
    /// Centralized reward granting. Delegates to CurrencyService / InventoryService.
    /// Supports level-mapped rewards via <see cref="RewardConfig"/> assets.
    /// Register as <see cref="IRewardService"/> in ServiceLocator.
    /// </summary>
    public class RewardService : IRewardService
    {
        public event Action<RewardEntry[]> OnRewardsGranted;

        private readonly ICurrencyService currencyService;
        private readonly IInventoryService inventoryService;
        private readonly Dictionary<int, RewardConfig> levelRewards = new();

        public RewardService(ICurrencyService currencyService, IInventoryService inventoryService,
            RewardConfig[] configs = null)
        {
            this.currencyService = currencyService;
            this.inventoryService = inventoryService;

            if (configs != null)
            {
                foreach (var cfg in configs)
                {
                    if (cfg != null && cfg.isLevelReward)
                        levelRewards[cfg.levelIndex] = cfg;
                }
            }
        }

        public void GrantRewards(params RewardEntry[] rewards)
        {
            if (rewards == null || rewards.Length == 0) return;

            foreach (var r in rewards)
            {
                if (r == null || r.Amount <= 0) continue;

                switch (r.Type)
                {
                    case RewardType.SoftCurrency:
                        currencyService?.AddSoft(r.Amount);
                        break;
                    case RewardType.HardCurrency:
                        currencyService?.AddHard(r.Amount);
                        break;
                    case RewardType.Item:
                        inventoryService?.AddItem(r.RewardId, r.Amount);
                        break;
                }
            }

            OnRewardsGranted?.Invoke(rewards);
            Debug.Log($"[RewardService] Granted {rewards.Length} reward(s).");
        }

        public void GrantLevelReward(int levelIndex)
        {
            if (!levelRewards.TryGetValue(levelIndex, out var config))
            {
                Debug.Log($"[RewardService] No reward config for level {levelIndex}.");
                return;
            }

            GrantRewards(config.rewards);
        }
    }
}
