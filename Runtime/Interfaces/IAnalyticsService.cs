namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Analytics facade — fire-and-forget event tracking.
    /// Implement with Firebase, GameAnalytics, or any backend.
    /// </summary>
    public interface IAnalyticsService
    {
        bool IsEnabled { get; set; }

        void TrackEvent(string eventName);
        void TrackEvent(string eventName, string paramKey, string paramValue);
        void TrackEvent(string eventName, string paramKey, int paramValue);
        void TrackEvent(string eventName, string paramKey, float paramValue);

        void LevelStart(int levelIndex);
        void LevelComplete(int levelIndex, float duration);
        void LevelFail(int levelIndex, float duration);
        void Purchase(string itemId, int price, bool hardCurrency);
        void AdWatched(string adType, string placement);
        void TutorialStep(int stepIndex, string stepName);
    }
}
