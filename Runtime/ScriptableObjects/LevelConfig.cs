using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        public string levelName;
        public GameObject levelPrefab;
        public int levelIndex;
    }
}
