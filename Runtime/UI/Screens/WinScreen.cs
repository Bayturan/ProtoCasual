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
                nextButton.onClick.AddListener(OnNextClicked);

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);
        }

        protected override void OnShow()
        {
            // Display final score & time
            var gm = GameManager.Instance;
            if (gm != null)
            {
                if (scoreText != null)
                    scoreText.text = $"Score: {gm.CurrentScore}";

                if (timeText != null)
                    timeText.text = $"Time: {gm.GameTime:F2}s";
            }

            // Play win audio
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLevelComplete();
                AudioManager.Instance.PlayWinMusic();
            }

            // Advance to next level
            if (LevelManager.Instance != null)
                LevelManager.Instance.NextLevel();
        }

        private void OnNextClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Restart();
        }

        private void OnRestartClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Restart();
        }

        private void OnMenuClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.ReturnToMenu();
        }
    }
}
