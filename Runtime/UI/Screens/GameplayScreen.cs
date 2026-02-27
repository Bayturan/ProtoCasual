using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.GameLoop;

namespace ProtoCasual.Core.UI
{
    public class GameplayScreen : UIScreen
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Slider progressSlider;

        protected override void OnInitialize()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);
        }

        protected override void OnShow()
        {
            UpdateUI();

            // Show current level
            if (levelText != null)
            {
                var levelMgr = LevelManager.Instance;
                int lvl = levelMgr != null ? levelMgr.CurrentLevelIndex + 1 : 1;
                levelText.text = $"Level {lvl}";
            }

            // Start gameplay music
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayGameplayMusic();
        }

        private void Update()
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState == GameState.Playing)
            {
                UpdateUI();
            }
        }

        public void AddScore(int points)
        {
            GameManager.Instance?.AddScore(points);
        }

        public void SetLevelText(string text)
        {
            if (levelText != null)
                levelText.text = text;
        }

        public void SetProgress(float progress)
        {
            if (progressSlider != null)
                progressSlider.value = progress;
        }

        private void UpdateUI()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (scoreText != null)
                scoreText.text = $"Score: {gm.CurrentScore}";

            if (timeText != null)
                timeText.text = $"Time: {gm.GameTime:F1}s";
        }

        private void OnPauseClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Pause();
        }
    }
}
