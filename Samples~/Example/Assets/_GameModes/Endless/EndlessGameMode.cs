using UnityEngine;
using ProtoCasual.Core.Systems;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.GameModes
{
    public class EndlessGameMode : GameModeBase
    {
        public override string ModeName => "Endless";

        [Header("Player")]
        [SerializeField] private Transform player;
        
        [Header("Generation")]
        [SerializeField] private EndlessGenerator endlessGenerator;
        
        private float distanceTraveled;
        private int score;
        private float speedMultiplier = 1f;
        private float difficultyTimer;
        private bool isRunning;

        public override void Initialize()
        {
            distanceTraveled = 0f;
            score = 0;
            speedMultiplier = 1f;
            difficultyTimer = 0f;
            isRunning = false;
            
            if (endlessGenerator != null)
            {
                endlessGenerator.Reset();
            }
            
            Debug.Log("Endless Mode Initialized");
        }

        public override void StartGame()
        {
            isRunning = true;
            Debug.Log("Endless Game Started!");
        }

        public override void EndGame(bool success)
        {
            isRunning = false;
            Debug.Log($"Endless Game Over! Distance: {distanceTraveled:F0}, Score: {score}");
            
            // Save high score
            int highScore = PlayerPrefs.GetInt("HighScore_Endless", 0);
            if (score > highScore)
            {
                PlayerPrefs.SetInt("HighScore_Endless", score);
                PlayerPrefs.Save();
                Debug.Log("New High Score!");
            }
        }

        private void Update()
        {
            if (!isRunning) return;

            // Track distance and score
            if (player != null)
            {
                distanceTraveled += player.forward.magnitude * Time.deltaTime * speedMultiplier;
                score = Mathf.RoundToInt(distanceTraveled);
            }

            // Increase difficulty over time
            difficultyTimer += Time.deltaTime;
            if (difficultyTimer >= 10f)
            {
                speedMultiplier += 0.1f;
                difficultyTimer = 0f;
                Debug.Log($"Difficulty increased! Speed: {speedMultiplier:F1}x");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isRunning) return;

            if (other.CompareTag("Obstacle"))
            {
                GameManager.Instance.FailGame();
            }
            else if (other.CompareTag("Collectible"))
            {
                score += 50;
                Destroy(other.gameObject);
            }
        }

        public float GetDistance() => distanceTraveled;
        public int GetScore() => score;
        public float GetSpeedMultiplier() => speedMultiplier;
    }
}
