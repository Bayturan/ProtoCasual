using UnityEngine;
using UnityEditor;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Custom inspector for FrameworkConfig.
    /// Shows a clean overview with feature toggles that auto-show/hide config sections.
    /// </summary>
    [CustomEditor(typeof(FrameworkConfig))]
    public class FrameworkConfigEditor : UnityEditor.Editor
    {
        private bool showCore = true;
        private bool showEvents = true;
        private bool showFeatures = true;
        private bool showEconomy = true;
        private bool showOptional = true;
        private bool showMapLevels = true;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeader();

            showCore = DrawSection("Core Configuration", showCore, () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gameConfig"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("audioConfig"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("economyConfig"));
            });

            showEvents = DrawSection("Game Events", showEvents, () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGameStart"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGamePause"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGameResume"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGameComplete"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGameFail"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onLevelLoaded"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onScoreChanged"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onCurrencyChanged"));
            });

            showFeatures = DrawSection("Feature Toggles", showFeatures, () =>
            {
                DrawFeatureToggle("enableStore", "Store System");
                DrawFeatureToggle("enableHaptics", "Haptics");
                DrawFeatureToggle("enableAnalytics", "Analytics");
                DrawFeatureToggle("enableRewards", "Rewards");
                DrawFeatureToggle("enableDailyRewards", "Daily Rewards");
                DrawFeatureToggle("enableTutorial", "Tutorial");
                DrawFeatureToggle("enableLeaderboards", "Leaderboards");
                DrawFeatureToggle("enableAchievements", "Achievements");
                DrawFeatureToggle("enablePopups", "Popups");
                DrawFeatureToggle("enableBots", "Bots");
            });

            showEconomy = DrawSection("Store & Economy", showEconomy, () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("itemDatabase"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rewardConfigs"), true);
            });

            showOptional = DrawSection("Feature Configs", showOptional, () =>
            {
                var fc = (FrameworkConfig)target;

                if (fc.enableHaptics)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hapticConfig"));
                if (fc.enableAnalytics)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("analyticsConfig"));
                if (fc.enableDailyRewards)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("dailyRewardConfig"));
                if (fc.enableTutorial)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("tutorialConfig"));
                if (fc.enableLeaderboards)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("leaderboardConfig"));
                if (fc.enableAchievements)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("achievementConfig"));
                if (fc.enableBots)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("botConfigs"), true);

                if (!fc.enableHaptics && !fc.enableAnalytics && !fc.enableDailyRewards &&
                    !fc.enableTutorial && !fc.enableLeaderboards && !fc.enableAchievements && !fc.enableBots)
                {
                    EditorGUILayout.HelpBox("No optional features enabled. Toggle features above to see their configs here.", MessageType.Info);
                }
            });

            showMapLevels = DrawSection("Map & Levels", showMapLevels, () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gameModeConfig"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("levelConfigs"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mapConfig"));
            });

            serializedObject.ApplyModifiedProperties();

            // Validate button
            EditorGUILayout.Space(8);
            if (GUILayout.Button("Validate Configuration", GUILayout.Height(30)))
            {
                ConfigValidator.ValidateSetup();
            }
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(4);
            var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 };
            GUILayout.Label("ProtoCasual Framework Config", style);
            EditorGUILayout.HelpBox(
                "This is the master configuration for your game. " +
                "Assign this to GameBootstrap — everything else is auto-wired from here.",
                MessageType.Info);
            EditorGUILayout.Space(4);
        }

        private bool DrawSection(string title, bool isExpanded, System.Action drawContent)
        {
            EditorGUILayout.Space(2);
            isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isExpanded, title);
            if (isExpanded)
            {
                EditorGUI.indentLevel++;
                drawContent();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            return isExpanded;
        }

        private void DrawFeatureToggle(string propertyName, string label)
        {
            var prop = serializedObject.FindProperty(propertyName);
            if (prop != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(prop, new GUIContent(label));
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
