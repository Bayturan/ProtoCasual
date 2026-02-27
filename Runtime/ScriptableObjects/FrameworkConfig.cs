using UnityEngine;
using ProtoCasual.Core.Events;
using ProtoCasual.Core.Store;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Master configuration asset for the ProtoCasual framework.
    /// This is the ONLY ScriptableObject you need to assign to GameBootstrap.
    /// All sub-configs are referenced here. The setup wizard creates and wires everything.
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Framework Config", order = -100)]
    public class FrameworkConfig : ScriptableObject
    {
        // ─── Core ───────────────────────────────────────────────────────

        [Header("Core")]
        [Tooltip("Main game configuration (FPS, settings, etc.)")]
        public GameConfig gameConfig;

        [Tooltip("Audio configuration (volumes, clips, etc.)")]
        public AudioConfig audioConfig;

        [Tooltip("Economy configuration (initial currencies, limits, etc.)")]
        public EconomyConfig economyConfig;

        // ─── Game Events ────────────────────────────────────────────────

        [Header("Game Events")]
        public GameEvent onGameStart;
        public GameEvent onGamePause;
        public GameEvent onGameResume;
        public GameEvent onGameComplete;
        public GameEvent onGameFail;
        public GameEvent onLevelLoaded;
        public GameEventInt onScoreChanged;
        public GameEventInt onCurrencyChanged;

        // ─── Feature Toggles ────────────────────────────────────────────

        [Header("Feature Toggles")]
        public bool enableStore = true;
        public bool enableHaptics = true;
        public bool enableAnalytics = true;
        public bool enableRewards = true;
        public bool enableDailyRewards;
        public bool enableTutorial;
        public bool enableLeaderboards;
        public bool enableAchievements;
        public bool enablePopups = true;
        public bool enableBots;

        // ─── Feature Configs (assigned based on toggles) ────────────────

        [Header("Store & Economy")]
        [Tooltip("Item database for the store system.")]
        public ItemDatabase itemDatabase;

        [Tooltip("Reward configurations for level completion, etc.")]
        public RewardConfig[] rewardConfigs;

        [Header("Haptics")]
        public HapticConfig hapticConfig;

        [Header("Analytics")]
        public AnalyticsConfig analyticsConfig;

        [Header("Daily Rewards")]
        public DailyRewardConfig dailyRewardConfig;

        [Header("Tutorial")]
        public TutorialConfig tutorialConfig;

        [Header("Leaderboards")]
        public LeaderboardConfig leaderboardConfig;

        [Header("Achievements")]
        public AchievementConfig achievementConfig;

        [Header("Bots")]
        public BotConfig[] botConfigs;

        [Header("Map & Levels")]
        public LevelConfig[] levelConfigs;
        public MapConfig mapConfig;
        public GameModeConfig gameModeConfig;
    }
}
