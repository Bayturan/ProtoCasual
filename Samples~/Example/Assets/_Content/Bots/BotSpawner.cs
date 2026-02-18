using UnityEngine;
using System.Collections.Generic;

namespace ProtoCasual.Core.Systems
{
    public class BotSpawner : MonoBehaviour
    {
        [SerializeField] private BotConfig[] botConfigs;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Transform waypointParent;

        private List<BotController> activeBots = new List<BotController>();
        private Transform[] waypoints;

        private void Awake()
        {
            if (waypointParent != null)
            {
                int waypointCount = waypointParent.childCount;
                waypoints = new Transform[waypointCount];
                for (int i = 0; i < waypointCount; i++)
                {
                    waypoints[i] = waypointParent.GetChild(i);
                }
            }
        }

        public void SpawnBots(int count)
        {
            ClearBots();

            if (botConfigs == null || botConfigs.Length == 0)
            {
                Debug.LogWarning("No bot configs assigned!");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                SpawnBot(i);
            }
        }

        private void SpawnBot(int index)
        {
            BotConfig config = botConfigs[Random.Range(0, botConfigs.Length)];
            
            if (config.botPrefab == null)
            {
                Debug.LogWarning($"Bot config {config.botName} has no prefab!");
                return;
            }

            Vector3 spawnPos = GetSpawnPosition(index);
            GameObject botGO = Instantiate(config.botPrefab, spawnPos, Quaternion.identity, transform);
            
            BotController botController = botGO.GetComponent<BotController>();
            if (botController == null)
            {
                botController = botGO.AddComponent<BotController>();
            }

            // Initialize bot (BotController has no Initialize method in simple version)
            // You can manually set bot properties here if needed
            
            activeBots.Add(botController);
        }

        private Vector3 GetSpawnPosition(int index)
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int spawnIndex = index % spawnPoints.Length;
                return spawnPoints[spawnIndex].position;
            }

            return Vector3.zero + Vector3.right * index * 2f;
        }

        public void ClearBots()
        {
            foreach (var bot in activeBots)
            {
                if (bot != null)
                {
                    Destroy(bot.gameObject);
                }
            }
            activeBots.Clear();
        }

        public List<BotController> GetActiveBots()
        {
            return new List<BotController>(activeBots);
        }
    }
}
