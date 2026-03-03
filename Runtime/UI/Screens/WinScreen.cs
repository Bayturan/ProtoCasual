using UnityEngine.UIElements;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Core.UI.Screens
{
    /// <summary>Victory screen — score, time, next level / restart / menu buttons.</summary>
    public class WinScreen : ScreenController
    {
        public override string ScreenName => "WinScreen";

        private Label scoreLabel;
        private Label timeLabel;

        protected override void OnBind()
        {
            Btn("next-btn")?.RegisterCallback<ClickEvent>(OnNextClicked);
            Btn("restart-btn")?.RegisterCallback<ClickEvent>(OnRestartClicked);
            Btn("menu-btn")?.RegisterCallback<ClickEvent>(OnMenuClicked);
            scoreLabel = Lbl("score-text");
            timeLabel = Lbl("time-text");
        }

        public override void OnShow()
        {
            var gm = GameManager.Instance;
            if (gm != null)
            {
                if (scoreLabel != null) scoreLabel.text = $"Score: {gm.CurrentScore}";
                if (timeLabel != null) timeLabel.text = $"Time: {gm.GameTime:F2}s";
            }
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLevelComplete();
                AudioManager.Instance.PlayWinMusic();
            }
            LevelManager.Instance?.NextLevel();
        }

        private void OnNextClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Restart();
        }

        private void OnRestartClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Restart();
        }

        private void OnMenuClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.ReturnToMenu();
        }
    }
}
