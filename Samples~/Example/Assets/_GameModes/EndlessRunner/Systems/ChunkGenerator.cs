using UnityEngine;
using System.Collections.Generic;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] chunkPrefabs;

    private EndlessRunnerConfig config;
    private Queue<GameObject> activeChunks = new();
    private float nextSpawnZ;

    public void Initialize(EndlessRunnerConfig cfg)
    {
        config = cfg;
        nextSpawnZ = 0f;

        for (int i = 0; i < config.initialChunks; i++)
            SpawnChunk();
    }

    private void Update()
    {
        if (player.position.z > nextSpawnZ - (config.initialChunks * config.chunkLength))
            SpawnChunk();
    }

    private void SpawnChunk()
    {
        var chunk = Instantiate(
            chunkPrefabs[Random.Range(0, chunkPrefabs.Length)],
            Vector3.forward * nextSpawnZ,
            Quaternion.identity
        );

        activeChunks.Enqueue(chunk);
        nextSpawnZ += config.chunkLength;

        if (activeChunks.Count > config.initialChunks)
            Destroy(activeChunks.Dequeue());
    }
}
