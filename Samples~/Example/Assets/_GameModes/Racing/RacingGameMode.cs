using UnityEngine;
using ProtoCasual.Core;
using ProtoCasual.Core.Managers;

public class RacingGameMode : GameModeBase
{
    [SerializeField] private TrackGenerator trackGenerator;
    [SerializeField] private BotSpawner botSpawner;

    public override void Initialize()
    {
        trackGenerator.Generate();
        botSpawner.SpawnBots();
    }

    public override void StartGame()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public override void EndGame(bool success)
    {
        GameManager.Instance.ChangeState(success ? GameState.Completed : GameState.Failed);
    }
}
