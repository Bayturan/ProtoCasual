using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    [SerializeField] private BotController botPrefab;
    [SerializeField] private int count = 5;

    public void SpawnBots()
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(botPrefab, GetSpawnPoint(), Quaternion.identity)
                .Initialize();
        }
    }

    private Vector3 GetSpawnPoint()
    {
        return new Vector3(Random.Range(-2, 2), 0, Random.Range(-5, 0));
    }
}
