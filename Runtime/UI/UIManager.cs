using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.Utilities;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Manages UI screens. Listens to GameManager state changes for automatic screen transitions.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [Header("Screens")]
        [SerializeField] private UIScreen[] screens;

        [Header("State-Screen Mapping")]
        [SerializeField] private string menuScreenName = "MenuScreen";
        [SerializeField] private string gameplayScreenName = "GameplayScreen";
        [SerializeField] private string winScreenName = "WinScreen";
        [SerializeField] private string loseScreenName = "LoseScreen";
        [SerializeField] private string pauseScreenName = "PauseScreen";

        private Dictionary<string, UIScreen> screenDictionary = new();
        private UIScreen currentScreen;

        protected override void Awake()
        {
            base.Awake();
            RegisterScreens();
        }

        private void RegisterScreens()
        {
            if (screens == null || screens.Length == 0)
                screens = GetComponentsInChildren<UIScreen>(true);

            screenDictionary.Clear();
            foreach (var screen in screens)
            {
                if (screen != null && !screenDictionary.ContainsKey(screen.ScreenName))
                {
                    screenDictionary.Add(screen.ScreenName, screen);
                    screen.Initialize();
                    screen.Hide();
                }
            }
        }

        /// <summary>
        /// Call after GameManager is ready. Subscribes to state changes and shows menu.
        /// </summary>
        public void Initialize()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChange;
            ShowScreen(menuScreenName);
        }

        private void HandleStateChange(GameState previous, GameState current)
        {
            switch (current)
            {
                case GameState.Menu: ShowScreen(menuScreenName); break;
                case GameState.Playing: ShowScreen(gameplayScreenName); break;
                case GameState.Completed: ShowScreen(winScreenName); break;
                case GameState.Failed: ShowScreen(loseScreenName); break;
                case GameState.Paused: ShowScreen(pauseScreenName); break;
            }
        }

        public void ShowScreen(string screenName)
        {
            if (string.IsNullOrEmpty(screenName)) return;
            if (!screenDictionary.TryGetValue(screenName, out UIScreen screen))
            {
                Debug.LogWarning($"[UIManager] Screen '{screenName}' not found!");
                return;
            }

            currentScreen?.Hide();
            screen.Show();
            currentScreen = screen;
        }

        public void HideScreen(string screenName)
        {
            if (screenDictionary.TryGetValue(screenName, out UIScreen screen))
            {
                screen.Hide();
                if (currentScreen == screen)
                {
                    currentScreen = null;
                }
            }
        }

        public void HideCurrentScreen()
        {
            currentScreen?.Hide();
            currentScreen = null;
        }

        public T GetScreen<T>() where T : UIScreen
        {
            return screens.OfType<T>().FirstOrDefault();
        }
    }
}
