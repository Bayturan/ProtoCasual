using System;
using System.Collections.Generic;

namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Leaderboard entry.
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public string PlayerId;
        public string PlayerName;
        public int Score;
        public int Rank;
    }

    /// <summary>
    /// Local + cloud leaderboard service.
    /// </summary>
    public interface ILeaderboardService
    {
        event Action<string> OnScoreSubmitted;
        event Action<string, List<LeaderboardEntry>> OnLeaderboardLoaded;

        void SubmitScore(string leaderboardId, int score);
        void LoadLeaderboard(string leaderboardId, int maxEntries = 10);
        LeaderboardEntry GetPlayerBest(string leaderboardId);
        List<LeaderboardEntry> GetCachedEntries(string leaderboardId);
    }
}
