using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.Utilities;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Central UI manager built on UI Toolkit.
    /// Replaces the legacy uGUI-based UIManager.
    ///
    /// Place on a GameObject with a UIDocument component.
    /// The Setup Wizard auto-wires all UXML and USS references.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class UIToolkitManager : Singleton<UIToolkitManager>
    {
        // ─── UIDocument ─────────────────────────────────────────────

        [Header("UI Document")]
        [SerializeField] private UIDocument uiDocument;

        // ─── Screen Layouts ─────────────────────────────────────────

        [Header("Screen Layouts")]
        [SerializeField] private VisualTreeAsset mainScreenLayout;
        [SerializeField] private VisualTreeAsset gameplayScreenLayout;
        [SerializeField] private VisualTreeAsset winScreenLayout;
        [SerializeField] private VisualTreeAsset loseScreenLayout;
        [SerializeField] private VisualTreeAsset pauseScreenLayout;
        [SerializeField] private VisualTreeAsset storeScreenLayout;
        [SerializeField] private VisualTreeAsset inventoryScreenLayout;
        [SerializeField] private VisualTreeAsset settingsScreenLayout;

        // ─── Popup Layouts ──────────────────────────────────────────

        [Header("Popup Layouts")]
        [SerializeField] private VisualTreeAsset confirmPopupLayout;
        [SerializeField] private VisualTreeAsset rewardPopupLayout;

        // ─── Styles ─────────────────────────────────────────────────

        [Header("Styles")]
        [SerializeField] private StyleSheet baseStyles;
        [SerializeField] private StyleSheet componentStyles;
        [SerializeField] private StyleSheet themeStyles;

        // ─── State-Screen Mapping ───────────────────────────────────

        [Header("State-Screen Mapping")]
        [SerializeField] private string menuScreenName = "MainScreen";
        [SerializeField] private string gameplayScreenName = "GameplayScreen";
        [SerializeField] private string winScreenName = "WinScreen";
        [SerializeField] private string loseScreenName = "LoseScreen";
        [SerializeField] private string pauseScreenName = "PauseScreen";

        // ─── Runtime ────────────────────────────────────────────────

        private readonly Dictionary<string, (VisualElement element, ScreenController controller)> screens = new();
        private ScreenController currentScreen;
        private VisualElement screenContainer;
        private VisualElement popupContainer;

        public PopupManager Popups { get; private set; }
        public ThemeManager Theme { get; private set; }
        public string CurrentScreenName => currentScreen?.ScreenName;

        // ─── Lifecycle ──────────────────────────────────────────────

        protected override void Awake()
        {
            base.Awake();
            if (uiDocument == null)
                uiDocument = GetComponent<UIDocument>();
        }

        /// <summary>
        /// Call from GameBootstrap after all services are registered.
        /// Builds the full visual tree, binds all controllers, wires state machine.
        /// </summary>
        public void Initialize()
        {
            var root = uiDocument.rootVisualElement;
            root.Clear();

            // Apply styles
            Theme = new ThemeManager(root);
            Theme.ApplyBaseStyles(baseStyles, componentStyles);
            if (themeStyles != null)
                Theme.ApplyTheme(themeStyles);

            // Create containers
            screenContainer = new VisualElement { name = "screen-container" };
            screenContainer.AddToClassList("screen-container");
            screenContainer.style.flexGrow = 1;
            root.Add(screenContainer);

            popupContainer = new VisualElement { name = "popup-container" };
            popupContainer.style.position = Position.Absolute;
            popupContainer.style.left = 0;
            popupContainer.style.right = 0;
            popupContainer.style.top = 0;
            popupContainer.style.bottom = 0;
            popupContainer.pickingMode = PickingMode.Ignore;
            root.Add(popupContainer);

            // Register all screens
            RegisterScreen("MainScreen", mainScreenLayout, new Screens.MainScreen());
            RegisterScreen("GameplayScreen", gameplayScreenLayout, new Screens.GameplayScreen());
            RegisterScreen("WinScreen", winScreenLayout, new Screens.WinScreen());
            RegisterScreen("LoseScreen", loseScreenLayout, new Screens.LoseScreen());
            RegisterScreen("PauseScreen", pauseScreenLayout, new Screens.PauseScreen());
            RegisterScreen("StoreScreen", storeScreenLayout, new Screens.StoreScreen());
            RegisterScreen("InventoryScreen", inventoryScreenLayout, new Screens.InventoryScreen());
            RegisterScreen("SettingsScreen", settingsScreenLayout, new Screens.SettingsScreen());

            // Register popups
            Popups = new PopupManager(popupContainer);
            if (confirmPopupLayout != null)
                Popups.Register("ConfirmPopup", confirmPopupLayout, new Popups.ConfirmPopup());
            if (rewardPopupLayout != null)
                Popups.Register("RewardPopup", rewardPopupLayout, new Popups.RewardPopup());

            // Listen to game state
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChange;

            // Show menu
            ShowScreen(menuScreenName);
        }

        private void RegisterScreen(string name, VisualTreeAsset layout, ScreenController controller)
        {
            if (layout == null) return;

            var instance = layout.Instantiate();
            instance.style.flexGrow = 1;
            instance.style.display = DisplayStyle.None;
            screenContainer.Add(instance);
            controller.Bind(instance);
            screens[name] = (instance, controller);
        }

        // ─── State Machine Listener ─────────────────────────────────

        private void HandleStateChange(GameState previous, GameState current)
        {
            switch (current)
            {
                case GameState.Menu:      ShowScreen(menuScreenName);     break;
                case GameState.Playing:   ShowScreen(gameplayScreenName); break;
                case GameState.Completed: ShowScreen(winScreenName);      break;
                case GameState.Failed:    ShowScreen(loseScreenName);     break;
                case GameState.Paused:    ShowScreen(pauseScreenName);    break;
            }
        }

        // ─── Screen Management ──────────────────────────────────────

        public void ShowScreen(string screenName)
        {
            if (string.IsNullOrEmpty(screenName)) return;
            if (!screens.TryGetValue(screenName, out var entry))
            {
                Debug.LogWarning($"[UIToolkitManager] Screen '{screenName}' not found!");
                return;
            }

            // Hide current
            if (currentScreen != null)
            {
                foreach (var kvp in screens)
                {
                    if (kvp.Value.controller == currentScreen)
                    {
                        kvp.Value.element.style.display = DisplayStyle.None;
                        break;
                    }
                }
                currentScreen.OnHide();
            }

            // Show target
            entry.element.style.display = DisplayStyle.Flex;
            entry.controller.OnShow();
            currentScreen = entry.controller;
        }

        public void ShowScreen<T>() where T : ScreenController
        {
            foreach (var kvp in screens)
            {
                if (kvp.Value.controller is T)
                {
                    ShowScreen(kvp.Key);
                    return;
                }
            }
        }

        public void HideScreen(string screenName)
        {
            if (screens.TryGetValue(screenName, out var entry))
            {
                entry.element.style.display = DisplayStyle.None;
                entry.controller.OnHide();
                if (currentScreen == entry.controller)
                    currentScreen = null;
            }
        }

        public T GetScreen<T>() where T : ScreenController
        {
            foreach (var kvp in screens)
                if (kvp.Value.controller is T typed) return typed;
            return null;
        }

        // ─── Popup Convenience ──────────────────────────────────────

        public void ShowPopup(string name, object data = null) => Popups?.Show(name, data);
        public void ShowPopup<T>(object data = null) where T : PopupController => Popups?.Show<T>(data);
        public void HidePopup(string name) => Popups?.Hide(name);
        public void HideTopPopup() => Popups?.HideTop();
        public void HideAllPopups() => Popups?.HideAll();

        // ─── Update ─────────────────────────────────────────────────

        private void Update()
        {
            currentScreen?.OnUpdate(Time.deltaTime);
        }
    }
}
