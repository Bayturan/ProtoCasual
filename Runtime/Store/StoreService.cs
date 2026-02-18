using System;
using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Store
{
    /// <summary>
    /// Orchestrates a purchase: validates price via <see cref="ItemDatabase"/>,
    /// deducts currency via <see cref="ICurrencyService"/>,
    /// adds item via <see cref="IInventoryService"/>.
    /// No direct IAP SDK references.
    /// </summary>
    public class StoreService : IStoreService
    {
        public event Action<PurchaseResult> OnPurchaseCompleted;

        private readonly ItemDatabase itemDatabase;
        private readonly ICurrencyService currencyService;
        private readonly IInventoryService inventoryService;

        public StoreService(ItemDatabase itemDatabase, ICurrencyService currencyService, IInventoryService inventoryService)
        {
            this.itemDatabase = itemDatabase;
            this.currencyService = currencyService;
            this.inventoryService = inventoryService;
        }

        /// <summary>
        /// Returns true if the player can afford the item (soft or hard currency).
        /// </summary>
        public bool CanPurchase(string itemId)
        {
            var item = itemDatabase.Get(itemId);
            if (item == null) return false;

            // Stackable items can always be bought again; non-stackable only if not owned
            if (!item.IsStackable && inventoryService.HasItem(itemId))
                return false;

            return currencyService.HasSoft(item.SoftCurrencyPrice)
                || currencyService.HasHard(item.HardCurrencyPrice);
        }

        /// <summary>
        /// Attempt a purchase. Returns true on success.
        /// When <paramref name="preferHardCurrency"/> is true, tries hard currency first.
        /// </summary>
        public bool TryPurchase(string itemId, bool preferHardCurrency = false)
        {
            var item = itemDatabase.Get(itemId);
            if (item == null)
            {
                Debug.LogWarning($"[StoreService] Item '{itemId}' not found in database.");
                return false;
            }

            // Non-stackable duplicate check
            if (!item.IsStackable && inventoryService.HasItem(itemId))
            {
                Debug.Log($"[StoreService] Already owns non-stackable '{itemId}'.");
                return false;
            }

            bool usedHard = false;
            int pricePaid = 0;

            if (preferHardCurrency && item.HardCurrencyPrice > 0 && currencyService.HasHard(item.HardCurrencyPrice))
            {
                currencyService.SpendHard(item.HardCurrencyPrice);
                usedHard = true;
                pricePaid = item.HardCurrencyPrice;
            }
            else if (item.SoftCurrencyPrice > 0 && currencyService.HasSoft(item.SoftCurrencyPrice))
            {
                currencyService.SpendSoft(item.SoftCurrencyPrice);
                pricePaid = item.SoftCurrencyPrice;
            }
            else if (item.HardCurrencyPrice > 0 && currencyService.HasHard(item.HardCurrencyPrice))
            {
                currencyService.SpendHard(item.HardCurrencyPrice);
                usedHard = true;
                pricePaid = item.HardCurrencyPrice;
            }
            else
            {
                Debug.Log($"[StoreService] Not enough currency for '{itemId}'.");
                return false;
            }

            // Grant item
            inventoryService.AddItem(itemId, 1);

            var result = new PurchaseResult
            {
                ItemId = itemId,
                UsedHardCurrency = usedHard,
                PricePaid = pricePaid
            };

            OnPurchaseCompleted?.Invoke(result);
            Debug.Log($"[StoreService] Purchased '{itemId}' for {pricePaid} {(usedHard ? "hard" : "soft")} currency.");
            return true;
        }
    }
}
