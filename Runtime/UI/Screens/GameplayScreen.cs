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
        [SerializeField] private Button pauseButton;
        [SerializeField] private Slider progressSlider;

        private float currentScore;
        private float currentTime;

        protected override void OnInitialize()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
            }
        }

        protected override void OnShow()
        {
            currentScore = 0;
            currentTime = 0;
            UpdateUI();
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState == GameState.Playing)
            {
                currentTime = GameManager.Instance.GetGameTime();
                UpdateUI();
            }
        }

        public void SetScore(float score)
        {
            currentScore = score;
            UpdateUI();
        }

        public void SetProgress(float progress)
        {
            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }
        }

        private void UpdateUI()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore:F0}";
            }

            if (timeText != null)
            {
                timeText.text = $"Time: {currentTime:F1}s";
            }
        }

        private void OnPauseClicked()
        {
            GameManager.Instance.ChangeState(GameState.Paused);
        }
    }
}
