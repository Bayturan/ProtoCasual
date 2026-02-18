using UnityEngine;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Sample.GameModes
{
    public class PuzzleGameMode : GameModeBase
    {
        public override void Initialize()
        {
            Debug.Log("Puzzle Mode Init");
        }

        public override void OnGameStart()
        {
            Debug.Log("Puzzle Start");
        }

        public override void OnGameComplete()
        {
            Debug.Log("Puzzle Win!");
        }

        public override void OnGameFail()
        {
            Debug.Log("Puzzle Lose!");
        }
    }
}
