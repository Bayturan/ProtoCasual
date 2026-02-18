using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Utilities;
using System.Collections.Generic;

namespace ProtoCasual.Core.Managers
{
    /// <summary>
    /// Manages available game modes and handles switching between them.
    /// </summary>
    public class GameModeManager : Singleton<GameModeManager>
    {
        [Header("Available Game Modes")]
        [SerializeField] private GameModeBase[] availableGameModes;

        [Header("Default Mode")]
        [SerializeField] private int defaultModeIndex = 0;

        private Dictionary<string, GameModeBase> gameModeDict = new();
        private IGameMode currentGameMode;

        protected override void Awake()
        {
            base.Awake();
            InitializeGameModes();
        }

        private void InitializeGameModes()
        {
            if (availableGameModes == null || availableGameModes.Length == 0)
                availableGameModes = GetComponentsInChildren<GameModeBase>(true);

            gameModeDict.Clear();
            foreach (var mode in availableGameModes)
            {
                if (mode != null && !gameModeDict.ContainsKey(mode.ModeName))
                {
                    gameModeDict.Add(mode.ModeName, mode);
                    mode.gameObject.SetActive(false);
                }
            }

            if (availableGameModes.Length > 0 && defaultModeIndex >= 0 && defaultModeIndex < availableGameModes.Length)
                SetGameMode(availableGameModes[defaultModeIndex].ModeName);
        }

        public void SetGameMode(string modeName)
        {
            if (!gameModeDict.TryGetValue(modeName, out GameModeBase mode))
            {
                Debug.LogError($"[GameModeManager] Game mode '{modeName}' not found!");
                return;
            }

            if (currentGameMode != null)
            {
                currentGameMode.Cleanup();
                if (currentGameMode is GameModeBase prevMode)
                    prevMode.gameObject.SetActive(false);
            }

            currentGameMode = mode;
            mode.gameObject.SetActive(true);
            GameManager.Instance.SetGameMode(currentGameMode);
        }

        public void SetGameModeByIndex(int index)
        {
            if (availableGameModes != null && index >= 0 && index < availableGameModes.Length)
                SetGameMode(availableGameModes[index].ModeName);
        }

        public IGameMode GetCurrentGameMode() => currentGameMode;

        public string[] GetAvailableGameModeNames()
        {
            var names = new string[availableGameModes.Length];
            for (int i = 0; i < availableGameModes.Length; i++)
                names[i] = availableGameModes[i].ModeName;
            return names;
        }
    }
}
