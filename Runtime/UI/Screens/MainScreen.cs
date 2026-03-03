using UnityEngine.UIElements;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Core.UI.Screens
{
    /// <summary>Main menu screen — play, store, inventory, settings navigation.</summary>
    public class MainScreen : ScreenController
    {
        public override string ScreenName => "MainScreen";

        private Label gameTitleLabel;
        private Label softCurrencyLabel;
        private Label hardCurrencyLabel;

        protected override void OnBind()
        {
            Btn("play-btn")?.RegisterCallback<ClickEvent>(OnPlayClicked);
            Btn("store-btn")?.RegisterCallback<ClickEvent>(OnStoreClicked);
            Btn("inventory-btn")?.RegisterCallback<ClickEvent>(OnInventoryClicked);
            Btn("settings-btn")?.RegisterCallback<ClickEvent>(OnSettingsClicked);

            gameTitleLabel = Lbl("game-title");
            softCurrencyLabel = Lbl("soft-currency");
            hardCurrencyLabel = Lbl("hard-currency");
        }

        public override void OnShow()
        {
            RefreshCurrency();
            AudioManager.Instance?.PlayMenuMusic();
        }

        public void SetTitle(string title)
        {
            if (gameTitleLabel != null) gameTitleLabel.text = title;
        }

        private void RefreshCurrency()
        {
            if (ServiceLocator.TryGet<ICurrencyService>(out var currency))
            {
                if (softCurrencyLabel != null) softCurrencyLabel.text = currency.SoftCurrency.ToString();
                if (hardCurrencyLabel != null) hardCurrencyLabel.text = currency.HardCurrency.ToString();
            }
        }

        private void OnPlayClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            GameManager.Instance?.ChangeState(GameState.Prepare);
        }

        private void OnStoreClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            UIToolkitManager.Instance?.ShowScreen("StoreScreen");
        }

        private void OnInventoryClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            UIToolkitManager.Instance?.ShowScreen("InventoryScreen");
        }

        private void OnSettingsClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            UIToolkitManager.Instance?.ShowScreen("SettingsScreen");
        }
    }
}
