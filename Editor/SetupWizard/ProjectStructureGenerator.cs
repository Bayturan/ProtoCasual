using UnityEngine;
using UnityEditor;
using System.IO;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Creates the Assets folder structure and default ScriptableObject configs
    /// for a new ProtoCasual project.
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
            CreateDir($"{root}/ScriptableObjects");
            CreateDir($"{root}/UI");
            CreateDir($"{root}/Materials");
            CreateDir($"{root}/Audio");
            CreateDir($"{root}/Animations");
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

        // ─── ScriptableObject creation ──────────────────────────────────

        public static void CreateDefaultConfigs(GameSetupConfig cfg)
        {
            string soDir = "Assets/_Game/ScriptableObjects";
            CreateDir(soDir);

            // GameConfig
            CreateAssetIfMissing<GameConfig>($"{soDir}/GameConfig.asset", gc =>
            {
                gc.targetFPS = 60;
                gc.soundEnabled = true;
            });

            // GameModeConfig
            CreateAssetIfMissing<GameModeConfig>($"{soDir}/GameModeConfig.asset", gmc =>
            {
                gmc.mode = ToGameModeType(cfg.gameType);
                gmc.modeName = cfg.gameType.ToString();
                gmc.description = $"Default {cfg.gameType} game mode configuration.";
            });

            // LevelConfig
            CreateAssetIfMissing<LevelConfig>($"{soDir}/LevelConfig.asset", lc =>
            {
                lc.levelName = "Level 1";
                lc.levelIndex = 0;
            });

            // MapConfig (only for procedural / endless)
            if (cfg.mapType == MapType.Procedural || cfg.mapType == MapType.EndlessGeneration)
            {
                CreateAssetIfMissing<MapConfig>($"{soDir}/MapConfig.asset", mc =>
                {
                    mc.mapName = "Default Map";
                    mc.isProcedural = true;
                    mc.mapSize = new Vector2(50, 50);
                    mc.tileSize = 1f;
                    mc.obstacleSpawnChance = 0.2f;
                    mc.collectibleSpawnChance = 0.1f;
                });
            }

            // BotConfig (only when bots enabled)
            if (cfg.bots != BotOption.None)
            {
                CreateAssetIfMissing<BotConfig>($"{soDir}/BotConfig.asset", bc =>
                {
                    bc.botName = "DefaultBot";
                    bc.moveSpeed = 5f;
                    bc.reactionTime = cfg.bots == BotOption.AdvancedAI ? 0.1f : 0.3f;
                    bc.errorMargin = cfg.bots == BotOption.AdvancedAI ? 0.2f : 0.6f;
                    bc.useRubberBanding = true;
                });
            }

            // HapticConfig
            if (cfg.enableHaptics)
            {
                CreateAssetIfMissing<HapticConfig>($"{soDir}/HapticConfig.asset", hc =>
                {
                    hc.enabled = true;
                });
            }

            // AnalyticsConfig
            if (cfg.enableAnalytics)
            {
                CreateAssetIfMissing<AnalyticsConfig>($"{soDir}/AnalyticsConfig.asset", ac =>
                {
                    ac.enabled = true;
                    ac.debugLogging = true;
                });
            }

            // DailyRewardConfig
            if (cfg.enableDailyRewards)
            {
                CreateAssetIfMissing<DailyRewardConfig>($"{soDir}/DailyRewardConfig.asset", drc =>
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

            // TutorialConfig
            if (cfg.enableTutorial)
            {
                CreateAssetIfMissing<TutorialConfig>($"{soDir}/TutorialConfig.asset", tc =>
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

            // LeaderboardConfig
            if (cfg.enableLeaderboards)
            {
                CreateAssetIfMissing<LeaderboardConfig>($"{soDir}/LeaderboardConfig.asset", lc =>
                {
                    lc.leaderboards = new LeaderboardDefinition[]
                    {
                        new LeaderboardDefinition { leaderboardId = "main", displayName = "High Scores", maxEntries = 100, descending = true }
                    };
                });
            }

            // AchievementConfig
            if (cfg.enableAchievements)
            {
                CreateAssetIfMissing<AchievementConfig>($"{soDir}/AchievementConfig.asset", ac =>
                {
                    ac.achievements = new AchievementDefinition[]
                    {
                        new AchievementDefinition { achievementId = "first_win", displayName = "First Win", description = "Complete a level.", requiredProgress = 1 },
                        new AchievementDefinition { achievementId = "collector", displayName = "Collector", description = "Own 10 items.", requiredProgress = 10 },
                    };
                });
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ProtoCasual] Default ScriptableObject configs created.");
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

        private static void CreateAssetIfMissing<T>(string assetPath, System.Action<T> configure = null) where T : ScriptableObject
        {
            if (AssetDatabase.LoadAssetAtPath<T>(assetPath) != null) return;

            var asset = ScriptableObject.CreateInstance<T>();
            configure?.Invoke(asset);
            AssetDatabase.CreateAsset(asset, assetPath);
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
