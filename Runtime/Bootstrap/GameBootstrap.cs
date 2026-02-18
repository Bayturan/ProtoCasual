using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Currency;
using ProtoCasual.Core.Inventory;
using ProtoCasual.Core.Store;
using ProtoCasual.Core.Haptics;
using ProtoCasual.Core.Rewards;
using ProtoCasual.Core.Analytics;
using ProtoCasual.Core.Tutorial;
using ProtoCasual.Core.Leaderboard;
using ProtoCasual.Core.Achievements;

namespace ProtoCasual.Core.Bootstrap
{
    /// <summary>
    /// Entry point for the game. Initializes ServiceLocator, registers services, and bootstraps managers.
    /// Place this on the first GameObject in your scene hierarchy.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("Haptics")]
        [SerializeField] private HapticConfig hapticConfig;

        [Header("Economy")]
        [SerializeField] private RewardConfig[] rewardConfigs;
        [SerializeField] private DailyRewardConfig dailyRewardConfig;

        [Header("Analytics")]
        [SerializeField] private AnalyticsConfig analyticsConfig;

        [Header("Tutorial")]
        [SerializeField] private TutorialConfig tutorialConfig;

        [Header("Social")]
        [SerializeField] private LeaderboardConfig leaderboardConfig;
        [SerializeField] private AchievementConfig achievementConfig;

        private void Awake()
        {
            ServiceLocator.Initialize();
            Application.targetFrameRate = gameConfig != null ? gameConfig.targetFPS : 60;
        }

        private void Start()
        {
            RegisterServices();
            InitializeManagers();
        }

        /// <summary>
        /// Override or extend this to register your own services with the ServiceLocator.
        /// </summary>
        protected virtual void RegisterServices()
        {
            // InputService
            var inputService = FindAnyObjectByType<Systems.InputManager>();
            if (inputService != null)
                ServiceLocator.Register<IInputService>(inputService);

            // SaveService
            var saveService = FindAnyObjectByType<Managers.SaveService>();
            if (saveService != null)
                ServiceLocator.Register<ISaveService>(saveService);

            // PlayerDataProvider  (loads PlayerData from save)
            var save = ServiceLocator.Get<ISaveService>();
            var dataProvider = new PlayerDataProvider(save);
            ServiceLocator.Register(dataProvider);

            // ItemDatabase
            if (itemDatabase != null)
            {
                itemDatabase.Initialise();
                ServiceLocator.Register(itemDatabase);
            }

            // CurrencyService
            var currencyService = new CurrencyService(dataProvider, save);
            ServiceLocator.Register<ICurrencyService>(currencyService);

            // InventoryService
            var inventoryService = new InventoryService(dataProvider);
            ServiceLocator.Register<IInventoryService>(inventoryService);

            // StoreService
            if (itemDatabase != null)
            {
                var storeService = new StoreService(itemDatabase, currencyService, inventoryService);
                ServiceLocator.Register<IStoreService>(storeService);
            }

            // HapticService
            var hapticService = new HapticService(hapticConfig);
            ServiceLocator.Register<IHapticService>(hapticService);

            // AnalyticsService (default = console logger; replace with your own)
            var analyticsService = new DebugAnalyticsService(analyticsConfig);
            ServiceLocator.Register<IAnalyticsService>(analyticsService);

            // RewardService
            var rewardService = new RewardService(currencyService, inventoryService, rewardConfigs);
            ServiceLocator.Register<IRewardService>(rewardService);

            // DailyRewardService
            if (dailyRewardConfig != null)
            {
                var dailyRewardService = new DailyRewardService(dailyRewardConfig, dataProvider, rewardService);
                ServiceLocator.Register<IDailyRewardService>(dailyRewardService);
            }

            // TutorialService
            if (tutorialConfig != null)
            {
                var tutorialService = new TutorialService(tutorialConfig, dataProvider, analyticsService);
                ServiceLocator.Register<ITutorialService>(tutorialService);
            }

            // LeaderboardService
            if (leaderboardConfig != null)
            {
                var leaderboardService = new LocalLeaderboardService(leaderboardConfig, dataProvider);
                ServiceLocator.Register<ILeaderboardService>(leaderboardService);
            }

            // AchievementService
            if (achievementConfig != null)
            {
                var achievementService = new AchievementService(achievementConfig, dataProvider, rewardService);
                ServiceLocator.Register<IAchievementService>(achievementService);
            }
        }

        protected virtual void InitializeManagers()
        {
            if (UI.UIManager.Instance != null)
                UI.UIManager.Instance.Initialize();
        }
    }
}
