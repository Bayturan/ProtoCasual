using UnityEngine;
using UnityEditor;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Bootstrap;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Validates FrameworkConfig and scene setup to catch missing references.
    /// Menu: ProtoCasual > Validate Setup
    /// </summary>
    public static class ConfigValidator
    {
        [MenuItem("ProtoCasual/Validate Setup", priority = 10)]
        public static void ValidateSetup()
        {
            int errors = 0;
            int warnings = 0;

            Debug.Log("═══════════════════════════════════════════════════");
            Debug.Log("[ProtoCasual] Validating project setup...");
            Debug.Log("═══════════════════════════════════════════════════");

            // Find FrameworkConfig
            var guids = AssetDatabase.FindAssets("t:FrameworkConfig");
            if (guids.Length == 0)
            {
                LogError(ref errors, "No FrameworkConfig found! Run ProtoCasual > Create New Game.");
                PrintResult(errors, warnings);
                return;
            }

            var configPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var config = AssetDatabase.LoadAssetAtPath<FrameworkConfig>(configPath);

            if (config == null)
            {
                LogError(ref errors, "FrameworkConfig asset is corrupted.");
                PrintResult(errors, warnings);
                return;
            }

            Debug.Log($"  Found FrameworkConfig at: {configPath}");

            // Core configs
            if (config.gameConfig == null) LogError(ref errors, "GameConfig is missing!");
            if (config.audioConfig == null) LogWarn(ref warnings, "AudioConfig is missing. Audio won't auto-configure.");
            if (config.economyConfig == null) LogWarn(ref warnings, "EconomyConfig is missing. Starting currency won't apply.");

            // Events
            if (config.onGameStart == null) LogWarn(ref warnings, "OnGameStart event is not assigned.");
            if (config.onGameComplete == null) LogWarn(ref warnings, "OnGameComplete event is not assigned.");
            if (config.onGameFail == null) LogWarn(ref warnings, "OnGameFail event is not assigned.");

            // Feature-specific configs
            if (config.enableStore && config.itemDatabase == null)
                LogWarn(ref warnings, "Store is enabled but ItemDatabase is not assigned.");

            if (config.enableHaptics && config.hapticConfig == null)
                LogWarn(ref warnings, "Haptics enabled but HapticConfig is not assigned.");

            if (config.enableAnalytics && config.analyticsConfig == null)
                LogWarn(ref warnings, "Analytics enabled but AnalyticsConfig is not assigned.");

            if (config.enableDailyRewards && config.dailyRewardConfig == null)
                LogError(ref errors, "Daily Rewards enabled but DailyRewardConfig is not assigned.");

            if (config.enableTutorial && config.tutorialConfig == null)
                LogError(ref errors, "Tutorial enabled but TutorialConfig is not assigned.");

            if (config.enableLeaderboards && config.leaderboardConfig == null)
                LogError(ref errors, "Leaderboards enabled but LeaderboardConfig is not assigned.");

            if (config.enableAchievements && config.achievementConfig == null)
                LogError(ref errors, "Achievements enabled but AchievementConfig is not assigned.");

            // Check scenes
            if (!System.IO.File.Exists("Assets/Scenes/Main.unity"))
                LogError(ref errors, "Main scene not found at Assets/Scenes/Main.unity");

            if (!System.IO.File.Exists("Assets/Scenes/InGame.unity"))
                LogError(ref errors, "InGame scene not found at Assets/Scenes/InGame.unity");

            // Check GameBootstrap in current scene
            var bootstrap = Object.FindAnyObjectByType<GameBootstrap>();
            if (bootstrap != null)
            {
                var so = new SerializedObject(bootstrap);
                var fwProp = so.FindProperty("frameworkConfig");
                if (fwProp == null || fwProp.objectReferenceValue == null)
                    LogError(ref errors, "GameBootstrap in current scene has no FrameworkConfig assigned!");
                else
                    Debug.Log("  GameBootstrap: FrameworkConfig is assigned.");
            }
            else
            {
                LogWarn(ref warnings, "No GameBootstrap found in current scene.");
            }

            PrintResult(errors, warnings);
        }

        private static void LogError(ref int count, string msg)
        {
            Debug.LogError($"[Validate] ERROR: {msg}");
            count++;
        }

        private static void LogWarn(ref int count, string msg)
        {
            Debug.LogWarning($"[Validate] WARNING: {msg}");
            count++;
        }

        private static void PrintResult(int errors, int warnings)
        {
            Debug.Log("═══════════════════════════════════════════════════");
            if (errors == 0 && warnings == 0)
                Debug.Log("[ProtoCasual] Validation PASSED! Everything looks good.");
            else if (errors == 0)
                Debug.Log($"[ProtoCasual] Validation passed with {warnings} warning(s).");
            else
                Debug.LogError($"[ProtoCasual] Validation FAILED: {errors} error(s), {warnings} warning(s).");
            Debug.Log("═══════════════════════════════════════════════════");
        }
    }
}
