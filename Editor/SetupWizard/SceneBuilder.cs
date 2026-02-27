using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    /// Creates the Main (menu) and InGame scenes with full hierarchy, components,
    /// auto-wired FrameworkConfig on GameBootstrap, and auto-wired UIManager screens.
    /// </summary>
    public static class SceneBuilder
    {
        // ═══════════════════════════════════════════════════════════════
        //  PUBLIC API
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Builds both scenes. Receives FrameworkConfig from ProjectStructureGenerator
        /// and auto-wires it to every GameBootstrap in each scene.
        /// </summary>
        public static void BuildAll(GameSetupConfig cfg, FrameworkConfig frameworkConfig)
        {
            BuildMainScene(cfg, frameworkConfig);
            BuildInGameScene(cfg, frameworkConfig);
            AddScenesToBuildSettings();
            Debug.Log("[ProtoCasual] Scenes created and added to Build Settings.");
        }

        // ═══════════════════════════════════════════════════════════════
        //  MAIN SCENE  (Menu)
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

            var gm = CreateChild("GameManager", managers);
            gm.AddComponent<GameManager>();

            var gmm = CreateChild("GameModeManager", managers);
            gmm.AddComponent<GameModeManager>();

            var audio = CreateChild("AudioManager", managers);
            audio.AddComponent<AudioManager>();

            var level = CreateChild("LevelManager", managers);
            level.AddComponent<LevelManager>();

            var save = CreateChild("SaveService", managers);
            save.AddComponent<SaveService>();

            // Monetization managers
            if (cfg.monetization == MonetizationType.AdsOnly || cfg.monetization == MonetizationType.AdsPlusIAP)
                CreateChild("AdsManager", managers);
            if (cfg.monetization == MonetizationType.IAPOnly || cfg.monetization == MonetizationType.AdsPlusIAP)
                CreateChild("IAPManager", managers);

            // Input
            var inputGO = CreateChild("InputManager", managers);
            inputGO.AddComponent<InputManager>();

            // ── GameModes container ──
            CreateGO("GameModes");

            // ── Canvas (UI) ──
            var canvasGO = CreateCanvas("Canvas");
            var uiManager = canvasGO.AddComponent<UIManager>();

            CreateScreenChild("MenuScreen", canvasGO, typeof(MenuScreen));
            CreateScreenChild("SettingsScreen", canvasGO, typeof(SettingsScreen));
            if (cfg.store == StoreOption.Enabled)
            {
                CreateScreenChild("StoreScreen", canvasGO, typeof(StoreScreen));
                CreateScreenChild("InventoryScreen", canvasGO, typeof(InventoryScreen));
            }

            // ── Popup Canvas (higher sort order) ──
            if (cfg.enablePopups)
            {
                var popupCanvas = CreateCanvas("PopupCanvas");
                popupCanvas.GetComponent<Canvas>().sortingOrder = 200;
                popupCanvas.AddComponent<PopupManager>();
                CreateScreenChild("ConfirmPopup", popupCanvas, typeof(ConfirmPopup));
                CreateScreenChild("RewardPopup", popupCanvas, typeof(RewardPopup));
            }

            // ── EventSystem ──
            EnsureEventSystem();

            // ── Auto-wire ──
            WireFrameworkConfig(bootstrap, frameworkConfig);
            WireUIManagerScreens(uiManager);

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

            // Player spawn point
            var spawnPoint = CreateChild("PlayerSpawnPoint", gameWorld);
            spawnPoint.transform.position = new Vector3(0, 1, 0);

            // Game-type specifics
            switch (cfg.gameType)
            {
                case GameType.Racing:
                    CreateChild("TrackGenerator", gameWorld);
                    if (cfg.bots != BotOption.None)
                        CreateChild("BotSpawner", gameWorld);
                    break;

                case GameType.Puzzle:
                    CreateChild("GridSystem", gameWorld);
                    CreateChild("MatchLogic", gameWorld);
                    break;

                case GameType.Endless:
                    var endlessGen = CreateChild("EndlessGenerator", gameWorld);
                    endlessGen.AddComponent<EndlessGenerator>();
                    break;

                case GameType.HyperCasual:
                    CreateChild("LevelLoader", gameWorld);
                    break;

                case GameType.Hybrid:
                    CreateChild("LevelLoader", gameWorld);
                    if (cfg.mapType == MapType.Procedural || cfg.mapType == MapType.EndlessGeneration)
                    {
                        var mapGen = CreateChild("MapGenerator", gameWorld);
                        mapGen.AddComponent<MapGenerator>();
                    }
                    break;
            }

            // Procedural map generator
            if (cfg.mapType == MapType.Procedural || cfg.mapType == MapType.EndlessGeneration)
            {
                if (gameWorld.transform.Find("MapGenerator") == null &&
                    gameWorld.transform.Find("EndlessGenerator") == null)
                {
                    var mg = CreateChild("MapGenerator", gameWorld);
                    mg.AddComponent<MapGenerator>();
                }
            }

            // Bots
            if (cfg.bots != BotOption.None && gameWorld.transform.Find("BotSpawner") == null)
                CreateChild("BotSpawner", gameWorld);

            // ── Managers ──
            var managers = CreateGO("Managers");

            var gm = CreateChild("GameManager", managers);
            gm.AddComponent<GameManager>();

            var gmm = CreateChild("GameModeManager", managers);
            gmm.AddComponent<GameModeManager>();

            var audioGO = CreateChild("AudioManager", managers);
            audioGO.AddComponent<AudioManager>();

            var levelGO = CreateChild("LevelManager", managers);
            levelGO.AddComponent<LevelManager>();

            var saveGO = CreateChild("SaveService", managers);
            saveGO.AddComponent<SaveService>();

            var inputGO = CreateChild("InputManager", managers);
            inputGO.AddComponent<InputManager>();

            // ── GameModes container ──
            CreateGO("GameModes");

            // ── Canvas (UI) ──
            var canvasGO = CreateCanvas("Canvas");
            var uiManager = canvasGO.AddComponent<UIManager>();

            CreateScreenChild("GameplayScreen", canvasGO, typeof(GameplayScreen));
            CreateScreenChild("PauseScreen", canvasGO, typeof(PauseScreen));
            CreateScreenChild("WinScreen", canvasGO, typeof(WinScreen));
            CreateScreenChild("LoseScreen", canvasGO, typeof(LoseScreen));

            // ── EventSystem ──
            EnsureEventSystem();

            // ── Auto-wire ──
            WireFrameworkConfig(bootstrap, frameworkConfig);
            WireUIManagerScreens(uiManager);

            // ── Save ──
            EnsureDir("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[ProtoCasual] InGame scene saved → {path}");
        }

        // ═══════════════════════════════════════════════════════════════
        //  AUTO-WIRING
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Assigns FrameworkConfig to GameBootstrap's serialized field using SerializedObject API.
        /// </summary>
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
            else
            {
                Debug.LogWarning("[ProtoCasual] Could not find 'frameworkConfig' field on GameBootstrap.");
            }
        }

        /// <summary>
        /// Auto-discovers UIScreen children and assigns them to UIManager's screens array.
        /// </summary>
        private static void WireUIManagerScreens(UIManager uiManager)
        {
            if (uiManager == null) return;

            var screens = uiManager.GetComponentsInChildren<UIScreen>(true);
            if (screens.Length == 0) return;

            var so = new SerializedObject(uiManager);
            var prop = so.FindProperty("screens");
            if (prop != null && prop.isArray)
            {
                prop.arraySize = screens.Length;
                for (int i = 0; i < screens.Length; i++)
                {
                    prop.GetArrayElementAtIndex(i).objectReferenceValue = screens[i];
                }
                so.ApplyModifiedPropertiesWithoutUndo();
                Debug.Log($"[ProtoCasual] UIManager: {screens.Length} screens auto-wired.");
            }
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
            foreach (var s in scenes)
                if (s.path == path) return;

            if (System.IO.File.Exists(path))
                scenes.Add(new EditorBuildSettingsScene(path, true));
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════

        private static bool SceneExistsAndSkip(string path)
        {
            if (System.IO.File.Exists(path))
            {
                Debug.LogWarning($"[ProtoCasual] Scene already exists, skipping: {path}");
                return true;
            }
            return false;
        }

        private static GameObject CreateGO(string name)
        {
            var go = new GameObject(name);
            return go;
        }

        private static GameObject CreateChild(string name, GameObject parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform);
            return go;
        }

        private static GameObject CreateCanvas(string name)
        {
            var go = new GameObject(name);

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        private static void CreateScreenChild(string name, GameObject canvas, System.Type screenType)
        {
            var go = new GameObject(name);
            go.transform.SetParent(canvas.transform, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            if (screenType != null && typeof(UIScreen).IsAssignableFrom(screenType))
                go.AddComponent(screenType);
            else
                go.AddComponent<UIScreen>();

            go.SetActive(false);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }
        }

        private static void EnsureDir(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                var folder = System.IO.Path.GetFileName(path);
                if (!AssetDatabase.IsValidFolder(parent))
                    EnsureDir(parent);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }
    }
}
