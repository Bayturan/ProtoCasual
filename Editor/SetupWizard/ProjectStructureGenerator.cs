using UnityEngine;
using UnityEditor;
using System.IO;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Events;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Store;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Creates the Assets folder structure, default ScriptableObject configs,
    /// GameEvent assets, and master FrameworkConfig for a new ProtoCasual project.
    /// Returns FrameworkConfig so SceneBuilder can auto-wire it to GameBootstrap.
    /// </summary>
    public static class ProjectStructureGenerator
    {
        // ─── Folder creation ────────────────────────────────────────────

        public static void CreateFolders(GameSetupConfig cfg)
        {
            string root = $"Assets/_Game";
            CreateDir(root);
            CreateDir($"{root}/Content");
            CreateDir($"{root}/Prefabs");
            CreateDir($"{root}/Prefabs/Levels");
            CreateDir($"{root}/Prefabs/UI");
            CreateDir($"{root}/ScriptableObjects");
            CreateDir($"{root}/ScriptableObjects/Events");
            CreateDir($"{root}/ScriptableObjects/Levels");
            CreateDir($"{root}/ScriptableObjects/Items");
            CreateDir($"{root}/ScriptableObjects/Rewards");
            CreateDir($"{root}/UI");
            CreateDir($"{root}/UI/Themes");
            CreateDir($"{root}/Materials");
            CreateDir($"{root}/Audio");
            CreateDir($"{root}/Audio/Music");
            CreateDir($"{root}/Audio/SFX");
            CreateDir($"{root}/Animations");
            CreateDir($"{root}/Scripts");
            CreateDir($"{root}/Scripts/GameModes");
            CreateDir($"{root}/Scripts/Mechanics");
            CreateDir("Assets/Scenes");

            // Game-type specific
            switch (cfg.gameType)
            {
                case GameType.Racing:
                    CreateDir($"{root}/Content/Tracks");
                    CreateDir($"{root}/Content/Vehicles");
                    break;
                case GameType.Puzzle:
                    CreateDir($"{root}/Content/Puzzles");
                    break;
                case GameType.Endless:
                case GameType.HyperCasual:
                    CreateDir($"{root}/Content/Chunks");
                    break;
                case GameType.Hybrid:
                    CreateDir($"{root}/Content/Levels");
                    break;
            }

            if (cfg.bots != BotOption.None)
                CreateDir($"{root}/Content/Bots");

            if (cfg.store == StoreOption.Enabled)
                CreateDir($"{root}/Content/Store");

            if (cfg.enableTutorial)
                CreateDir($"{root}/Content/Tutorial");

            if (cfg.enableAchievements)
                CreateDir($"{root}/Content/Achievements");

            AssetDatabase.Refresh();
            Debug.Log("[ProtoCasual] Project folders created.");
        }

        // ─── Full config + event creation returning FrameworkConfig ─────

        public static FrameworkConfig CreateDefaultConfigs(GameSetupConfig cfg)
        {
            string soDir = "Assets/_Game/ScriptableObjects";
            string evtDir = $"{soDir}/Events";
            CreateDir(soDir);
            CreateDir(evtDir);

            // ── Core configs ────────────────────────────────────────────

            var gameConfig = CreateAsset<GameConfig>($"{soDir}/GameConfig.asset", gc =>
            {
                gc.targetFPS = 60;
                gc.soundEnabled = true;
                gc.neverSleep = true;
                gc.defaultTimeScale = 1f;
            });

            var audioConfig = CreateAsset<AudioConfig>($"{soDir}/AudioConfig.asset", ac =>
            {
                ac.masterVolume = 1f;
                ac.musicVolume = 0.5f;
                ac.sfxVolume = 1f;
                ac.uiVolume = 1f;
                ac.musicFadeDuration = 0.5f;
                ac.maxSimultaneousSFX = 8;
            });

            var economyConfig = CreateAsset<EconomyConfig>($"{soDir}/EconomyConfig.asset", ec =>
            {
                ec.startingSoftCurrency = 100;
                ec.startingHardCurrency = 0;
                ec.baseLevelReward = 50;
                ec.levelRewardMultiplier = 1.2f;
                ec.rewardedAdCurrencyAmount = 50;
                ec.doubleRewardMultiplier = 2f;
            });

            // ── Game Events ─────────────────────────────────────────────

            var onGameStart = CreateAsset<GameEvent>($"{evtDir}/OnGameStart.asset");
            var onGamePause = CreateAsset<GameEvent>($"{evtDir}/OnGamePause.asset");
            var onGameResume = CreateAsset<GameEvent>($"{evtDir}/OnGameResume.asset");
            var onGameComplete = CreateAsset<GameEvent>($"{evtDir}/OnGameComplete.asset");
            var onGameFail = CreateAsset<GameEvent>($"{evtDir}/OnGameFail.asset");
            var onLevelLoaded = CreateAsset<GameEvent>($"{evtDir}/OnLevelLoaded.asset");
            var onScoreChanged = CreateAsset<GameEventInt>($"{evtDir}/OnScoreChanged.asset");
            var onCurrencyChanged = CreateAsset<GameEventInt>($"{evtDir}/OnCurrencyChanged.asset");

            // ── GameModeConfig ───────────────────────────────────────────

            var gameModeConfig = CreateAsset<GameModeConfig>($"{soDir}/GameModeConfig.asset", gmc =>
            {
                gmc.mode = ToGameModeType(cfg.gameType);
                gmc.modeName = cfg.gameType.ToString();
                gmc.description = $"Default {cfg.gameType} game mode configuration.";
            });

            // ── LevelConfig ─────────────────────────────────────────────

            var levelConfig = CreateAsset<LevelConfig>($"{soDir}/Levels/LevelConfig_01.asset", lc =>
            {
                lc.levelName = "Level 1";
                lc.levelIndex = 0;
            });

            // ── MapConfig ───────────────────────────────────────────────

            MapConfig mapConfig = null;
            if (cfg.mapType == MapType.Procedural || cfg.mapType == MapType.EndlessGeneration)
            {
                mapConfig = CreateAsset<MapConfig>($"{soDir}/MapConfig.asset", mc =>
                {
                    mc.mapName = "Default Map";
                    mc.isProcedural = true;
                    mc.mapSize = new Vector2(50, 50);
                    mc.tileSize = 1f;
                    mc.obstacleSpawnChance = 0.2f;
                    mc.collectibleSpawnChance = 0.1f;
                });
            }

            // ── BotConfig ───────────────────────────────────────────────

            BotConfig[] botConfigs = null;
            if (cfg.bots != BotOption.None)
            {
                var botConfig = CreateAsset<BotConfig>($"{soDir}/BotConfig.asset", bc =>
                {
                    bc.botName = "DefaultBot";
                    bc.moveSpeed = 5f;
                    bc.reactionTime = cfg.bots == BotOption.AdvancedAI ? 0.1f : 0.3f;
                    bc.errorMargin = cfg.bots == BotOption.AdvancedAI ? 0.2f : 0.6f;
                    bc.useRubberBanding = true;
                });
                botConfigs = new[] { botConfig };
            }

            // ── HapticConfig ────────────────────────────────────────────

            HapticConfig hapticConfig = null;
            if (cfg.enableHaptics)
            {
                hapticConfig = CreateAsset<HapticConfig>($"{soDir}/HapticConfig.asset", hc =>
                {
                    hc.enabled = true;
                });
            }

            // ── AnalyticsConfig ─────────────────────────────────────────

            AnalyticsConfig analyticsConfig = null;
            if (cfg.enableAnalytics)
            {
                analyticsConfig = CreateAsset<AnalyticsConfig>($"{soDir}/AnalyticsConfig.asset", ac =>
                {
                    ac.enabled = true;
                    ac.debugLogging = true;
                });
            }

            // ── RewardConfig ────────────────────────────────────────────

            RewardConfig[] rewardConfigs = null;
            if (cfg.enableRewards)
            {
                var levelReward = CreateAsset<RewardConfig>($"{soDir}/Rewards/LevelReward.asset", rc =>
                {
                    rc.rewards = new[]
                    {
                        new RewardEntry { Type = RewardType.SoftCurrency, Amount = 50 }
                    };
                });
                rewardConfigs = new[] { levelReward };
            }

            // ── DailyRewardConfig ───────────────────────────────────────

            DailyRewardConfig dailyRewardConfig = null;
            if (cfg.enableDailyRewards)
            {
                dailyRewardConfig = CreateAsset<DailyRewardConfig>($"{soDir}/DailyRewardConfig.asset", drc =>
                {
                    drc.cycleDays = 7;
                    drc.streakExpiryHours = 48;
                    drc.days = new DayReward[]
                    {
                        new DayReward { label = "Day 1", rewards = new[] { new RewardEntry { Type = RewardType.SoftCurrency, Amount = 100 } } },
                        new DayReward { label = "Day 2", rewards = new[] { new RewardEntry { Type = RewardType.SoftCurrency, Amount = 150 } } },
                        new DayReward { label = "Day 3", rewards = new[] { new RewardEntry { Type = RewardType.SoftCurrency, Amount = 200 } } },
                        new DayReward { label = "Day 4", rewards = new[] { new RewardEntry { Type = RewardType.SoftCurrency, Amount = 300 } } },
                        new DayReward { label = "Day 5", rewards = new[] { new RewardEntry { Type = RewardType.HardCurrency, Amount = 5 } } },
                        new DayReward { label = "Day 6", rewards = new[] { new RewardEntry { Type = RewardType.SoftCurrency, Amount = 500 } } },
                        new DayReward { label = "Day 7", rewards = new[] { new RewardEntry { Type = RewardType.HardCurrency, Amount = 15 } } },
                    };
                });
            }

            // ── TutorialConfig ──────────────────────────────────────────

            TutorialConfig tutorialConfig = null;
            if (cfg.enableTutorial)
            {
                tutorialConfig = CreateAsset<TutorialConfig>($"{soDir}/TutorialConfig.asset", tc =>
                {
                    tc.autoStart = true;
                    tc.allowSkip = true;
                    tc.steps = new TutorialStepData[]
                    {
                        new TutorialStepData { stepId = "welcome", title = "Welcome!", message = "Tap to continue.", completeOnTap = true },
                        new TutorialStepData { stepId = "gameplay", title = "How to Play", message = "Tap anywhere to play.", completeOnTap = true },
                    };
                });
            }

            // ── LeaderboardConfig ───────────────────────────────────────

            LeaderboardConfig leaderboardConfig = null;
            if (cfg.enableLeaderboards)
            {
                leaderboardConfig = CreateAsset<LeaderboardConfig>($"{soDir}/LeaderboardConfig.asset", lc =>
                {
                    lc.leaderboards = new LeaderboardDefinition[]
                    {
                        new LeaderboardDefinition { leaderboardId = "main", displayName = "High Scores", maxEntries = 100, descending = true }
                    };
                });
            }

            // ── AchievementConfig ───────────────────────────────────────

            AchievementConfig achievementConfig = null;
            if (cfg.enableAchievements)
            {
                achievementConfig = CreateAsset<AchievementConfig>($"{soDir}/AchievementConfig.asset", ac =>
                {
                    ac.achievements = new AchievementDefinition[]
                    {
                        new AchievementDefinition { achievementId = "first_win", displayName = "First Win", description = "Complete a level.", requiredProgress = 1 },
                        new AchievementDefinition { achievementId = "collector", displayName = "Collector", description = "Own 10 items.", requiredProgress = 10 },
                    };
                });
            }

            // ── ItemDatabase + Sample Items ─────────────────────────────

            ItemDatabase itemDatabase = null;
            if (cfg.store == StoreOption.Enabled)
            {
                string itemsDir = $"{soDir}/Items";
                CreateDir(itemsDir);

                var speedBoost = CreateAsset<ItemConfig>($"{itemsDir}/SpeedBoost.asset", ic =>
                {
                    ic.Id = "speed_boost";
                    ic.DisplayName = "Speed Boost";
                    ic.Description = "Increases speed for 10 seconds.";
                    ic.SoftCurrencyPrice = 50;
                    ic.Type = ItemType.Consumable;
                    ic.Category = "Consumable";
                    ic.IsStackable = true;
                    ic.EffectValue = 2f;
                    ic.EffectDuration = 10f;
                });

                var shield = CreateAsset<ItemConfig>($"{itemsDir}/Shield.asset", ic =>
                {
                    ic.Id = "shield";
                    ic.DisplayName = "Shield";
                    ic.Description = "Protects from one hit.";
                    ic.SoftCurrencyPrice = 100;
                    ic.Type = ItemType.Consumable;
                    ic.Category = "Consumable";
                    ic.IsStackable = true;
                });

                var coolSkin = CreateAsset<ItemConfig>($"{itemsDir}/CoolSkin.asset", ic =>
                {
                    ic.Id = "cool_skin";
                    ic.DisplayName = "Cool Skin";
                    ic.Description = "A stylish character skin.";
                    ic.HardCurrencyPrice = 5;
                    ic.Type = ItemType.Cosmetic;
                    ic.Category = "Cosmetic";
                    ic.IsEquippable = true;
                });

                // Create ItemDatabase and wire items via SerializedObject
                itemDatabase = CreateAsset<ItemDatabase>($"{soDir}/ItemDatabase.asset");
                var dbSO = new SerializedObject(itemDatabase);
                var itemsProp = dbSO.FindProperty("items");
                if (itemsProp != null)
                {
                    itemsProp.arraySize = 3;
                    itemsProp.GetArrayElementAtIndex(0).objectReferenceValue = speedBoost;
                    itemsProp.GetArrayElementAtIndex(1).objectReferenceValue = shield;
                    itemsProp.GetArrayElementAtIndex(2).objectReferenceValue = coolSkin;
                    dbSO.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            // ═══════════════════════════════════════════════════════════
            //  MASTER FRAMEWORK CONFIG — wires everything together
            // ═══════════════════════════════════════════════════════════

            var frameworkConfig = CreateAsset<FrameworkConfig>($"{soDir}/FrameworkConfig.asset", fc =>
            {
                // Core
                fc.gameConfig = gameConfig;
                fc.audioConfig = audioConfig;
                fc.economyConfig = economyConfig;

                // Events
                fc.onGameStart = onGameStart;
                fc.onGamePause = onGamePause;
                fc.onGameResume = onGameResume;
                fc.onGameComplete = onGameComplete;
                fc.onGameFail = onGameFail;
                fc.onLevelLoaded = onLevelLoaded;
                fc.onScoreChanged = onScoreChanged;
                fc.onCurrencyChanged = onCurrencyChanged;

                // Feature toggles (mirror wizard choices)
                fc.enableStore = cfg.store == StoreOption.Enabled;
                fc.enableHaptics = cfg.enableHaptics;
                fc.enableAnalytics = cfg.enableAnalytics;
                fc.enableRewards = cfg.enableRewards;
                fc.enableDailyRewards = cfg.enableDailyRewards;
                fc.enableTutorial = cfg.enableTutorial;
                fc.enableLeaderboards = cfg.enableLeaderboards;
                fc.enableAchievements = cfg.enableAchievements;
                fc.enablePopups = cfg.enablePopups;
                fc.enableBots = cfg.bots != BotOption.None;

                // Feature configs
                fc.itemDatabase = itemDatabase;
                fc.rewardConfigs = rewardConfigs;
                fc.hapticConfig = hapticConfig;
                fc.analyticsConfig = analyticsConfig;
                fc.dailyRewardConfig = dailyRewardConfig;
                fc.tutorialConfig = tutorialConfig;
                fc.leaderboardConfig = leaderboardConfig;
                fc.achievementConfig = achievementConfig;
                fc.botConfigs = botConfigs;

                // Map & Levels
                fc.levelConfigs = new[] { levelConfig };
                fc.mapConfig = mapConfig;
                fc.gameModeConfig = gameModeConfig;
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ProtoCasual] All configs, events, and FrameworkConfig created.");

            return frameworkConfig;
        }

        // ─── Helpers ────────────────────────────────────────────────────

        private static void CreateDir(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folder = Path.GetFileName(path);
                if (!AssetDatabase.IsValidFolder(parent))
                    CreateDir(parent);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }

        /// <summary>
        /// Creates or retrieves an existing asset. If already exists, re-applies config.
        /// Returns the asset reference so it can be wired into FrameworkConfig.
        /// </summary>
        private static T CreateAsset<T>(string assetPath, System.Action<T> configure = null) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existing != null)
            {
                configure?.Invoke(existing);
                EditorUtility.SetDirty(existing);
                return existing;
            }

            var asset = ScriptableObject.CreateInstance<T>();
            configure?.Invoke(asset);
            AssetDatabase.CreateAsset(asset, assetPath);
            return asset;
        }

        private static GameModeType ToGameModeType(GameType gt)
        {
            return gt switch
            {
                GameType.Puzzle => GameModeType.Puzzle,
                GameType.Racing => GameModeType.Racing,
                GameType.Endless => GameModeType.Endless,
                GameType.Hybrid => GameModeType.Hybrid,
                _ => GameModeType.HyperCasual,
            };
        }
    }
}
