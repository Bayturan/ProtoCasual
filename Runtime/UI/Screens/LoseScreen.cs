using UnityEngine.UIElements;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Core.UI.Screens
{
    /// <summary>Game over screen — retry, watch ad for revive, return to menu.</summary>
    public class LoseScreen : ScreenController
    {
        public override string ScreenName => "LoseScreen";

        protected override void OnBind()
        {
            Btn("retry-btn")?.RegisterCallback<ClickEvent>(OnRetryClicked);
            Btn("menu-btn")?.RegisterCallback<ClickEvent>(OnMenuClicked);
            Btn("watch-ad-btn")?.RegisterCallback<ClickEvent>(OnWatchAdClicked);
        }

        public override void OnShow()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLevelFail();
                AudioManager.Instance.PlayLoseMusic();
            }
        }

        private void OnRetryClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Restart();
        }

        private void OnMenuClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.ReturnToMenu();
        }

        private void OnWatchAdClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            if (ServiceLocator.TryGet<IAdsService>(out var ads))
            {
                ads.ShowRewarded(success =>
                {
                    if (success) GameManager.Instance?.Resume();
                });
            }
        }
    }
}
