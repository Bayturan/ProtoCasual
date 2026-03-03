using UnityEngine.UIElements;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.Store;

namespace ProtoCasual.Core.UI.Screens
{
    /// <summary>Inventory screen — view owned items with detail panel.</summary>
    public class InventoryScreen : ScreenController
    {
        public override string ScreenName => "InventoryScreen";

        private ScrollView itemList;
        private VisualElement detailPanel;
        private Label detailName;
        private Label detailDescription;
        private Label detailQuantity;
        private Label detailType;
        private VisualElement emptyState;

        private IInventoryService inventoryService;
        private string selectedItemId;

        protected override void OnBind()
        {
            ServiceLocator.TryGet<IInventoryService>(out inventoryService);
            Btn("close-btn")?.RegisterCallback<ClickEvent>(OnCloseClicked);

            itemList = SView("item-list");
            detailPanel = Q("detail-panel");
            detailName = Lbl("detail-name");
            detailDescription = Lbl("detail-description");
            detailQuantity = Lbl("detail-quantity");
            detailType = Lbl("detail-type");
            emptyState = Q("empty-state");

            if (inventoryService != null)
                inventoryService.OnInventoryChanged += RefreshList;
        }

        public override void OnShow() { RefreshList(); HideDetail(); }
        public override void OnHide() { selectedItemId = null; }

        private void RefreshList()
        {
            if (itemList == null) return;
            itemList.Clear();

            var items = inventoryService?.GetAll();
            bool empty = items == null || items.Count == 0;

            if (emptyState != null)
                emptyState.style.display = empty ? DisplayStyle.Flex : DisplayStyle.None;
            if (empty) return;

            ServiceLocator.TryGet<ItemDatabase>(out var db);

            foreach (var entry in items)
            {
                if (entry == null || string.IsNullOrEmpty(entry.ItemId)) continue;

                var config = db?.Get(entry.ItemId);
                var card = new VisualElement();
                card.AddToClassList("inventory-item-card");

                string displayName = config != null ? config.DisplayName : entry.ItemId;
                var nameLabel = new Label(entry.Quantity > 1 ? $"{displayName}  x{entry.Quantity}" : displayName);
                nameLabel.AddToClassList("item-name");
                card.Add(nameLabel);

                string capturedId = entry.ItemId;
                card.RegisterCallback<ClickEvent>(_ => SelectItem(capturedId));
                itemList.Add(card);
            }
        }

        private void SelectItem(string itemId)
        {
            selectedItemId = itemId;
            ServiceLocator.TryGet<ItemDatabase>(out var db);
            var config = db?.Get(itemId);
            int quantity = inventoryService?.GetQuantity(itemId) ?? 0;

            if (detailPanel != null) detailPanel.style.display = DisplayStyle.Flex;
            if (detailName != null) detailName.text = config?.DisplayName ?? itemId;
            if (detailDescription != null) detailDescription.text = config?.Description ?? "";
            if (detailQuantity != null) detailQuantity.text = $"Owned: {quantity}";
            if (detailType != null) detailType.text = config?.Type.ToString() ?? "";
        }

        private void HideDetail()
        {
            if (detailPanel != null) detailPanel.style.display = DisplayStyle.None;
            selectedItemId = null;
        }

        private void OnCloseClicked(ClickEvent evt)
        {
            AudioManager.Instance?.PlayButtonClick();
            UIToolkitManager.Instance?.ShowScreen("MainScreen");
        }
    }
}
