using System;
using System.Collections.Generic;

namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Serializable achievement progress entry.
    /// </summary>
    [Serializable]
    public class AchievementProgress
    {
        public string AchievementId;
        public int CurrentProgress;
        public bool IsUnlocked;
    }

    /// <summary>
    /// Condition-based achievement / unlock system.
    /// </summary>
    public interface IAchievementService
    {
        event Action<string> OnAchievementUnlocked;
        event Action<string, int> OnAchievementProgress;

        void AddProgress(string achievementId, int amount = 1);
        bool IsUnlocked(string achievementId);
        int GetProgress(string achievementId);
        List<AchievementProgress> GetAll();
        void Reset();
    }
}
