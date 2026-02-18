using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Utilities;

namespace ProtoCasual.Core.Managers
{
    public class LevelManager : Singleton<LevelManager>, IManager
    {
        [SerializeField] private LevelConfig[] levels;
        private GameObject currentLevel;
        private int currentLevelIndex;

        public int CurrentLevelIndex => currentLevelIndex;
        public int TotalLevels => levels != null ? levels.Length : 0;

        public void Init()
        {
            currentLevelIndex = SaveManager.CurrentLevel;
            LoadLevel(currentLevelIndex);
        }

        public void Cleanup()
        {
            if (currentLevel != null)
                Destroy(currentLevel);
        }

        public void LoadLevel(int index)
        {
            if (levels == null || levels.Length == 0)
            {
                Debug.LogWarning("[LevelManager] No levels configured!");
                return;
            }

            if (currentLevel != null)
                Destroy(currentLevel);

            currentLevelIndex = index % levels.Length;
            if (levels[currentLevelIndex].levelPrefab != null)
                currentLevel = Instantiate(levels[currentLevelIndex].levelPrefab);
        }

        public void NextLevel()
        {
            SaveManager.NextLevel();
            LoadLevel(SaveManager.CurrentLevel);
        }
    }
}
