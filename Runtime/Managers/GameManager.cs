using UnityEngine;
using System;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Events;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Utilities;

namespace ProtoCasual.Core.Managers
{
    /// <summary>
    /// Central game state machine. Controls state transitions and notifies listeners via events.
    /// Uses Singleton<T> to avoid manual boilerplate.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("Events")]
        [SerializeField] private GameEvent onGameStart;
        [SerializeField] private GameEvent onGamePause;
        [SerializeField] private GameEvent onGameResume;
        [SerializeField] private GameEvent onGameComplete;
        [SerializeField] private GameEvent onGameFail;

        public GameState CurrentState { get; private set; } = GameState.Boot;
        public event Action<GameState, GameState> OnStateChanged;

        private IGameMode currentGameMode;
        private float gameTime;
        private bool isGameRunning;
        private bool isTransitioning;

        private void Update()
        {
            if (isGameRunning)
            {
                gameTime += Time.deltaTime;
                currentGameMode?.UpdateMode(Time.deltaTime);
            }
        }

        /// <summary>
        /// Main state transition method. All state changes flow through here.
        /// Protected against re-entrant calls.
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState || isTransitioning) return;

            isTransitioning = true;
            var previousState = CurrentState;
            CurrentState = newState;

            HandleStateExit(previousState);
            HandleStateEnter(newState);
            OnStateChanged?.Invoke(previousState, newState);

            isTransitioning = false;
        }

        private void HandleStateExit(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    isGameRunning = false;
                    break;
                case GameState.Paused:
                    Time.timeScale = 1f;
                    break;
            }
        }

        private void HandleStateEnter(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    gameTime = CurrentState == GameState.Paused ? gameTime : 0f;
                    isGameRunning = true;
                    currentGameMode?.OnGameStart();
                    onGameStart?.Raise();
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    currentGameMode?.OnGamePause();
                    onGamePause?.Raise();
                    break;
                case GameState.Completed:
                    currentGameMode?.OnGameComplete();
                    onGameComplete?.Raise();
                    break;
                case GameState.Failed:
                    currentGameMode?.OnGameFail();
                    onGameFail?.Raise();
                    break;
            }
        }

        public void SetGameMode(IGameMode gameMode)
        {
            currentGameMode?.Cleanup();
            currentGameMode = gameMode;
            currentGameMode?.Initialize();
        }

        public void Play() => ChangeState(GameState.Playing);
        public void Pause() => ChangeState(GameState.Paused);
        public void Resume() => ChangeState(GameState.Playing);
        public void Complete() => ChangeState(GameState.Completed);
        public void Fail() => ChangeState(GameState.Failed);

        public void Restart()
        {
            Time.timeScale = 1f;
            currentGameMode?.Cleanup();
            currentGameMode?.Initialize();
            ChangeState(GameState.Playing);
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            currentGameMode?.Cleanup();
            ChangeState(GameState.Menu);
        }

        public float GetGameTime() => gameTime;
        public IGameMode GetCurrentGameMode() => currentGameMode;
    }
}