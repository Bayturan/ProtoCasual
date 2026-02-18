using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.GameLoop;

namespace ProtoCasual.Core.UI
{
    public class WinScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        protected override void OnInitialize()
        {
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnNextClicked);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (menuButton != null)
            {
                menuButton.onClick.AddListener(OnMenuClicked);
            }
        }

        protected override void OnShow()
        {
            float finalTime = GameManager.Instance.GetGameTime();
            
            if (timeText != null)
            {
                timeText.text = $"Time: {finalTime:F2}s";
            }
        }

        public void SetFinalScore(float score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score:F0}";
            }
        }

        private void OnNextClicked()
        {
            GameManager.Instance.Restart();
        }

        private void OnRestartClicked()
        {
            GameManager.Instance.Restart();
        }

        private void OnMenuClicked()
        {
            GameManager.Instance.ReturnToMenu();
        }
    }
}
