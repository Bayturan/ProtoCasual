using UnityEditor;
using UnityEngine;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.Systems;
using ProtoCasual.Core.UI;

namespace ProtoCasual.Editor
{
    public class FrameworkSetupWindow : EditorWindow
    {
        private bool createBootstrap = true;
        private bool createManagers = true;
        private bool createUI = true;
        private bool createSystems = true;

        [MenuItem("ProtoCasual/Setup Scene", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<FrameworkSetupWindow>("ProtoCasual Setup");
            window.minSize = new Vector2(350, 400);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("ProtoCasual Framework Setup", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This wizard creates the default scene hierarchy for a ProtoCasual game.\n" +
                "Select which systems to create:",
                MessageType.Info);
            EditorGUILayout.Space(10);

            createBootstrap = EditorGUILayout.ToggleLeft("Bootstrap (GameBootstrap)", createBootstrap);
            createManagers = EditorGUILayout.ToggleLeft("Managers (Game, Audio, Level, Economy, Inventory, Equipment)", createManagers);
            createUI = EditorGUILayout.ToggleLeft("UI System (UIManager + Screens)", createUI);
            createSystems = EditorGUILayout.ToggleLeft("Systems (InputManager, MapGenerator)", createSystems);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Create Scene Structure", GUILayout.Height(35)))
            {
                SetupScene();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "After setup:\n" +
                "1. Assign GameConfig ScriptableObject to Bootstrap\n" +
                "2. Add GameMode components to GameModes object\n" +
                "3. Assign UI Screen references in UIManager\n" +
                "4. Configure ScriptableObject configs in Resources/",
                MessageType.None);
        }

        private void SetupScene()
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("ProtoCasual Scene Setup");

            if (createBootstrap)
                CreateBootstrap();
            if (createManagers)
                CreateManagers();
            if (createUI)
                CreateUISystem();
            if (createSystems)
                CreateSystems();

            Debug.Log("[ProtoCasual] Scene structure created successfully!");
        }

        private void CreateBootstrap()
        {
            var bootstrap = CreateGameObject("--- BOOTSTRAP ---");
            Undo.AddComponent<GameBootstrap>(bootstrap);
        }

        private void CreateManagers()
        {
            var managers = CreateGameObject("--- MANAGERS ---");

            var gameManager = CreateChild("GameManager", managers.transform);
            Undo.AddComponent<GameManager>(gameManager);

            var gameModeManager = CreateChild("GameModeManager", managers.transform);
            Undo.AddComponent<GameModeManager>(gameModeManager);

            var audioManager = CreateChild("AudioManager", managers.transform);
            Undo.AddComponent<AudioManager>(audioManager);

            var levelManager = CreateChild("LevelManager", managers.transform);
            Undo.AddComponent<LevelManager>(levelManager);

            var economyManager = CreateChild("EconomyManager", managers.transform);
            Undo.AddComponent<EconomyManager>(economyManager);

            var inventoryManager = CreateChild("InventoryManager", managers.transform);
            Undo.AddComponent<InventoryManager>(inventoryManager);

            var equipmentManager = CreateChild("EquipmentManager", managers.transform);
            Undo.AddComponent<EquipmentManager>(equipmentManager);

            var saveService = CreateChild("SaveService", managers.transform);
            Undo.AddComponent<SaveService>(saveService);
        }

        private void CreateUISystem()
        {
            var ui = CreateGameObject("--- UI ---");
            Undo.AddComponent<UIManager>(ui);

            var canvas = CreateChild("Canvas", ui.transform);
            Undo.AddComponent<Canvas>(canvas);
            var canvasComponent = canvas.GetComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            Undo.AddComponent<UnityEngine.UI.CanvasScaler>(canvas);
            Undo.AddComponent<UnityEngine.UI.GraphicRaycaster>(canvas);

            string[] screenNames = { "MenuScreen", "GameplayScreen", "WinScreen", "LoseScreen", "PauseScreen" };
            foreach (var screenName in screenNames)
            {
                var screen = CreateChild(screenName, canvas.transform);
                Undo.AddComponent<UIScreen>(screen);
            }
        }

        private void CreateSystems()
        {
            var systems = CreateGameObject("--- SYSTEMS ---");

            var inputManager = CreateChild("InputManager", systems.transform);
            Undo.AddComponent<InputManager>(inputManager);

            var mapGenerator = CreateChild("MapGenerator", systems.transform);
            Undo.AddComponent<MapGenerator>(mapGenerator);

            CreateChild("GameModes", systems.transform);
        }

        private static GameObject CreateGameObject(string name)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
            return go;
        }

        private static GameObject CreateChild(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
            return go;
        }
    }
}
