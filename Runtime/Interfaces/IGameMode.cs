namespace ProtoCasual.Core.Interfaces
{
    public interface IGameMode
    {
        string ModeName { get; }
        void Initialize();
        void OnGameStart();
        void OnGamePause();
        void OnGameResume();
        void OnGameComplete();
        void OnGameFail();
        void Cleanup();
        void UpdateMode(float deltaTime);
    }
}
