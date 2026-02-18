using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Store;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Displays items from <see cref="ItemDatabase"/> and lets the player purchase
    /// through <see cref="IStoreService"/>. Resolves services from ServiceLocator.
    /// </summary>
    public class StoreScreen : UIScreen
    {
        [Header("References")]
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("Currency Display")]
        [SerializeField] private TextMeshProUGUI softCurrencyText;
        [SerializeField] private TextMeshProUGUI hardCurrencyText;

        [Header("Item List")]
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject storeItemPrefab;

        [Header("Detail Panel")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private Image detailIcon;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private TextMeshProUGUI detailPrice;
        [SerializeField] private Button buySoftButton;
        [SerializeField] private Button buyHardButton;

        [Header("Navigation")]
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI feedbackText;

        private IStoreService storeService;
        private ICurrencyService currencyService;
        private IInventoryService inventoryService;

        private readonly List<GameObject> spawnedItems = new();
        private string selectedItemId;

        // ─── Lifecycle ──────────────────────────────────────────────────

        protected override void OnInitialize()
        {
            storeService    = ServiceLocator.Get<IStoreService>();
            currencyService = ServiceLocator.Get<ICurrencyService>();
            inventoryService = ServiceLocator.Get<IInventoryService>();

            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);

            if (buySoftButton != null)
                buySoftButton.onClick.AddListener(() => TryPurchase(false));

            if (buyHardButton != null)
                buyHardButton.onClick.AddListener(() => TryPurchase(true));

            if (storeService != null)
                storeService.OnPurchaseCompleted += HandlePurchaseCompleted;

            if (currencyService != null)
                currencyService.OnCurrencyChanged += RefreshCurrencyUI;

            HideDetail();
        }

        protected override void OnShow()
        {
            RefreshCurrencyUI();
            PopulateItems();
            HideDetail();
            SetFeedback(string.Empty);
        }

        protected override void OnHide()
        {
            selectedItemId = null;
        }

        private void OnDestroy()
        {
            if (storeService != null)
                storeService.OnPurchaseCompleted -= HandlePurchaseCompleted;

            if (currencyService != null)
                currencyService.OnCurrencyChanged -= RefreshCurrencyUI;
        }

        // ─── Item List ─────────────────────────────────────────────────

        private void PopulateItems()
        {
            ClearSpawnedItems();

            if (itemDatabase == null || storeItemPrefab == null || itemContainer == null)
                return;

            foreach (var item in itemDatabase.All)
            {
                if (item == null) continue;

                var go = Instantiate(storeItemPrefab, itemContainer);
                spawnedItems.Add(go);

                // Icon
                var icon = go.GetComponentInChildren<Image>();
                if (icon != null && item.Icon != null)
                    icon.sprite = item.Icon;

                // Name label
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = item.DisplayName;

                // Owned badge — grey-out non-stackable items already owned
                bool owned = !item.IsStackable && inventoryService != null && inventoryService.HasItem(item.Id);
                if (owned)
                {
                    var cg = go.GetComponent<CanvasGroup>();
                    if (cg == null) cg = go.AddComponent<CanvasGroup>();
                    cg.alpha = 0.5f;
                }

                // Select button
                string capturedId = item.Id;
                var btn = go.GetComponent<Button>();
                if (btn == null) btn = go.AddComponent<Button>();
                btn.onClick.AddListener(() => SelectItem(capturedId));
            }
        }

        private void ClearSpawnedItems()
        {
            foreach (var go in spawnedItems)
            {
                if (go != null) Destroy(go);
            }
            spawnedItems.Clear();
        }

        // ─── Detail Panel ───────────────────────────────────────────────

        private void SelectItem(string itemId)
        {
            selectedItemId = itemId;
            var item = itemDatabase.Get(itemId);
            if (item == null) return;

            if (detailPanel != null) detailPanel.SetActive(true);
            if (detailIcon != null) detailIcon.sprite = item.Icon;
            if (detailName != null) detailName.text = item.DisplayName;
            if (detailDescription != null) detailDescription.text = item.Description;

            // Price labels
            if (detailPrice != null)
            {
                string price = item.SoftCurrencyPrice > 0
                    ? $"{item.SoftCurrencyPrice} Coins"
                    : $"{item.HardCurrencyPrice} Gems";
                detailPrice.text = price;
            }

            // Button states
            bool canBuy = storeService != null && storeService.CanPurchase(itemId);
            if (buySoftButton != null)
                buySoftButton.interactable = canBuy && item.SoftCurrencyPrice > 0;
            if (buyHardButton != null)
                buyHardButton.interactable = canBuy && item.HardCurrencyPrice > 0;

            SetFeedback(string.Empty);
        }

        private void HideDetail()
        {
            if (detailPanel != null) detailPanel.SetActive(false);
            selectedItemId = null;
        }

        // ─── Purchase ───────────────────────────────────────────────────

        private void TryPurchase(bool preferHard)
        {
            if (string.IsNullOrEmpty(selectedItemId) || storeService == null)
                return;

            bool success = storeService.TryPurchase(selectedItemId, preferHard);
            if (!success)
            {
                SetFeedback("Not enough currency!");
            }
        }

        private void HandlePurchaseCompleted(PurchaseResult result)
        {
            SetFeedback($"Purchased {result.ItemId}!");
            RefreshCurrencyUI();
            PopulateItems();

            // Re-select so detail panel updates button states
            if (selectedItemId == result.ItemId)
                SelectItem(selectedItemId);
        }

        // ─── Currency Display ───────────────────────────────────────────

        private void RefreshCurrencyUI()
        {
            if (currencyService == null) return;
            if (softCurrencyText != null) softCurrencyText.text = currencyService.SoftCurrency.ToString();
            if (hardCurrencyText != null) hardCurrencyText.text = currencyService.HardCurrency.ToString();
        }

        // ─── Helpers ────────────────────────────────────────────────────

        private void SetFeedback(string message)
        {
            if (feedbackText != null) feedbackText.text = message;
        }

        private void OnCloseClicked()
        {
            UIManager.Instance.ShowScreen("MenuScreen");
        }
    }
}
