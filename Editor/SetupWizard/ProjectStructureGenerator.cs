using UnityEngine;
using UnityEditor;
using System.IO;
using ProtoCasual.Core.ScriptableObjects;

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
