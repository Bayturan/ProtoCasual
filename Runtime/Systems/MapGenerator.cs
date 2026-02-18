using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;
using System.Collections.Generic;

namespace ProtoCasual.Core.Systems
{
    public class MapGenerator : MonoBehaviour, IMapGenerator
    {
        [SerializeField] private MapConfig config;
        [SerializeField] private Transform mapParent;

        private List<GameObject> spawnedObjects = new List<GameObject>();

        public void GenerateMap(MapConfig config)
        {
            this.config = config;
            ClearMap();

            if (config.isProcedural)
            {
                GenerateProceduralMap();
            }
            else
            {
                GenerateFixedMap();
            }
        }

        private void GenerateProceduralMap()
        {
            Random.InitState(config.seed);

            int tilesX = Mathf.RoundToInt(config.mapSize.x / config.tileSize);
            int tilesZ = Mathf.RoundToInt(config.mapSize.y / config.tileSize);

            // Generate ground tiles
            for (int x = 0; x < tilesX; x++)
            {
                for (int z = 0; z < tilesZ; z++)
                {
                    Vector3 position = new Vector3(x * config.tileSize, 0, z * config.tileSize);
                    SpawnTile(position);

                    // Spawn obstacles
                    if (Random.value < config.obstacleSpawnChance && spawnedObjects.Count < config.maxObstacles)
                    {
                        SpawnObstacle(position);
                    }

                    // Spawn collectibles
                    if (Random.value < config.collectibleSpawnChance && spawnedObjects.Count < config.maxCollectibles)
                    {
                        SpawnCollectible(position);
                    }
                }
            }
        }

        private void GenerateFixedMap()
        {
            // Load pre-designed map
            Debug.Log("Loading fixed map layout");
        }

        private void SpawnTile(Vector3 position)
        {
            if (config.tilePrefabs == null || config.tilePrefabs.Length == 0) return;

            GameObject tilePrefab = config.tilePrefabs[Random.Range(0, config.tilePrefabs.Length)];
            GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, mapParent);
            spawnedObjects.Add(tile);
        }

        private void SpawnObstacle(Vector3 position)
        {
            if (config.obstaclePrefabs == null || config.obstaclePrefabs.Length == 0) return;

            GameObject obstaclePrefab = config.obstaclePrefabs[Random.Range(0, config.obstaclePrefabs.Length)];
            position.y = 0.5f;
            GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity, mapParent);
            spawnedObjects.Add(obstacle);
        }

        private void SpawnCollectible(Vector3 position)
        {
            if (config.collectiblePrefabs == null || config.collectiblePrefabs.Length == 0) return;

            GameObject collectiblePrefab = config.collectiblePrefabs[Random.Range(0, config.collectiblePrefabs.Length)];
            position.y = 1f;
            GameObject collectible = Instantiate(collectiblePrefab, position, Quaternion.identity, mapParent);
            spawnedObjects.Add(collectible);
        }

        public void ClearMap()
        {
            foreach (var obj in spawnedObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            spawnedObjects.Clear();
        }

        public void RegenerateMap()
        {
            if (config != null)
            {
                config.seed = Random.Range(0, 10000);
                GenerateMap(config);
            }
        }
    }
}
