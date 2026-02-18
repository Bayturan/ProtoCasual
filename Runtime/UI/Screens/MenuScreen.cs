using UnityEngine;
using UnityEngine.UI;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.GameLoop;

namespace ProtoCasual.Core.UI
{
    public class MenuScreen : UIScreen
    {
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button storeButton;
        [SerializeField] private Button settingsButton;

        protected override void OnInitialize()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
            }

            if (storeButton != null)
            {
                storeButton.onClick.AddListener(OnStoreClicked);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }
        }

        private void OnPlayClicked()
        {
            GameManager.Instance.ChangeState(GameState.Prepare);
        }

        private void OnStoreClicked()
        {
            UIManager.Instance.ShowScreen("Store");
        }

        private void OnSettingsClicked()
        {
            UIManager.Instance.ShowScreen("Settings");
        }
    }
}
