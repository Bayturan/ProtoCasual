using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Achievements
{
    /// <summary>
    /// Condition-based achievement system backed by <see cref="PlayerData.Achievements"/>.
    /// Auto-grants rewards via <see cref="IRewardService"/> on unlock.
    /// Register as <see cref="IAchievementService"/> in ServiceLocator.
    /// </summary>
    public class AchievementService : IAchievementService
    {
        public event Action<string> OnAchievementUnlocked;
        public event Action<string, int> OnAchievementProgress;

        private readonly Dictionary<string, AchievementDefinition> definitions = new();
        private readonly Dictionary<string, AchievementProgressData> progressLookup = new();
        private readonly AchievementSaveData data;
        private readonly PlayerDataProvider dataProvider;
        private readonly IRewardService rewardService;

        public AchievementService(AchievementConfig config, PlayerDataProvider dataProvider,
            IRewardService rewardService = null)
        {
            this.dataProvider = dataProvider;
            this.rewardService = rewardService;
            data = dataProvider.Data.Achievements;

            if (config != null && config.achievements != null)
            {
                foreach (var def in config.achievements)
                {
                    if (def != null && !string.IsNullOrEmpty(def.achievementId))
                        definitions[def.achievementId] = def;
                }
            }

            RebuildLookup();
        }

        public void AddProgress(string achievementId, int amount = 1)
        {
            if (string.IsNullOrEmpty(achievementId) || amount <= 0) return;

            var entry = GetOrCreateEntry(achievementId);
            if (entry.IsUnlocked) return;

            entry.CurrentProgress += amount;
            OnAchievementProgress?.Invoke(achievementId, entry.CurrentProgress);

            if (definitions.TryGetValue(achievementId, out var def) &&
                entry.CurrentProgress >= def.requiredProgress)
            {
                entry.IsUnlocked = true;
                OnAchievementUnlocked?.Invoke(achievementId);

                if (rewardService != null && def.rewards != null && def.rewards.Length > 0)
                    rewardService.GrantRewards(def.rewards);

                Debug.Log($"[AchievementService] Unlocked '{achievementId}'!");
            }

            dataProvider.Save();
        }

        public bool IsUnlocked(string achievementId)
        {
            return progressLookup.TryGetValue(achievementId, out var e) && e.IsUnlocked;
        }

        public int GetProgress(string achievementId)
        {
            return progressLookup.TryGetValue(achievementId, out var e) ? e.CurrentProgress : 0;
        }

        public List<AchievementProgress> GetAll()
        {
            var result = new List<AchievementProgress>();
            foreach (var def in definitions.Values)
            {
                var entry = GetOrCreateEntry(def.achievementId);
                result.Add(new AchievementProgress
                {
                    AchievementId = def.achievementId,
                    CurrentProgress = entry.CurrentProgress,
                    IsUnlocked = entry.IsUnlocked
                });
            }
            return result;
        }

        public void Reset()
        {
            data.Entries.Clear();
            progressLookup.Clear();
            dataProvider.Save();
        }

        // ─── Internal ───────────────────────────────────────────────────

        private AchievementProgressData GetOrCreateEntry(string id)
        {
            if (progressLookup.TryGetValue(id, out var existing))
                return existing;

            var entry = new AchievementProgressData { AchievementId = id };
            data.Entries.Add(entry);
            progressLookup[id] = entry;
            return entry;
        }

        private void RebuildLookup()
        {
            progressLookup.Clear();
            foreach (var e in data.Entries)
            {
                if (e != null && !string.IsNullOrEmpty(e.AchievementId))
                    progressLookup[e.AchievementId] = e;
            }
        }
    }
}
