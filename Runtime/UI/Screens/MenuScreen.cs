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
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button settingsButton;

        protected override void OnInitialize()
        {
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);

            if (storeButton != null)
                storeButton.onClick.AddListener(OnStoreClicked);

            if (inventoryButton != null)
                inventoryButton.onClick.AddListener(OnInventoryClicked);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        protected override void OnShow()
        {
            // Play menu music when entering menu
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMenuMusic();
        }

        private void OnPlayClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance.ChangeState(GameState.Prepare);
        }

        private void OnStoreClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            UIManager.Instance.ShowScreen(nameof(StoreScreen));
        }

        private void OnInventoryClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            UIManager.Instance.ShowScreen(nameof(InventoryScreen));
        }

        private void OnSettingsClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            UIManager.Instance.ShowScreen(nameof(SettingsScreen));
        }
    }
}
