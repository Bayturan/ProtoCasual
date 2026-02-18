using System;

namespace ProtoCasual.Core.Interfaces
{
    public enum AdType { Interstitial, Rewarded, Banner }
    
    public interface IAdsService
    {
        bool IsInitialized { get; }
        bool IsRewardedAdReady { get; }
        bool IsInterstitialAdReady { get; }
        
        void Initialize();
        void ShowInterstitial(Action onClosed = null, Action onFailed = null);
        void ShowRewarded(Action<bool> onComplete);
        void ShowBanner();
        void HideBanner();
    }
}
