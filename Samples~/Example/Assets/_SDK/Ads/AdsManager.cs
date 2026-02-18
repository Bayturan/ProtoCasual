using UnityEngine;
using System;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Managers
{
    public class AdsManager : MonoBehaviour, IAdsService
    {
        public bool IsInitialized { get; private set; }
        public bool IsRewardedAdReady => isRewardedReady;
        public bool IsInterstitialAdReady => isInterstitialReady;

        [Header("Settings")]
        [SerializeField] private bool enableAds = true;
        [SerializeField] private int interstitialFrequency = 3;

        private bool isRewardedReady = true; // Mock
        private bool isInterstitialReady = true; // Mock
        private int gameCount;

        public void Initialize()
        {
            if (IsInitialized) return;

            // Initialize your ad SDK here (Unity Ads, AdMob, etc.)
            IsInitialized = true;
            Debug.Log("Ads Initialized");
        }

        public void ShowInterstitial(Action onClosed = null, Action onFailed = null)
        {
            if (!enableAds || !IsInitialized)
            {
                onClosed?.Invoke();
                return;
            }

            gameCount++;
            if (gameCount % interstitialFrequency != 0)
            {
                onClosed?.Invoke();
                return;
            }

            // Show interstitial ad
            Debug.Log("Showing Interstitial Ad");
            onClosed?.Invoke();
        }

        public void ShowRewarded(Action<bool> onComplete)
        {
            if (!enableAds || !IsInitialized)
            {
                onComplete?.Invoke(false);
                return;
            }

            if (!isRewardedReady)
            {
                Debug.LogWarning("Rewarded ad not ready");
                onComplete?.Invoke(false);
                return;
            }

            // Show rewarded ad
            Debug.Log("Showing Rewarded Ad");
            // Mock reward
            onComplete?.Invoke(true);
        }

        public void ShowBanner()
        {
            if (!enableAds || !IsInitialized) return;
            
            Debug.Log("Showing Banner");
            // Show banner ad
        }

        public void HideBanner()
        {
            Debug.Log("Hiding Banner");
            // Hide banner ad
        }
    }
}
