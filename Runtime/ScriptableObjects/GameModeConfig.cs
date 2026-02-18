using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    public enum GameModeType
    {
        HyperCasual,
        Puzzle,
        Racing,
        Endless,
        EndlessRunner,
        Hybrid
    }

    [CreateAssetMenu(menuName = "ProtoCasual/Config/GameMode Config")]
    public class GameModeConfig : ScriptableObject
    {
        public GameModeType mode;
        public string modeName;
        public string description;
    }
}
