using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Haptics
{
    /// <summary>
    /// Default haptic service using <see cref="Handheld.Vibrate"/> on mobile.
    /// Reads durations from <see cref="HapticConfig"/> (Android) or uses
    /// UIImpactFeedbackGenerator on iOS (via native plugin if available).
    /// Register as <see cref="IHapticService"/> in ServiceLocator.
    /// </summary>
    public class HapticService : IHapticService
    {
        public bool IsEnabled { get; set; }

        private readonly HapticConfig config;

        public HapticService(HapticConfig config)
        {
            this.config = config;
            IsEnabled = config != null && config.enabled;
        }

        public void LightImpact() => Vibrate(config != null ? config.lightDuration : 20);
        public void MediumImpact() => Vibrate(config != null ? config.mediumDuration : 40);
        public void HeavyImpact() => Vibrate(config != null ? config.heavyDuration : 80);
        public void Selection() => Vibrate(config != null ? config.selectionDuration : 10);
        public void Success() => Vibrate(config != null ? config.successDuration : 50);
        public void Warning() => Vibrate(config != null ? config.warningDuration : 60);
        public void Error() => Vibrate(config != null ? config.errorDuration : 100);

        private void Vibrate(long durationMs)
        {
            if (!IsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                vibrator?.Call("vibrate", durationMs);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[HapticService] Android vibrate failed: {e.Message}");
            }
#elif UNITY_IOS && !UNITY_EDITOR
            Handheld.Vibrate();
#else
            Debug.Log($"[HapticService] Vibrate {durationMs}ms (editor — no-op)");
#endif
        }
    }
}
