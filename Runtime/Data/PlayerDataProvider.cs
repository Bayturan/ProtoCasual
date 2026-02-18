using UnityEngine;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Data
{
    /// <summary>
    /// Single owner of <see cref="PlayerData"/>.
    /// Loads on construction, provides Save() for child services.
    /// Registered in ServiceLocator so every service shares the same instance.
    /// </summary>
    public class PlayerDataProvider
    {
        private const string SAVE_KEY = "PlayerData";

        public PlayerData Data { get; private set; }

        private readonly ISaveService saveService;

        public PlayerDataProvider(ISaveService saveService)
        {
            this.saveService = saveService;
            Load();
        }

        /// <summary>
        /// Persist the current snapshot to disk.
        /// </summary>
        public void Save()
        {
            saveService?.Save(SAVE_KEY, Data);
        }

        /// <summary>
        /// Reload from disk (or create defaults).
        /// </summary>
        public void Load()
        {
            Data = saveService != null
                ? saveService.Load(SAVE_KEY, CreateDefault())
                : CreateDefault();
        }

        /// <summary>
        /// Delete all player data and recreate defaults.
        /// </summary>
        public void Reset()
        {
            saveService?.Delete(SAVE_KEY);
            Data = CreateDefault();
            Save();
        }

        private static PlayerData CreateDefault()
        {
            return new PlayerData
            {
                Version = 2,
                Currency = new CurrencyData { SoftCurrency = 0, HardCurrency = 0 },
                Inventory = new InventoryData(),
                Equipment = new EquipmentData(),
                DailyReward = new DailyRewardData(),
                Tutorial = new TutorialSaveData(),
                Leaderboards = new LeaderboardSaveData(),
                Achievements = new AchievementSaveData()
            };
        }
    }
}
