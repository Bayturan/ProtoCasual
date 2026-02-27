using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.UI
{
    public class LoseScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button watchAdButton;

        protected override void OnInitialize()
        {
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryClicked);

            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);

            if (watchAdButton != null)
                watchAdButton.onClick.AddListener(OnWatchAdClicked);
        }

        protected override void OnShow()
        {
            // Play fail audio
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLevelFail();
                AudioManager.Instance.PlayLoseMusic();
            }
        }

        private void OnRetryClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Restart();
        }

        private void OnMenuClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.ReturnToMenu();
        }

        private void OnWatchAdClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            var adsService = ServiceLocator.IsRegistered<IAdsService>()
                ? ServiceLocator.Get<IAdsService>()
                : null;
            adsService?.ShowRewarded((success) =>
            {
                if (success)
                {
                    Hide();
                    GameManager.Instance?.Resume();
                }
            });
        }
    }
}
