using UnityEngine;

public class DifficultySystem : MonoBehaviour
{
    private EndlessRunnerConfig config;
    private float elapsedTime;
    private float currentSpeed;

    public float CurrentSpeed => currentSpeed;

    public void Initialize(EndlessRunnerConfig cfg)
    {
        config = cfg;
        currentSpeed = config.baseSpeed;
    }

    public void StartScaling()
    {
        elapsedTime = 0f;
        enabled = true;
    }

    public void StopScaling()
    {
        enabled = false;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        currentSpeed = config.baseSpeed +
            (elapsedTime / 60f) * config.speedIncreasePerMinute;
    }
}
