using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Store;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Displays the player's owned items from <see cref="IInventoryService"/>
    /// and resolves display data from <see cref="ItemDatabase"/>.
    /// Refreshes automatically when the inventory changes.
    /// </summary>
    public class InventoryScreen : UIScreen
    {
        [Header("References")]
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("Item List")]
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject inventoryItemPrefab;

        [Header("Detail Panel")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private Image detailIcon;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private TextMeshProUGUI detailQuantity;
        [SerializeField] private TextMeshProUGUI detailType;

        [Header("Empty State")]
        [SerializeField] private GameObject emptyStatePanel;
        [SerializeField] private TextMeshProUGUI emptyStateText;

        [Header("Navigation")]
        [SerializeField] private Button closeButton;

        private IInventoryService inventoryService;
        private readonly List<GameObject> spawnedItems = new();
        private string selectedItemId;

        // ─── Lifecycle ──────────────────────────────────────────────────

        protected override void OnInitialize()
        {
            inventoryService = ServiceLocator.Get<IInventoryService>();

            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);

            if (inventoryService != null)
                inventoryService.OnInventoryChanged += RefreshList;

            HideDetail();
        }

        protected override void OnShow()
        {
            RefreshList();
            HideDetail();
        }

        protected override void OnHide()
        {
            selectedItemId = null;
        }

        private void OnDestroy()
        {
            if (inventoryService != null)
                inventoryService.OnInventoryChanged -= RefreshList;
        }

        // ─── Item List ─────────────────────────────────────────────────

        private void RefreshList()
        {
            ClearSpawnedItems();

            if (inventoryService == null || inventoryItemPrefab == null || itemContainer == null)
                return;

            var items = inventoryService.GetAll();

            // Empty state
            bool empty = items == null || items.Count == 0;
            if (emptyStatePanel != null) emptyStatePanel.SetActive(empty);
            if (empty)
            {
                if (emptyStateText != null) emptyStateText.text = "No items yet. Visit the Store!";
                return;
            }

            if (emptyStatePanel != null) emptyStatePanel.SetActive(false);

            foreach (var entry in items)
            {
                if (entry == null || string.IsNullOrEmpty(entry.ItemId)) continue;

                var go = Instantiate(inventoryItemPrefab, itemContainer);
                spawnedItems.Add(go);

                // Try to resolve display info from ItemDatabase
                ItemConfig config = itemDatabase != null ? itemDatabase.Get(entry.ItemId) : null;

                // Icon
                var icon = go.GetComponentInChildren<Image>();
                if (icon != null && config != null && config.Icon != null)
                    icon.sprite = config.Icon;

                // Label — show display name + quantity
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    string displayName = config != null ? config.DisplayName : entry.ItemId;
                    label.text = entry.Quantity > 1
                        ? $"{displayName}  x{entry.Quantity}"
                        : displayName;
                }

                // Select button
                string capturedId = entry.ItemId;
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

            ItemConfig config = itemDatabase != null ? itemDatabase.Get(itemId) : null;
            int quantity = inventoryService != null ? inventoryService.GetQuantity(itemId) : 0;

            if (detailPanel != null) detailPanel.SetActive(true);

            if (detailIcon != null)
                detailIcon.sprite = config != null ? config.Icon : null;

            if (detailName != null)
                detailName.text = config != null ? config.DisplayName : itemId;

            if (detailDescription != null)
                detailDescription.text = config != null ? config.Description : string.Empty;

            if (detailQuantity != null)
                detailQuantity.text = $"Owned: {quantity}";

            if (detailType != null)
                detailType.text = config != null ? config.Type.ToString() : string.Empty;
        }

        private void HideDetail()
        {
            if (detailPanel != null) detailPanel.SetActive(false);
            selectedItemId = null;
        }

        // ─── Navigation ─────────────────────────────────────────────────

        private void OnCloseClicked()
        {
            UIManager.Instance.ShowScreen("MenuScreen");
        }
    }
}
