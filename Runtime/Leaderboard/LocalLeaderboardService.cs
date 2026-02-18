using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Leaderboard
{
    /// <summary>
    /// Local leaderboard service backed by <see cref="PlayerData.Leaderboards"/>.
    /// Stores entries per leaderboard ID, sorts by score.
    /// Replace with a cloud implementation by registering your own <see cref="ILeaderboardService"/>.
    /// </summary>
    public class LocalLeaderboardService : ILeaderboardService
    {
        public event Action<string> OnScoreSubmitted;
        public event Action<string, List<LeaderboardEntry>> OnLeaderboardLoaded;

        private readonly Dictionary<string, LeaderboardDefinition> definitions = new();
        private readonly Dictionary<string, List<LeaderboardEntry>> cache = new();
        private readonly LeaderboardSaveData data;
        private readonly PlayerDataProvider dataProvider;

        public LocalLeaderboardService(LeaderboardConfig config, PlayerDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            data = dataProvider.Data.Leaderboards;

            if (config != null && config.leaderboards != null)
            {
                foreach (var def in config.leaderboards)
                {
                    if (def != null && !string.IsNullOrEmpty(def.leaderboardId))
                        definitions[def.leaderboardId] = def;
                }
            }

            RebuildCache();
        }

        public void SubmitScore(string leaderboardId, int score)
        {
            if (string.IsNullOrEmpty(leaderboardId)) return;

            var board = GetOrCreateBoard(leaderboardId);
            var existing = board.Entries.Find(e => e.PlayerId == "local_player");

            bool descending = definitions.TryGetValue(leaderboardId, out var def) && def.descending;

            if (existing != null)
            {
                bool isBetter = descending ? score > existing.Score : score < existing.Score;
                if (!isBetter) return;
                existing.Score = score;
            }
            else
            {
                board.Entries.Add(new LeaderboardEntry
                {
                    PlayerId = "local_player",
                    PlayerName = "You",
                    Score = score,
                    Rank = 0
                });
            }

            SortBoard(board, descending);
            dataProvider.Save();
            RebuildCache();
            OnScoreSubmitted?.Invoke(leaderboardId);
            Debug.Log($"[LeaderboardService] Submitted score {score} to '{leaderboardId}'.");
        }

        public void LoadLeaderboard(string leaderboardId, int maxEntries = 10)
        {
            if (!cache.TryGetValue(leaderboardId, out var entries))
                entries = new List<LeaderboardEntry>();

            int count = Mathf.Min(maxEntries, entries.Count);
            var result = entries.GetRange(0, count);
            OnLeaderboardLoaded?.Invoke(leaderboardId, result);
        }

        public LeaderboardEntry GetPlayerBest(string leaderboardId)
        {
            if (!cache.TryGetValue(leaderboardId, out var entries)) return null;
            return entries.Find(e => e.PlayerId == "local_player");
        }

        public List<LeaderboardEntry> GetCachedEntries(string leaderboardId)
        {
            return cache.TryGetValue(leaderboardId, out var entries)
                ? new List<LeaderboardEntry>(entries)
                : new List<LeaderboardEntry>();
        }

        // ─── Internal ───────────────────────────────────────────────────

        private LeaderboardBoardData GetOrCreateBoard(string id)
        {
            foreach (var b in data.Boards)
                if (b.LeaderboardId == id) return b;

            var newBoard = new LeaderboardBoardData { LeaderboardId = id };
            data.Boards.Add(newBoard);
            return newBoard;
        }

        private void SortBoard(LeaderboardBoardData board, bool descending)
        {
            board.Entries.Sort((a, b) => descending
                ? b.Score.CompareTo(a.Score)
                : a.Score.CompareTo(b.Score));

            for (int i = 0; i < board.Entries.Count; i++)
                board.Entries[i].Rank = i + 1;

            int max = definitions.TryGetValue(board.LeaderboardId, out var def) ? def.maxEntries : 100;
            if (board.Entries.Count > max)
                board.Entries.RemoveRange(max, board.Entries.Count - max);
        }

        private void RebuildCache()
        {
            cache.Clear();
            foreach (var board in data.Boards)
                cache[board.LeaderboardId] = new List<LeaderboardEntry>(board.Entries);
        }
    }
}
