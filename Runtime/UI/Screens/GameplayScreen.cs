using UnityEngine.UIElements;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Core.UI.Screens
{
    /// <summary>In-game HUD — score, time, level, pause button, progress bar.</summary>
    public class GameplayScreen : ScreenController
    {
        public override string ScreenName => "GameplayScreen";

        private Label scoreLabel;
        private Label timeLabel;
        private Label levelLabel;
        private ProgressBar progressBar;

        protected override void OnBind()
        {
            Btn("pause-btn")?.RegisterCallback<ClickEvent>(OnPauseClicked);
            scoreLabel = Lbl("score-text");
            timeLabel = Lbl("time-text");
            levelLabel = Lbl("level-text");
            progressBar = PBar("progress-bar");
        }

        public override void OnShow()
        {
            UpdateUI();
            if (levelLabel != null)
            {
                var levelMgr = LevelManager.Instance;
                int lvl = levelMgr != null ? levelMgr.CurrentLevelIndex + 1 : 1;
                levelLabel.text = $"Level {lvl}";
            }
            AudioManager.Instance?.PlayGameplayMusic();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState == GameState.Playing)
            {
                UpdateUI();
            }
        }

        public void SetLevelText(string text) { if (levelLabel != null) levelLabel.text = text; }
        public void SetProgress(float progress) { if (progressBar != null) progressBar.value = progress * 100f; }
        public void AddScore(int points) => GameManager.Instance?.AddScore(points);

        private void UpdateUI()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            if (scoreLabel != null) scoreLabel.text = $"Score: {gm.CurrentScore}";
            if (timeLabel != null) timeLabel.text = $"Time: {gm.GameTime:F1}s";
        }

        private void OnPauseClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Pause();
        }
    }
}
