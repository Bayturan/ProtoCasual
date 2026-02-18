using UnityEngine;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Sample.GameModes
{
    public class EndlessRunnerGameMode : GameModeBase
    {
        [SerializeField] private EndlessRunnerConfig config;
        [SerializeField] private ChunkGenerator chunkGenerator;
        [SerializeField] private DifficultySystem difficultySystem;

        public override void Initialize()
        {
            chunkGenerator.Initialize(config);
            difficultySystem.Initialize(config);
        }

        public override void OnGameStart()
        {
            difficultySystem.StartScaling();
        }

        public override void OnGameFail()
        {
            difficultySystem.StopScaling();
        }

        public override void OnGameComplete()
        {
            difficultySystem.StopScaling();
        }
    }
}
