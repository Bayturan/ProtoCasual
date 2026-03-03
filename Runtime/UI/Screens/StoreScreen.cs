using UnityEngine;
using UnityEngine.UIElements;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.Store;

namespace ProtoCasual.Core.UI.Screens
{
    /// <summary>Store screen — browse items, purchase with soft/hard currency.</summary>
    public class StoreScreen : ScreenController
    {
        public override string ScreenName => "StoreScreen";

        private Label softCurrencyLabel;
        private Label hardCurrencyLabel;
        private ScrollView itemList;
        private VisualElement detailPanel;
        private Label detailName;
        private Label detailDescription;
        private Label detailPrice;
        private Button buySoftBtn;
        private Button buyHardBtn;
        private Label feedbackLabel;

        private IStoreService storeService;
        private ICurrencyService currencyService;
        private IInventoryService inventoryService;
        private string selectedItemId;

        protected override void OnBind()
        {
            ServiceLocator.TryGet<IStoreService>(out storeService);
            ServiceLocator.TryGet<ICurrencyService>(out currencyService);
            ServiceLocator.TryGet<IInventoryService>(out inventoryService);

            Btn("close-btn")?.RegisterCallback<ClickEvent>(OnCloseClicked);

            softCurrencyLabel = Lbl("soft-currency");
            hardCurrencyLabel = Lbl("hard-currency");
            itemList = SView("item-list");
            detailPanel = Q("detail-panel");
            detailName = Lbl("detail-name");
            detailDescription = Lbl("detail-description");
            detailPrice = Lbl("detail-price");
            buySoftBtn = Btn("buy-soft-btn");
            buyHardBtn = Btn("buy-hard-btn");
            feedbackLabel = Lbl("feedback-text");

            buySoftBtn?.RegisterCallback<ClickEvent>(_ => TryPurchase(false));
            buyHardBtn?.RegisterCallback<ClickEvent>(_ => TryPurchase(true));

            if (storeService != null) storeService.OnPurchaseCompleted += HandlePurchase;
            if (currencyService != null) currencyService.OnCurrencyChanged += RefreshCurrency;
        }

        public override void OnShow()
        {
            RefreshCurrency();
            PopulateItems();
            HideDetail();
            SetFeedback("");
        }

        public override void OnHide() { selectedItemId = null; }

        private void PopulateItems()
        {
            if (itemList == null) return;
            itemList.Clear();

            if (!ServiceLocator.TryGet<ItemDatabase>(out var db)) return;

            foreach (var item in db.All)
            {
                if (item == null) continue;

                var card = new VisualElement();
                card.AddToClassList("store-item-card");

                var nameLabel = new Label(item.DisplayName);
                nameLabel.AddToClassList("item-name");
                card.Add(nameLabel);

                string priceText = item.SoftCurrencyPrice > 0
                    ? $"{item.SoftCurrencyPrice} Coins"
                    : $"{item.HardCurrencyPrice} Gems";
                var priceLbl = new Label(priceText);
                priceLbl.AddToClassList("item-price");
                card.Add(priceLbl);

                bool owned = !item.IsStackable && inventoryService != null && inventoryService.HasItem(item.Id);
                if (owned) card.AddToClassList("item-owned");

                string capturedId = item.Id;
                card.RegisterCallback<ClickEvent>(_ => SelectItem(capturedId));
                itemList.Add(card);
            }
        }

        private void SelectItem(string itemId)
        {
            selectedItemId = itemId;
            if (!ServiceLocator.TryGet<ItemDatabase>(out var db)) return;
            var item = db.Get(itemId);
            if (item == null) return;

            if (detailPanel != null) detailPanel.style.display = DisplayStyle.Flex;
            if (detailName != null) detailName.text = item.DisplayName;
            if (detailDescription != null) detailDescription.text = item.Description;
            if (detailPrice != null)
            {
                detailPrice.text = item.SoftCurrencyPrice > 0
                    ? $"{item.SoftCurrencyPrice} Coins"
                    : $"{item.HardCurrencyPrice} Gems";
            }

            bool canBuy = storeService?.CanPurchase(itemId) ?? false;
            if (buySoftBtn != null) buySoftBtn.SetEnabled(canBuy && item.SoftCurrencyPrice > 0);
            if (buyHardBtn != null) buyHardBtn.SetEnabled(canBuy && item.HardCurrencyPrice > 0);
            SetFeedback("");
        }

        private void HideDetail()
        {
            if (detailPanel != null) detailPanel.style.display = DisplayStyle.None;
            selectedItemId = null;
        }

        private void TryPurchase(bool preferHard)
        {
            if (string.IsNullOrEmpty(selectedItemId) || storeService == null) return;
            if (!storeService.TryPurchase(selectedItemId, preferHard))
                SetFeedback("Not enough currency!");
        }

        private void HandlePurchase(PurchaseResult result)
        {
            SetFeedback($"Purchased {result.ItemId}!");
            RefreshCurrency();
            PopulateItems();
            if (selectedItemId == result.ItemId) SelectItem(selectedItemId);
        }

        private void RefreshCurrency()
        {
            if (currencyService == null) return;
            if (softCurrencyLabel != null) softCurrencyLabel.text = currencyService.SoftCurrency.ToString();
            if (hardCurrencyLabel != null) hardCurrencyLabel.text = currencyService.HardCurrency.ToString();
        }

        private void SetFeedback(string msg)
        {
            if (feedbackLabel != null) feedbackLabel.text = msg;
        }

        private void OnCloseClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            UIToolkitManager.Instance?.ShowScreen("MainScreen");
        }
    }
}
