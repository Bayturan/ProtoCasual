using UnityEngine;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.GameModes
{
    public class HybridGameMode : GameModeBase
    {
        public override string ModeName => "Hybrid";

        [Header("Modes")]
        [SerializeField] private GameObject primaryModeObject;
        [SerializeField] private GameObject secondaryModeObject;
        
        [Header("Switching")]
        [SerializeField] private float switchInterval = 30f;
        
        private float switchTimer;
        private bool isPrimaryActive = true;
        private bool isRunning;

        public override void Initialize()
        {
            switchTimer = switchInterval;
            isPrimaryActive = true;
            isRunning = false;
            
            // Setup initial mode
            if (primaryModeObject != null) primaryModeObject.SetActive(true);
            if (secondaryModeObject != null) secondaryModeObject.SetActive(false);
            
            Debug.Log("Hybrid Mode Initialized");
        }

        public override void StartGame()
        {
            isRunning = true;
            Debug.Log("Hybrid Game Started!");
        }

        public override void EndGame(bool success)
        {
            isRunning = false;
            if (success)
            {
                Debug.Log("Hybrid Mode Win!");
            }
            else
            {
                Debug.Log("Hybrid Mode Lose!");
            }
        }

        private void Update()
        {
            if (!isRunning) return;

            // Handle mode switching
            switchTimer -= Time.deltaTime;
            if (switchTimer <= 0f)
            {
                SwitchMode();
                switchTimer = switchInterval;
            }
        }

        private void SwitchMode()
        {
            isPrimaryActive = !isPrimaryActive;
            
            if (isPrimaryActive)
            {
                if (primaryModeObject != null) primaryModeObject.SetActive(true);
                if (secondaryModeObject != null) secondaryModeObject.SetActive(false);
                Debug.Log("Switched to Primary Mode");
            }
            else
            {
                if (primaryModeObject != null) primaryModeObject.SetActive(false);
                if (secondaryModeObject != null) secondaryModeObject.SetActive(true);
                Debug.Log("Switched to Secondary Mode");
            }
        }

        public bool IsPrimaryActive() => isPrimaryActive;
        public float GetTimeUntilSwitch() => switchTimer;
    }
}
