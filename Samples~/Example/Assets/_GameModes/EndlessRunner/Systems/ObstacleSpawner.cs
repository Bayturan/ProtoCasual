using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private EndlessRunnerConfig config;

    public void TrySpawn(Vector3 position)
    {
        if (Random.value > config.obstacleSpawnChance)
            return;

        Instantiate(obstacles[Random.Range(0, obstacles.Length)],
                    position,
                    Quaternion.identity);
    }
}
