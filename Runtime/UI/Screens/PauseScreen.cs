using UnityEngine;
using UnityEngine.UI;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.GameLoop;

namespace ProtoCasual.Core.UI
{
    public class PauseScreen : UIScreen
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        protected override void OnInitialize()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeClicked);
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

        private void OnResumeClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance.Resume();
        }

        private void OnRestartClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance.Restart();
        }

        private void OnMenuClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance.ReturnToMenu();
        }
    }
}
