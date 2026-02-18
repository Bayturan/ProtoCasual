using UnityEngine;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.GameModes
{
    public class HyperCasualGameMode : GameModeBase
    {
        public override string ModeName => "HyperCasual";

        [Header("Player")]
        [SerializeField] private Transform player;
        
        [Header("Win Conditions")]
        [SerializeField] private float targetDistance = 100f;
        
        private float distanceTraveled;
        private int score;
        private bool isRunning;

        public override void Initialize()
        {
            distanceTraveled = 0f;
            score = 0;
            isRunning = false;
            Debug.Log("HyperCasual Mode Initialized");
        }

        public override void StartGame()
        {
            isRunning = true;
            Debug.Log("HyperCasual Game Started!");
        }

        public override void EndGame(bool success)
        {
            isRunning = false;
            if (success)
            {
                Debug.Log($"HyperCasual Win! Distance: {distanceTraveled:F0}");
            }
            else
            {
                Debug.Log($"HyperCasual Lose! Distance: {distanceTraveled:F0}");
            }
        }

        private void Update()
        {
            if (!isRunning) return;

            // Track distance
            if (player != null)
            {
                distanceTraveled += player.forward.magnitude * Time.deltaTime;
            }

            // Check win condition
            if (distanceTraveled >= targetDistance)
            {
                GameManager.Instance.CompleteGame();
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
                score += 10;
                Destroy(other.gameObject);
            }
        }

        public int GetScore() => score;
        public float GetDistance() => distanceTraveled;
    }
}
