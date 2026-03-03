using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.Systems;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.UI;

namespace ProtoCasual.Editor
{
    /// <summary>
    /// Creates Main (menu) and InGame scenes with full hierarchy,
    /// UIDocument + UIToolkitManager with UXML/USS auto-wired from package.
    /// </summary>
    public static class SceneBuilder
    {
        private const string PKG_UI = "Packages/com.bayturan.protocasual/Runtime/UI";

        // ═══════════════════════════════════════════════════════════════
        //  PUBLIC API
        // ═══════════════════════════════════════════════════════════════

        public static void BuildAll(GameSetupConfig cfg, FrameworkConfig frameworkConfig)
        {
            BuildMainScene(cfg, frameworkConfig);
            BuildInGameScene(cfg, frameworkConfig);
            AddScenesToBuildSettings();
            Debug.Log("[ProtoCasual] Scenes created and added to Build Settings.");
        }

        // ═══════════════════════════════════════════════════════════════
        //  MAIN SCENE
        // ═══════════════════════════════════════════════════════════════

        public static void BuildMainScene(GameSetupConfig cfg, FrameworkConfig frameworkConfig)
        {
            string path = "Assets/Scenes/Main.unity";
            if (SceneExistsAndSkip(path)) return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // ── Bootstrap ──
            var bootstrapGO = CreateGO("Bootstrap");
            var bootstrap = bootstrapGO.AddComponent<GameBootstrap>();

            // ── Managers ──
            var managers = CreateGO("Managers");
            CreateChild("GameManager", managers).AddComponent<GameManager>();
            CreateChild("GameModeManager", managers).AddComponent<GameModeManager>();
            CreateChild("AudioManager", managers).AddComponent<AudioManager>();
            CreateChild("LevelManager", managers).AddComponent<LevelManager>();
            CreateChild("SaveService", managers).AddComponent<SaveService>();

            if (cfg.monetization == MonetizationType.AdsOnly || cfg.monetization == MonetizationType.AdsPlusIAP)
                CreateChild("AdsManager", managers);
            if (cfg.monetization == MonetizationType.IAPOnly || cfg.monetization == MonetizationType.AdsPlusIAP)
                CreateChild("IAPManager", managers);

            CreateChild("InputManager", managers).AddComponent<InputManager>();

            // ── GameModes ──
            CreateGO("GameModes");

            // ── UI (UI Toolkit) ──
            var uiGO = CreateGO("UI");
            var uiDoc = uiGO.AddComponent<UIDocument>();
            var panelSettings = GetOrCreatePanelSettings();
            uiDoc.panelSettings = panelSettings;
            var uiManager = uiGO.AddComponent<UIToolkitManager>();

            WireUIToolkitManager(uiManager, cfg);
            WireFrameworkConfig(bootstrap, frameworkConfig);

            // ── Save ──
            EnsureDir("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[ProtoCasual] Main scene saved → {path}");
        }

        // ═══════════════════════════════════════════════════════════════
        //  INGAME SCENE
        // ═══════════════════════════════════════════════════════════════

        public static void BuildInGameScene(GameSetupConfig cfg, FrameworkConfig frameworkConfig)
        {
            string path = "Assets/Scenes/InGame.unity";
            if (SceneExistsAndSkip(path)) return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // ── Bootstrap ──
            var bootstrapGO = CreateGO("Bootstrap");
            var bootstrap = bootstrapGO.AddComponent<GameBootstrap>();

            // ── GameWorld ──
            var gameWorld = CreateGO("GameWorld");
            var spawnPoint = CreateChild("PlayerSpawnPoint", gameWorld);
            spawnPoint.transform.position = new Vector3(0, 1, 0);

            switch (cfg.gameType)
            {
                case GameType.Racing:
                    CreateChild("TrackGenerator", gameWorld);
                    if (cfg.bots != BotOption.None) CreateChild("BotSpawner", gameWorld);
                    break;
                case GameType.Puzzle:
                    CreateChild("GridSystem", gameWorld);
                    CreateChild("MatchLogic", gameWorld);
                    break;
                case GameType.Endless:
                    CreateChild("EndlessGenerator", gameWorld).AddComponent<EndlessGenerator>();
                    break;
                case GameType.HyperCasual:
                    CreateChild("LevelLoader", gameWorld);
                    break;
                case GameType.Hybrid:
                    CreateChild("LevelLoader", gameWorld);
                    if (cfg.mapType == MapType.Procedural || cfg.mapType == MapType.EndlessGeneration)
                        CreateChild("MapGenerator", gameWorld).AddComponent<MapGenerator>();
                    break;
            }

            if ((cfg.mapType == MapType.Procedural || cfg.mapType == MapType.EndlessGeneration)
                && gameWorld.transform.Find("MapGenerator") == null
                && gameWorld.transform.Find("EndlessGenerator") == null)
            {
                CreateChild("MapGenerator", gameWorld).AddComponent<MapGenerator>();
            }

            if (cfg.bots != BotOption.None && gameWorld.transform.Find("BotSpawner") == null)
                CreateChild("BotSpawner", gameWorld);

            // ── Managers ──
            var managers = CreateGO("Managers");
            CreateChild("GameManager", managers).AddComponent<GameManager>();
            CreateChild("GameModeManager", managers).AddComponent<GameModeManager>();
            CreateChild("AudioManager", managers).AddComponent<AudioManager>();
            CreateChild("LevelManager", managers).AddComponent<LevelManager>();
            CreateChild("SaveService", managers).AddComponent<SaveService>();
            CreateChild("InputManager", managers).AddComponent<InputManager>();

            // ── GameModes ──
            CreateGO("GameModes");

            // ── UI (UI Toolkit) ──
            var uiGO = CreateGO("UI");
            var uiDoc = uiGO.AddComponent<UIDocument>();
            uiDoc.panelSettings = GetOrCreatePanelSettings();
            var uiManager = uiGO.AddComponent<UIToolkitManager>();

            WireUIToolkitManager(uiManager, cfg);
            WireFrameworkConfig(bootstrap, frameworkConfig);

            // ── Save ──
            EnsureDir("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[ProtoCasual] InGame scene saved → {path}");
        }

        // ═══════════════════════════════════════════════════════════════
        //  AUTO-WIRING
        // ═══════════════════════════════════════════════════════════════

        private static void WireUIToolkitManager(UIToolkitManager manager, GameSetupConfig cfg)
        {
            var so = new SerializedObject(manager);

            // Screens
            SetAsset<VisualTreeAsset>(so, "mainScreenLayout",      $"{PKG_UI}/UXML/MainScreen.uxml");
            SetAsset<VisualTreeAsset>(so, "gameplayScreenLayout",   $"{PKG_UI}/UXML/GameplayScreen.uxml");
            SetAsset<VisualTreeAsset>(so, "winScreenLayout",        $"{PKG_UI}/UXML/WinScreen.uxml");
            SetAsset<VisualTreeAsset>(so, "loseScreenLayout",       $"{PKG_UI}/UXML/LoseScreen.uxml");
            SetAsset<VisualTreeAsset>(so, "pauseScreenLayout",      $"{PKG_UI}/UXML/PauseScreen.uxml");
            SetAsset<VisualTreeAsset>(so, "settingsScreenLayout",   $"{PKG_UI}/UXML/SettingsScreen.uxml");

            if (cfg.store == StoreOption.Enabled)
            {
                SetAsset<VisualTreeAsset>(so, "storeScreenLayout",     $"{PKG_UI}/UXML/StoreScreen.uxml");
                SetAsset<VisualTreeAsset>(so, "inventoryScreenLayout",  $"{PKG_UI}/UXML/InventoryScreen.uxml");
            }

            // Popups
            if (cfg.enablePopups)
            {
                SetAsset<VisualTreeAsset>(so, "confirmPopupLayout", $"{PKG_UI}/UXML/ConfirmPopup.uxml");
                SetAsset<VisualTreeAsset>(so, "rewardPopupLayout",  $"{PKG_UI}/UXML/RewardPopup.uxml");
            }

            // Styles
            SetAsset<StyleSheet>(so, "baseStyles",      $"{PKG_UI}/USS/ProtoCasual-Base.uss");
            SetAsset<StyleSheet>(so, "componentStyles",  $"{PKG_UI}/USS/ProtoCasual-Components.uss");

            // Theme (prefer project copy, fallback to package)
            string themeSrc = GetThemeFileName(cfg.gameType);
            string projectTheme = "Assets/_Game/UI/Themes/GameTheme.uss";
            string packageTheme = $"{PKG_UI}/USS/Themes/{themeSrc}.uss";

            var themeAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>(projectTheme)
                          ?? AssetDatabase.LoadAssetAtPath<StyleSheet>(packageTheme);
            var themeProp = so.FindProperty("themeStyles");
            if (themeProp != null) themeProp.objectReferenceValue = themeAsset;

            so.ApplyModifiedPropertiesWithoutUndo();
            Debug.Log("[ProtoCasual] UIToolkitManager auto-wired (UXML + USS).");
        }

        private static void WireFrameworkConfig(GameBootstrap bootstrap, FrameworkConfig config)
        {
            if (bootstrap == null || config == null) return;
            var so = new SerializedObject(bootstrap);
            var prop = so.FindProperty("frameworkConfig");
            if (prop != null)
            {
                prop.objectReferenceValue = config;
                so.ApplyModifiedPropertiesWithoutUndo();
                Debug.Log("[ProtoCasual] FrameworkConfig auto-wired to GameBootstrap.");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  PANEL SETTINGS
        // ═══════════════════════════════════════════════════════════════

        private static PanelSettings GetOrCreatePanelSettings()
        {
            string path = "Assets/_Game/UI/GamePanelSettings.asset";
            var existing = AssetDatabase.LoadAssetAtPath<PanelSettings>(path);
            if (existing != null) return existing;

            EnsureDir("Assets/_Game/UI");
            var ps = ScriptableObject.CreateInstance<PanelSettings>();
            ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            ps.referenceResolution = new Vector2Int(1080, 1920);
            ps.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            ps.match = 0.5f;
            AssetDatabase.CreateAsset(ps, path);
            Debug.Log("[ProtoCasual] PanelSettings created for mobile scaling.");
            return ps;
        }

        // ═══════════════════════════════════════════════════════════════
        //  BUILD SETTINGS
        // ═══════════════════════════════════════════════════════════════

        public static void AddScenesToBuildSettings()
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            TryAddScene(scenes, "Assets/Scenes/Main.unity");
            TryAddScene(scenes, "Assets/Scenes/InGame.unity");
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static void TryAddScene(System.Collections.Generic.List<EditorBuildSettingsScene> scenes, string path)
        {
            foreach (var s in scenes) if (s.path == path) return;
            if (System.IO.File.Exists(path))
                scenes.Add(new EditorBuildSettingsScene(path, true));
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════

        private static string GetThemeFileName(GameType type) => type switch
        {
            GameType.HyperCasual => "HyperCasual",
            GameType.Puzzle      => "Puzzle",
            GameType.Racing      => "Racing",
            GameType.Endless     => "Endless",
            GameType.Hybrid      => "Hybrid",
            _                    => "HyperCasual"
        };

        private static void SetAsset<T>(SerializedObject so, string propName, string assetPath) where T : Object
        {
            var prop = so.FindProperty(propName);
            if (prop != null)
                prop.objectReferenceValue = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        private static bool SceneExistsAndSkip(string path)
        {
            if (System.IO.File.Exists(path))
            {
                Debug.LogWarning($"[ProtoCasual] Scene already exists, skipping: {path}");
                return true;
            }
            return false;
        }

        private static GameObject CreateGO(string name) => new GameObject(name);

        private static GameObject CreateChild(string name, GameObject parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform);
            return go;
        }

        private static void EnsureDir(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                var folder = System.IO.Path.GetFileName(path);
                if (!AssetDatabase.IsValidFolder(parent)) EnsureDir(parent);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }
    }
}
