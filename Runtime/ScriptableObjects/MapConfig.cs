using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Map Config")]
    public class MapConfig : ScriptableObject
    {
        [Header("Map Info")]
        public string mapName;
        public bool isProcedural;

        [Header("Dimensions")]
        public Vector2 mapSize = new Vector2(50, 50);
        public float tileSize = 1f;

        [Header("Generation")]
        public int seed;
        public GameObject[] tilePrefabs;
        public GameObject[] obstaclePrefabs;
        public GameObject[] collectiblePrefabs;

        [Header("Spawning")]
        [Range(0f, 1f)] public float obstacleSpawnChance = 0.2f;
        [Range(0f, 1f)] public float collectibleSpawnChance = 0.1f;
        public int maxObstacles = 20;
        public int maxCollectibles = 10;
    }
}
