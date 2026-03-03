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
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Core.Bootstrap
{
    /// <summary>
    /// Entry point for the game. Only requires a single FrameworkConfig reference.
    /// All sub-configs, events, and services are auto-wired from the master config.
    /// Place this on the first GameObject in your scene hierarchy.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Master Config (only thing you need to assign)")]
        [SerializeField] private FrameworkConfig frameworkConfig;

        private void Awake()
        {
            ServiceLocator.Initialize();

            // Register FrameworkConfig itself so any system can access it
            if (frameworkConfig != null)
                ServiceLocator.Register(frameworkConfig);

            // Apply core game settings
            var gc = frameworkConfig != null ? frameworkConfig.gameConfig : null;
            Application.targetFrameRate = gc != null ? gc.targetFPS : 60;

            if (gc != null)
            {
                Screen.sleepTimeout = gc.neverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
                Time.timeScale = gc.defaultTimeScale;
            }
        }

        private void Start()
        {
            RegisterCoreServices();
            RegisterEconomyServices();
            RegisterOptionalServices();
            InitializeManagers();
            WireGameEvents();
        }

        // ─── Core Services ──────────────────────────────────────────────

        private void RegisterCoreServices()
        {
            // InputService
            var inputService = FindAnyObjectByType<Systems.InputManager>();
            if (inputService != null)
                ServiceLocator.Register<IInputService>(inputService);

            // SaveService
            var saveService = FindAnyObjectByType<SaveService>();
            if (saveService != null)
                ServiceLocator.Register<ISaveService>(saveService);

            // PlayerDataProvider
            var save = ServiceLocator.Get<ISaveService>();
            var dataProvider = new PlayerDataProvider(save);
            ServiceLocator.Register(dataProvider);
        }

        // ─── Economy & Store Services ───────────────────────────────────

        private void RegisterEconomyServices()
        {
            var dataProvider = ServiceLocator.Get<PlayerDataProvider>();
            var save = ServiceLocator.Get<ISaveService>();

            // CurrencyService
            var currencyService = new CurrencyService(dataProvider, save);
            ServiceLocator.Register<ICurrencyService>(currencyService);

            // InventoryService
            var inventoryService = new InventoryService(dataProvider);
            ServiceLocator.Register<IInventoryService>(inventoryService);

            // EquipmentService
            var equipmentService = new EquipmentService(dataProvider, inventoryService);
            ServiceLocator.Register<IEquipmentService>(equipmentService);

            // Apply starting currency on first launch
            if (frameworkConfig != null && frameworkConfig.economyConfig != null)
            {
                var eco = frameworkConfig.economyConfig;
                if (!PlayerPrefs.HasKey("ProtoCasual_EconomyInitialized"))
                {
                    currencyService.AddSoft(eco.startingSoftCurrency);
                    currencyService.AddHard(eco.startingHardCurrency);
                    PlayerPrefs.SetInt("ProtoCasual_EconomyInitialized", 1);
                    PlayerPrefs.Save();
                    Debug.Log($"[GameBootstrap] Starting currency applied: {eco.startingSoftCurrency} soft, {eco.startingHardCurrency} hard.");
                }
            }

            // Store
            if (frameworkConfig != null && frameworkConfig.enableStore && frameworkConfig.itemDatabase != null)
            {
                var itemDb = frameworkConfig.itemDatabase;
                itemDb.Initialise();
                ServiceLocator.Register(itemDb);

                var storeService = new StoreService(itemDb, currencyService, inventoryService);
                ServiceLocator.Register<IStoreService>(storeService);
            }

            // RewardService
            var rewardService = new RewardService(
                currencyService, inventoryService,
                frameworkConfig != null ? frameworkConfig.rewardConfigs : null);
            ServiceLocator.Register<IRewardService>(rewardService);
        }

        // ─── Optional Feature Services ──────────────────────────────────

        private void RegisterOptionalServices()
        {
            if (frameworkConfig == null) return;

            var dataProvider = ServiceLocator.Get<PlayerDataProvider>();

            // Haptics
            if (frameworkConfig.enableHaptics)
            {
                var hapticService = new HapticService(frameworkConfig.hapticConfig);
                ServiceLocator.Register<IHapticService>(hapticService);
            }

            // Analytics
            IAnalyticsService analyticsService = null;
            if (frameworkConfig.enableAnalytics)
            {
                analyticsService = new DebugAnalyticsService(frameworkConfig.analyticsConfig);
                ServiceLocator.Register<IAnalyticsService>(analyticsService);
            }

            // Daily Rewards
            if (frameworkConfig.enableDailyRewards && frameworkConfig.dailyRewardConfig != null)
            {
                var rewardService = ServiceLocator.Get<IRewardService>();
                var dailyRewardService = new DailyRewardService(
                    frameworkConfig.dailyRewardConfig, dataProvider, rewardService);
                ServiceLocator.Register<IDailyRewardService>(dailyRewardService);
            }

            // Tutorial
            if (frameworkConfig.enableTutorial && frameworkConfig.tutorialConfig != null)
            {
                var tutorialService = new TutorialService(
                    frameworkConfig.tutorialConfig, dataProvider, analyticsService);
                ServiceLocator.Register<ITutorialService>(tutorialService);
            }

            // Leaderboards
            if (frameworkConfig.enableLeaderboards && frameworkConfig.leaderboardConfig != null)
            {
                var leaderboardService = new LocalLeaderboardService(
                    frameworkConfig.leaderboardConfig, dataProvider);
                ServiceLocator.Register<ILeaderboardService>(leaderboardService);
            }

            // Achievements
            if (frameworkConfig.enableAchievements && frameworkConfig.achievementConfig != null)
            {
                var rewardService = ServiceLocator.Get<IRewardService>();
                var achievementService = new AchievementService(
                    frameworkConfig.achievementConfig, dataProvider, rewardService);
                ServiceLocator.Register<IAchievementService>(achievementService);
            }
        }

        // ─── Manager Init ───────────────────────────────────────────────

        protected virtual void InitializeManagers()
        {
            if (UI.UIToolkitManager.Instance != null)
                UI.UIToolkitManager.Instance.Initialize();

            // Apply AudioConfig to AudioManager
            if (frameworkConfig != null && frameworkConfig.audioConfig != null &&
                AudioManager.Instance != null)
            {
                AudioManager.Instance.ApplyConfig(frameworkConfig.audioConfig);
            }
        }

        // ─── Wire GameEvents to GameManager ─────────────────────────────

        private void WireGameEvents()
        {
            if (frameworkConfig == null || GameManager.Instance == null) return;

            GameManager.Instance.SetFrameworkEvents(
                frameworkConfig.onGameStart,
                frameworkConfig.onGamePause,
                frameworkConfig.onGameResume,
                frameworkConfig.onGameComplete,
                frameworkConfig.onGameFail);
        }
    }
}
