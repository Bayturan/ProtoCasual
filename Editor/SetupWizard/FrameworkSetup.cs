using UnityEditor;
using UnityEngine;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Full-featured Game Setup Wizard with 3-page flow.
    /// Menu: ProtoCasual → Create New Game
    ///
    /// Page 1 – Core Settings (game type, map, bots, monetization, store, input, platform)
    /// Page 2 – Feature Toggles (haptics, analytics, rewards, daily rewards, tutorial, etc.)
    /// Page 3 – Review & Generate (summary + one-click generation)
    /// </summary>
    public class FrameworkSetupWindow : EditorWindow
    {
        private GameSetupConfig config = new();
        private Vector2 scroll;
        private int currentPage;
        private const int TotalPages = 3;

        // ─── Menu items ─────────────────────────────────────────────────

        [MenuItem("ProtoCasual/Create New Game", priority = 0)]
        public static void ShowWindow()
        {
            var w = GetWindow<FrameworkSetupWindow>("ProtoCasual Setup Wizard");
            w.minSize = new Vector2(460, 700);
        }

        [MenuItem("ProtoCasual/Setup Scene (Quick)", priority = 1)]
        public static void QuickSetup()
        {
            if (!EditorUtility.DisplayDialog(
                    "Quick Setup",
                    "This will create a project with default settings.\nAre you sure?",
                    "Generate", "Cancel"))
                return;

            RunSetup(new GameSetupConfig());
        }

        // ─── GUI ────────────────────────────────────────────────────────

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.Space(8);

            DrawHeader();
            DrawPageIndicator();
            EditorGUILayout.Space(6);

            switch (currentPage)
            {
                case 0: DrawPage1_CoreSettings(); break;
                case 1: DrawPage2_Features(); break;
                case 2: DrawPage3_ReviewGenerate(); break;
            }

            EditorGUILayout.Space(12);
            DrawNavigation();
            EditorGUILayout.Space(8);

            EditorGUILayout.EndScrollView();
        }

        // ─── Header ────────────────────────────────────────────────────

        private void DrawHeader()
        {
            var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 15 };
            GUILayout.Label("Proto Casual — Game Setup Wizard", style);
            EditorGUILayout.HelpBox(
                "Configure your game in 3 easy steps. " +
                "Everything will be auto-generated and wired — just add game mechanics and art.",
                MessageType.Info);
        }

        // ─── Page Indicator ─────────────────────────────────────────────

        private void DrawPageIndicator()
        {
            EditorGUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string[] labels = { "1. Core", "2. Features", "3. Generate" };
            for (int i = 0; i < TotalPages; i++)
            {
                var s = new GUIStyle(EditorStyles.miniButton)
                {
                    fontStyle = i == currentPage ? FontStyle.Bold : FontStyle.Normal
                };
                GUI.enabled = true;
                if (GUILayout.Button(labels[i], s, GUILayout.Width(100)))
                    currentPage = i;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(4);
        }

        // ════════════════════════════════════════════════════════════════
        //  PAGE 1 — Core Settings
        // ════════════════════════════════════════════════════════════════

        private void DrawPage1_CoreSettings()
        {
            EditorGUILayout.LabelField("Core Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            config.gameName = EditorGUILayout.TextField("Game Name", config.gameName);
            EditorGUILayout.Space(4);

            DrawEnumWithHelp("Game Type", ref config.gameType,
                "HyperCasual: Simple one-tap games\n" +
                "Hybrid: Level-based with depth\n" +
                "Puzzle: Grid / match mechanics\n" +
                "Racing: Track-based with vehicles\n" +
                "Endless: Infinite runner / procedural");

            DrawEnumWithHelp("Map Type", ref config.mapType,
                "FixedLevels: Hand-crafted levels\n" +
                "Procedural: Generated at runtime\n" +
                "EndlessGeneration: Infinite scrolling chunks");

            DrawEnumWithHelp("Bots", ref config.bots,
                "None: No AI opponents\n" +
                "SimpleAI: Basic pathfinding\n" +
                "AdvancedAI: Rubber-banding + smart behaviour");

            DrawEnumWithHelp("Monetization", ref config.monetization,
                "None: No ads or IAP\n" +
                "AdsOnly: Rewarded + interstitial ads\n" +
                "IAPOnly: In-app purchases only\n" +
                "AdsPlusIAP: Both ads and IAP");

            DrawEnumWithHelp("Store", ref config.store,
                "Enabled: Item store + inventory system\n" +
                "Disabled: No store");

            DrawEnumWithHelp("Input Type", ref config.inputType,
                "Tap / Swipe / Drag / Steering / Mixed");

            DrawEnumWithHelp("Platform", ref config.platform,
                "Android / iOS / Both");
        }

        private void DrawEnumWithHelp<T>(string label, ref T value, string helpText) where T : System.Enum
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(120));
            value = (T)(object)EditorGUILayout.EnumPopup((System.Enum)(object)value);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(helpText, EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        // ════════════════════════════════════════════════════════════════
        //  PAGE 2 — Feature Toggles
        // ════════════════════════════════════════════════════════════════

        private void DrawPage2_Features()
        {
            EditorGUILayout.LabelField("Feature Toggles", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Enable the systems you need. Disabled features won't create configs or register services.",
                MessageType.Info);
            EditorGUILayout.Space(4);

            DrawFeatureToggle(ref config.enableHaptics, "Haptics",
                "Vibration feedback on game events.");
            DrawFeatureToggle(ref config.enableAnalytics, "Analytics",
                "Event tracking (debug logger by default, replace with your SDK).");
            DrawFeatureToggle(ref config.enableRewards, "Reward System",
                "Level completion rewards, ad rewards, etc.");
            DrawFeatureToggle(ref config.enableDailyRewards, "Daily Rewards",
                "7-day login reward cycle with streak tracking.");
            DrawFeatureToggle(ref config.enableTutorial, "Tutorial",
                "Step-by-step tutorial system with skip option.");
            DrawFeatureToggle(ref config.enableLeaderboards, "Leaderboards",
                "Local leaderboard (replace with online service).");
            DrawFeatureToggle(ref config.enableAchievements, "Achievements",
                "Progress-based achievements with rewards.");
            DrawFeatureToggle(ref config.enablePopups, "Popup System",
                "Confirm dialogs, reward popups, etc.");
        }

        private void DrawFeatureToggle(ref bool value, string label, string description)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            value = EditorGUILayout.ToggleLeft($"  {label}", value, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(description, EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(1);
        }

        // ════════════════════════════════════════════════════════════════
        //  PAGE 3 — Review & Generate
        // ════════════════════════════════════════════════════════════════

        private void DrawPage3_ReviewGenerate()
        {
            EditorGUILayout.LabelField("Review & Generate", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            // Summary
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawSummaryLine("Game Name", config.gameName);
            DrawSummaryLine("Game Type", config.gameType.ToString());
            DrawSummaryLine("Map Type", config.mapType.ToString());
            DrawSummaryLine("Bots", config.bots.ToString());
            DrawSummaryLine("Monetization", config.monetization.ToString());
            DrawSummaryLine("Store", config.store.ToString());
            DrawSummaryLine("Input", config.inputType.ToString());
            DrawSummaryLine("Platform", config.platform.ToString());
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(4);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Enabled Features", EditorStyles.boldLabel);
            DrawSummaryLine("Haptics", config.enableHaptics ? "YES" : "—");
            DrawSummaryLine("Analytics", config.enableAnalytics ? "YES" : "—");
            DrawSummaryLine("Rewards", config.enableRewards ? "YES" : "—");
            DrawSummaryLine("Daily Rewards", config.enableDailyRewards ? "YES" : "—");
            DrawSummaryLine("Tutorial", config.enableTutorial ? "YES" : "—");
            DrawSummaryLine("Leaderboards", config.enableLeaderboards ? "YES" : "—");
            DrawSummaryLine("Achievements", config.enableAchievements ? "YES" : "—");
            DrawSummaryLine("Popups", config.enablePopups ? "YES" : "—");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(4);

            int configCount = CountConfigs(config);
            int uiCount = CountUIPrefabs(config);
            DrawSummaryLine("Scenes", "2 (Main + InGame)");
            DrawSummaryLine("Config Assets", configCount.ToString());
            DrawSummaryLine("UI Assets", uiCount.ToString());
            DrawSummaryLine("Game Events", "8");

            EditorGUILayout.Space(12);

            // Generate button
            var btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            if (GUILayout.Button("Generate Project", btnStyle, GUILayout.Height(44)))
            {
                RunSetup(config);
            }

            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                "After generation, everything is wired automatically.\n" +
                "Just add your GameMode script, game mechanics, and art assets!",
                MessageType.None);
        }

        private void DrawSummaryLine(string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(130));
            EditorGUILayout.LabelField(value, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

        // ─── Navigation ────────────────────────────────────────────────

        private void DrawNavigation()
        {
            GUILayout.BeginHorizontal();

            GUI.enabled = currentPage > 0;
            if (GUILayout.Button("← Back", GUILayout.Height(30)))
                currentPage--;

            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField($"Page {currentPage + 1} / {TotalPages}",
                new GUIStyle(EditorStyles.centeredGreyMiniLabel));

            GUILayout.FlexibleSpace();

            GUI.enabled = currentPage < TotalPages - 1;
            if (GUILayout.Button("Next →", GUILayout.Height(30)))
                currentPage++;

            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        // ─── Execute ───────────────────────────────────────────────────

        private static void RunSetup(GameSetupConfig cfg)
        {
            Debug.Log("═══════════════════════════════════════════════════");
            Debug.Log("[ProtoCasual] Starting project generation…");
            Debug.Log("═══════════════════════════════════════════════════");

            try
            {
                // Step 1 — Folders
                EditorUtility.DisplayProgressBar("ProtoCasual Setup", "Creating folders…", 0.1f);
                ProjectStructureGenerator.CreateFolders(cfg);

                // Step 2 — Default ScriptableObjects + FrameworkConfig
                EditorUtility.DisplayProgressBar("ProtoCasual Setup", "Creating configs & events…", 0.3f);
                var frameworkConfig = ProjectStructureGenerator.CreateDefaultConfigs(cfg);

                // Step 3 — UI Prefabs
                EditorUtility.DisplayProgressBar("ProtoCasual Setup", "Generating UI prefabs…", 0.5f);
                UIPrefabGenerator.Generate(cfg);

                // Step 4 — Scenes (Main + InGame) with auto-wiring
                EditorUtility.DisplayProgressBar("ProtoCasual Setup", "Building scenes…", 0.7f);
                SceneBuilder.BuildAll(cfg, frameworkConfig);

                // Step 5 — Open Main scene
                EditorUtility.DisplayProgressBar("ProtoCasual Setup", "Finalizing…", 0.9f);
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");

                // Step 6 — Summary
                PrintSummary(cfg);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            // Completion dialog
            EditorUtility.DisplayDialog(
                "ProtoCasual — Setup Complete!",
                $"Project \"{cfg.gameName}\" is ready!\n\n" +
                "• All configs created and wired to FrameworkConfig\n" +
                "• GameBootstrap has FrameworkConfig assigned\n" +
                "• UIToolkitManager UXML/USS auto-wired\n" +
                $"• Game-type UI: {cfg.gameType} MainScreen + GameplayScreen\n" +
                "• 8 GameEvent assets created\n" +
                "• Theme USS copied to _Game/UI/Themes/ for customisation\n\n" +
                "Next steps:\n" +
                "1. Add your GameMode script under GameModes\n" +
                "2. Add game mechanics and art\n" +
                "3. Edit _Game/UI/Themes/GameTheme.uss to reskin UI\n" +
                "4. Customize configs in _Game/ScriptableObjects\n" +
                "5. Press Play!",
                "OK");
        }

        private static void PrintSummary(GameSetupConfig cfg)
        {
            int configCount = CountConfigs(cfg);

            Debug.Log("═══════════════════════════════════════════════════");
            Debug.Log("[ProtoCasual] SETUP COMPLETE");
            Debug.Log("───────────────────────────────────────────────────");
            Debug.Log($"  Game Name     : {cfg.gameName}");
            Debug.Log($"  Game Type     : {cfg.gameType}");
            Debug.Log($"  UI Variant    : {cfg.gameType} (MainScreen + GameplayScreen)");
            Debug.Log($"  Map Type      : {cfg.mapType}");
            Debug.Log($"  Bots          : {cfg.bots}");
            Debug.Log($"  Monetization  : {cfg.monetization}");
            Debug.Log($"  Store         : {cfg.store}");
            Debug.Log($"  Input         : {cfg.inputType}");
            Debug.Log($"  Platform      : {cfg.platform}");
            Debug.Log($"  Configs       : {configCount}");
            Debug.Log($"  Game Events   : 8");
            Debug.Log($"  Scenes        : 2 (Main + InGame)");
            Debug.Log("═══════════════════════════════════════════════════");
        }

        // ─── Counting helpers ───────────────────────────────────────────

        private static int CountConfigs(GameSetupConfig cfg)
        {
            // Always: GameConfig, AudioConfig, EconomyConfig, GameModeConfig, LevelConfig, FrameworkConfig = 6
            int count = 6;
            if (cfg.mapType == MapType.Procedural || cfg.mapType == MapType.EndlessGeneration) count++; // MapConfig
            if (cfg.bots != BotOption.None) count++; // BotConfig
            if (cfg.enableHaptics) count++;
            if (cfg.enableAnalytics) count++;
            if (cfg.enableRewards) count++;
            if (cfg.enableDailyRewards) count++;
            if (cfg.enableTutorial) count++;
            if (cfg.enableLeaderboards) count++;
            if (cfg.enableAchievements) count++;
            if (cfg.store == StoreOption.Enabled) count += 4; // ItemDatabase + 3 sample items
            return count;
        }

        private static int CountUIPrefabs(GameSetupConfig cfg)
        {
            // Game-type MainScreen + GameplayScreen = 2 UXML
            // Shared: WinScreen, LoseScreen, PauseScreen, SettingsScreen = 4 UXML
            // + Base USS + Components USS + Theme USS = 3 stylesheets
            int count = 9;
            if (cfg.store == StoreOption.Enabled) count += 2; // StoreScreen, InventoryScreen UXML
            if (cfg.enablePopups) count += 2; // ConfirmPopup, RewardPopup UXML
            return count;
        }
    }
}