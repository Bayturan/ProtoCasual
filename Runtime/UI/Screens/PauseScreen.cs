using UnityEngine.UIElements;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Core.UI.Screens
{
    /// <summary>Pause overlay — resume, restart, quit to menu.</summary>
    public class PauseScreen : ScreenController
    {
        public override string ScreenName => "PauseScreen";

        protected override void OnBind()
        {
            Btn("resume-btn")?.RegisterCallback<ClickEvent>(OnResumeClicked);
            Btn("restart-btn")?.RegisterCallback<ClickEvent>(OnRestartClicked);
            Btn("menu-btn")?.RegisterCallback<ClickEvent>(OnMenuClicked);
        }

        private void OnResumeClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.Resume();
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
