using UnityEngine;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.GameLoop
{
    /// <summary>
    /// Base class for all game modes. Extend this to create genre-specific modes.
    /// </summary>
    public abstract class GameModeBase : MonoBehaviour, IGameMode
    {
        public virtual string ModeName => GetType().Name.Replace("GameMode", "");

        public abstract void Initialize();

        public virtual void OnGameStart() { }
        public virtual void OnGamePause() { }
        public virtual void OnGameResume() { }
        public virtual void OnGameComplete() { }
        public virtual void OnGameFail() { }
        public virtual void Cleanup() { }

        public virtual void UpdateMode(float deltaTime) { }

        // Legacy bridge methods for backward compatibility
        public void StartGame() => OnGameStart();
        public void EndGame(bool success)
        {
            if (success) OnGameComplete();
            else OnGameFail();
        }
    }
}