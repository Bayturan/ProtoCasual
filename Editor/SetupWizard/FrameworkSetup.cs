using UnityEditor;
using UnityEngine;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Full-featured Game Setup Wizard.
    /// Menu: ProtoCasual → Create New Game
    ///
    /// Step 1 – Configuration questions (game type, map, bots, monetization, store, input, platform)
    /// Step 2 – One-click generation of folders, scenes, managers, configs, UI prefabs, build settings.
    /// </summary>
    public class FrameworkSetupWindow : EditorWindow
    {
        private GameSetupConfig config = new();
        private Vector2 scroll;
        private bool confirmed;

        // ─── Menu item ──────────────────────────────────────────────────

        [MenuItem("ProtoCasual/Create New Game", priority = 0)]
        public static void ShowWindow()
        {
            var w = GetWindow<FrameworkSetupWindow>("ProtoCasual Setup Wizard");
            w.minSize = new Vector2(440, 680);
        }

        // Keep old entry point too
        [MenuItem("ProtoCasual/Setup Scene (Quick)", priority = 1)]
        public static void QuickSetup()
        {
            var cfg = new GameSetupConfig();
            RunSetup(cfg);
        }

        // ─── GUI ────────────────────────────────────────────────────────

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.Space(8);

            DrawHeader();
            EditorGUILayout.Space(6);
            DrawConfigSection();
            EditorGUILayout.Space(12);
            DrawActions();
            EditorGUILayout.Space(8);
            DrawPostSetupNotes();

            EditorGUILayout.EndScrollView();
        }

        // ─── Header ────────────────────────────────────────────────────

        private void DrawHeader()
        {
            GUILayout.Label("Proto Casual — Game Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Configure your game settings below, then click \"Generate Project\" " +
                "to create the full project structure, scenes, managers, configs, and UI.",
                MessageType.Info);
        }

        // ─── Configuration fields ──────────────────────────────────────

        private void DrawConfigSection()
        {
            EditorGUI.BeginChangeCheck();

            config.gameName = EditorGUILayout.TextField("Game Name", config.gameName);
            EditorGUILayout.Space(4);

            DrawEnumSection("🎮 Game Type", ref config.gameType);
            DrawEnumSection("🗺  Map Type", ref config.mapType);
            DrawEnumSection("🤖 Bots", ref config.bots);
            DrawEnumSection("💰 Monetization", ref config.monetization);
            DrawEnumSection("🏪 Store", ref config.store);
            DrawEnumSection("🎯 Input Type", ref config.inputType);
            DrawEnumSection("📱 Platform", ref config.platform);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Optional Systems", EditorStyles.boldLabel);
            config.enableHaptics = EditorGUILayout.Toggle("📳 Haptics", config.enableHaptics);
            config.enableAnalytics = EditorGUILayout.Toggle("📊 Analytics", config.enableAnalytics);
            config.enableRewards = EditorGUILayout.Toggle("🎁 Reward System", config.enableRewards);
            config.enableDailyRewards = EditorGUILayout.Toggle("📅 Daily Rewards", config.enableDailyRewards);
            config.enableTutorial = EditorGUILayout.Toggle("📖 Tutorial System", config.enableTutorial);
            config.enableLeaderboards = EditorGUILayout.Toggle("🏆 Leaderboards", config.enableLeaderboards);
            config.enableAchievements = EditorGUILayout.Toggle("🏅 Achievements", config.enableAchievements);
            config.enablePopups = EditorGUILayout.Toggle("💬 Popup System", config.enablePopups);

            if (EditorGUI.EndChangeCheck())
                confirmed = false;
        }

        private void DrawEnumSection<T>(string label, ref T value) where T : System.Enum
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(140));
            value = (T)(object)EditorGUILayout.EnumPopup((System.Enum)(object)value);
            EditorGUILayout.EndHorizontal();
        }

        // ─── Action buttons ────────────────────────────────────────────

        private void DrawActions()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Summary preview
            EditorGUILayout.LabelField("Setup Summary", EditorStyles.boldLabel);
            DrawSummaryLine("Game Type", config.gameType.ToString());
            DrawSummaryLine("Map Type", config.mapType.ToString());
            DrawSummaryLine("Bots", config.bots.ToString());
            DrawSummaryLine("Monetization", config.monetization.ToString());
            DrawSummaryLine("Store", config.store.ToString());
            DrawSummaryLine("Input", config.inputType.ToString());
            DrawSummaryLine("Platform", config.platform.ToString());
            DrawSummaryLine("Haptics", config.enableHaptics ? "✓" : "—");
            DrawSummaryLine("Analytics", config.enableAnalytics ? "✓" : "—");
            DrawSummaryLine("Rewards", config.enableRewards ? "✓" : "—");
            DrawSummaryLine("Daily Rewards", config.enableDailyRewards ? "✓" : "—");
            DrawSummaryLine("Tutorial", config.enableTutorial ? "✓" : "—");
            DrawSummaryLine("Leaderboards", config.enableLeaderboards ? "✓" : "—");
            DrawSummaryLine("Achievements", config.enableAchievements ? "✓" : "—");
            DrawSummaryLine("Popups", config.enablePopups ? "✓" : "—");

            int managerCount = CountManagers(config);
            DrawSummaryLine("Scenes", "2 (Main + InGame)");
            DrawSummaryLine("Managers", managerCount.ToString());

            EditorGUILayout.Space(8);

            // Confirm checkbox
            confirmed = EditorGUILayout.ToggleLeft("I confirm the settings above are correct", confirmed);
            EditorGUILayout.Space(4);

            GUI.enabled = confirmed;
            var btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            if (GUILayout.Button("Generate Project", btnStyle, GUILayout.Height(40)))
            {
                RunSetup(config);
            }
            GUI.enabled = true;
        }

        private void DrawSummaryLine(string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(120));
            EditorGUILayout.LabelField(value, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

        // ─── Post-setup notes ──────────────────────────────────────────

        private void DrawPostSetupNotes()
        {
            EditorGUILayout.HelpBox(
                "After generation:\n" +
                "1. Assign GameConfig SO to Bootstrap (in both scenes)\n" +
                "2. Add your GameMode component under GameModes\n" +
                "3. Assign Screen references in UIManager\n" +
                "4. Add prefabs used by your levels / map config\n" +
                "5. Press Play — Menu → Start → Win/Lose flow works out of the box",
                MessageType.None);
        }

        // ─── Execute ───────────────────────────────────────────────────

        private static void RunSetup(GameSetupConfig cfg)
        {
            Debug.Log("═══════════════════════════════════════════════════");
            Debug.Log("[ProtoCasual] Starting project generation…");
            Debug.Log("═══════════════════════════════════════════════════");

            // Step 1 — Folders
            ProjectStructureGenerator.CreateFolders(cfg);

            // Step 2 — Default ScriptableObjects
            ProjectStructureGenerator.CreateDefaultConfigs(cfg);

            // Step 3 — UI Prefabs
            UIPrefabGenerator.Generate(cfg);

            // Step 4 — Scenes (Main + InGame)
            SceneBuilder.BuildAll(cfg);

            // Step 5 — Open Main scene
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");

            // Step 6 — Summary
            PrintSummary(cfg);
        }

        private static void PrintSummary(GameSetupConfig cfg)
        {
            int managerCount = CountManagers(cfg);

            Debug.Log("═══════════════════════════════════════════════════");
            Debug.Log("[ProtoCasual] SETUP COMPLETE");
            Debug.Log("───────────────────────────────────────────────────");
            Debug.Log($"  Game Name     : {cfg.gameName}");
            Debug.Log($"  Game Type     : {cfg.gameType}");
            Debug.Log($"  Map Type      : {cfg.mapType}");
            Debug.Log($"  Bots          : {cfg.bots}");
            Debug.Log($"  Monetization  : {cfg.monetization}");
            Debug.Log($"  Store         : {cfg.store}");
            Debug.Log($"  Input         : {cfg.inputType}");
            Debug.Log($"  Platform      : {cfg.platform}");
            Debug.Log($"  Haptics       : {cfg.enableHaptics}");
            Debug.Log($"  Analytics     : {cfg.enableAnalytics}");
            Debug.Log($"  Rewards       : {cfg.enableRewards}");
            Debug.Log($"  Daily Rewards : {cfg.enableDailyRewards}");
            Debug.Log($"  Tutorial      : {cfg.enableTutorial}");
            Debug.Log($"  Leaderboards  : {cfg.enableLeaderboards}");
            Debug.Log($"  Achievements  : {cfg.enableAchievements}");
            Debug.Log($"  Popups        : {cfg.enablePopups}");
            Debug.Log($"  Scenes Created: 2 (Main + InGame)");
            Debug.Log($"  Managers      : {managerCount}");
            Debug.Log("═══════════════════════════════════════════════════");
        }

        private static int CountManagers(GameSetupConfig cfg)
        {
            // Base: GameManager, GameModeManager, AudioManager, LevelManager, SaveService, InputManager = 6
            // + Services (non-MonoBehaviour): CurrencyService, InventoryService, StoreService, PlayerDataProvider = 4
            int count = 10;
            if (cfg.monetization == MonetizationType.AdsOnly || cfg.monetization == MonetizationType.AdsPlusIAP) count++;
            if (cfg.monetization == MonetizationType.IAPOnly || cfg.monetization == MonetizationType.AdsPlusIAP) count++;
            if (cfg.enableHaptics) count++;
            if (cfg.enableAnalytics) count++;
            if (cfg.enableRewards) count++;
            if (cfg.enableDailyRewards) count++;
            if (cfg.enableTutorial) count++;
            if (cfg.enableLeaderboards) count++;
            if (cfg.enableAchievements) count++;
            if (cfg.enablePopups) count++;
            return count;
        }
    }
}

