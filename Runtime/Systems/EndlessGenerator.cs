using UnityEngine;
using System.Collections.Generic;

namespace ProtoCasual.Core.Systems
{
    public class EndlessGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject[] chunkPrefabs;
        [SerializeField] private float chunkLength = 10f;
        [SerializeField] private int activeChunkCount = 5;
        [SerializeField] private Transform playerTransform;

        private Queue<GameObject> activeChunks = new Queue<GameObject>();
        private float nextSpawnZ;

        private void Start()
        {
            // Spawn initial chunks
            for (int i = 0; i < activeChunkCount; i++)
            {
                SpawnChunk();
            }
        }

        private void Update()
        {
            if (playerTransform == null) return;

            // Check if we need to spawn new chunk
            if (playerTransform.position.z > nextSpawnZ - (activeChunkCount * chunkLength))
            {
                SpawnChunk();
                RemoveOldChunk();
            }
        }

        private void SpawnChunk()
        {
            if (chunkPrefabs == null || chunkPrefabs.Length == 0) return;

            GameObject chunkPrefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
            Vector3 spawnPosition = new Vector3(0, 0, nextSpawnZ);
            
            GameObject chunk = Instantiate(chunkPrefab, spawnPosition, Quaternion.identity, transform);
            activeChunks.Enqueue(chunk);
            
            nextSpawnZ += chunkLength;
        }

        private void RemoveOldChunk()
        {
            if (activeChunks.Count > activeChunkCount)
            {
                GameObject oldChunk = activeChunks.Dequeue();
                Destroy(oldChunk);
            }
        }

        public void Reset()
        {
            while (activeChunks.Count > 0)
            {
                GameObject chunk = activeChunks.Dequeue();
                Destroy(chunk);
            }

            nextSpawnZ = 0;
            
            for (int i = 0; i < activeChunkCount; i++)
            {
                SpawnChunk();
            }
        }
    }
}
