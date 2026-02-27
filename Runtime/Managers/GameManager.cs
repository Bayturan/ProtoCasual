using UnityEngine;
using System;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Events;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Utilities;

namespace ProtoCasual.Core.Managers
{
    /// <summary>
    /// Central game state machine. Controls state transitions, score tracking,
    /// and notifies listeners via GameEvent ScriptableObjects and C# events.
    /// Events are wired automatically by GameBootstrap from FrameworkConfig.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        // ─── Events (wired by GameBootstrap) ────────────────────────────

        private GameEvent onGameStart;
        private GameEvent onGamePause;
        private GameEvent onGameResume;
        private GameEvent onGameComplete;
        private GameEvent onGameFail;

        // ─── State ──────────────────────────────────────────────────────

        public GameState CurrentState { get; private set; } = GameState.Boot;
        public event Action<GameState, GameState> OnStateChanged;

        // ─── Score ──────────────────────────────────────────────────────

        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;

        // ─── Properties ─────────────────────────────────────────────────

        public float GameTime => gameTime;
        public bool IsGameRunning => isGameRunning;
        public IGameMode CurrentGameMode => currentGameMode;

        private IGameMode currentGameMode;
        private float gameTime;
        private bool isGameRunning;
        private bool isTransitioning;

        // ─── Framework Event Wiring ─────────────────────────────────────

        /// <summary>
        /// Called by GameBootstrap to wire GameEvent SOs from FrameworkConfig.
        /// No manual assignment needed.
        /// </summary>
        public void SetFrameworkEvents(
            GameEvent start, GameEvent pause, GameEvent resume,
            GameEvent complete, GameEvent fail)
        {
            onGameStart = start;
            onGamePause = pause;
            onGameResume = resume;
            onGameComplete = complete;
            onGameFail = fail;
        }

        // ─── Update ─────────────────────────────────────────────────────

        private void Update()
        {
            if (isGameRunning)
            {
                gameTime += Time.deltaTime;
                currentGameMode?.UpdateMode(Time.deltaTime);
            }
        }

        // ─── State Machine ──────────────────────────────────────────────

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
            HandleStateEnter(newState, previousState);
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

        private void HandleStateEnter(GameState state, GameState previousState)
        {
            switch (state)
            {
                case GameState.Playing:
                    bool isResuming = previousState == GameState.Paused;
                    if (!isResuming)
                    {
                        gameTime = 0f;
                        CurrentScore = 0;
                    }
                    isGameRunning = true;
                    if (isResuming)
                    {
                        currentGameMode?.OnGameResume();
                        onGameResume?.Raise();
                    }
                    else
                    {
                        currentGameMode?.OnGameStart();
                        onGameStart?.Raise();
                    }
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

        // ─── Game Mode ──────────────────────────────────────────────────

        public void SetGameMode(IGameMode gameMode)
        {
            currentGameMode?.Cleanup();
            currentGameMode = gameMode;
            currentGameMode?.Initialize();
        }

        // ─── Score Methods ──────────────────────────────────────────────

        public void AddScore(int points)
        {
            CurrentScore += points;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void SetScore(int score)
        {
            CurrentScore = score;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        // ─── Convenience Commands ───────────────────────────────────────

        public void Play() => ChangeState(GameState.Playing);
        public void Pause() => ChangeState(GameState.Paused);
        public void Resume() => ChangeState(GameState.Playing);
        public void Complete() => ChangeState(GameState.Completed);
        public void Fail() => ChangeState(GameState.Failed);
        public void GoToMenu() => ChangeState(GameState.Menu);
        public void GoToPrepare() => ChangeState(GameState.Prepare);

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

        // ─── Legacy API (backwards compatible) ──────────────────────────

        [Obsolete("Use GameTime property instead")]
        public float GetGameTime() => gameTime;

        [Obsolete("Use CurrentGameMode property instead")]
        public IGameMode GetCurrentGameMode() => currentGameMode;
    }
}