using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Analytics
{
    /// <summary>
    /// Default analytics implementation — logs to console.
    /// Replace with Firebase / GameAnalytics by implementing <see cref="IAnalyticsService"/>
    /// and registering your implementation in ServiceLocator.
    /// </summary>
    public class DebugAnalyticsService : IAnalyticsService
    {
        public bool IsEnabled { get; set; }

        private readonly AnalyticsConfig config;

        public DebugAnalyticsService(AnalyticsConfig config = null)
        {
            this.config = config;
            IsEnabled = config == null || config.enabled;
        }

        public void TrackEvent(string eventName)
        {
            if (!ShouldLog()) return;
            Debug.Log($"[Analytics] {eventName}");
        }

        public void TrackEvent(string eventName, string paramKey, string paramValue)
        {
            if (!ShouldLog()) return;
            Debug.Log($"[Analytics] {eventName} | {paramKey}={paramValue}");
        }

        public void TrackEvent(string eventName, string paramKey, int paramValue)
        {
            if (!ShouldLog()) return;
            Debug.Log($"[Analytics] {eventName} | {paramKey}={paramValue}");
        }

        public void TrackEvent(string eventName, string paramKey, float paramValue)
        {
            if (!ShouldLog()) return;
            Debug.Log($"[Analytics] {eventName} | {paramKey}={paramValue:F2}");
        }

        public void LevelStart(int levelIndex)
        {
            if (!IsEnabled || (config != null && !config.trackLevelEvents)) return;
            TrackEvent("level_start", "level", levelIndex);
        }

        public void LevelComplete(int levelIndex, float duration)
        {
            if (!IsEnabled || (config != null && !config.trackLevelEvents)) return;
            TrackEvent("level_complete", "level", levelIndex);
            TrackEvent("level_complete", "duration", duration);
        }

        public void LevelFail(int levelIndex, float duration)
        {
            if (!IsEnabled || (config != null && !config.trackLevelEvents)) return;
            TrackEvent("level_fail", "level", levelIndex);
            TrackEvent("level_fail", "duration", duration);
        }

        public void Purchase(string itemId, int price, bool hardCurrency)
        {
            if (!IsEnabled || (config != null && !config.trackPurchaseEvents)) return;
            TrackEvent("purchase", "item", itemId);
            TrackEvent("purchase", "price", price);
            TrackEvent("purchase", "currency", hardCurrency ? "hard" : "soft");
        }

        public void AdWatched(string adType, string placement)
        {
            if (!IsEnabled || (config != null && !config.trackAdEvents)) return;
            TrackEvent("ad_watched", "type", adType);
            TrackEvent("ad_watched", "placement", placement);
        }

        public void TutorialStep(int stepIndex, string stepName)
        {
            if (!IsEnabled || (config != null && !config.trackTutorialEvents)) return;
            TrackEvent("tutorial_step", "step", stepIndex);
            TrackEvent("tutorial_step", "name", stepName);
        }

        private bool ShouldLog() => IsEnabled && (config == null || config.debugLogging);
    }
}
