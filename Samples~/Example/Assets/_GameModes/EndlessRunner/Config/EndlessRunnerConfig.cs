using UnityEngine;

[CreateAssetMenu(menuName = "Config/EndlessRunner")]
public class EndlessRunnerConfig : ScriptableObject
{
    public float baseSpeed = 6f;
    public float speedIncreasePerMinute = 1.5f;

    public int initialChunks = 5;
    public float chunkLength = 20f;

    public float obstacleSpawnChance = 0.35f;
    public float pickupSpawnChance = 0.2f;
}
