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
            {
                retryButton.onClick.AddListener(OnRetryClicked);
            }

            if (menuButton != null)
            {
                menuButton.onClick.AddListener(OnMenuClicked);
            }

            if (watchAdButton != null)
            {
                watchAdButton.onClick.AddListener(OnWatchAdClicked);
            }
        }

        private void OnRetryClicked()
        {
            GameManager.Instance.Restart();
        }

        private void OnMenuClicked()
        {
            GameManager.Instance.ReturnToMenu();
        }

        private void OnWatchAdClicked()
        {
            var adsService = ServiceLocator.Get<IAdsService>();
            adsService?.ShowRewarded((success) =>
            {
                if (success)
                {
                    Hide();
                    GameManager.Instance.Resume();
                }
            });
        }
    }
}
