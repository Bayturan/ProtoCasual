using System;

namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Purchase result passed with OnPurchaseCompleted.
    /// </summary>
    public class PurchaseResult
    {
        public string ItemId;
        public bool UsedHardCurrency;
        public int PricePaid;
    }

    /// <summary>
    /// Store facade — validates price, deducts currency, adds to inventory.
    /// Does NOT reference any IAP SDK.
    /// </summary>
    public interface IStoreService
    {
        event Action<PurchaseResult> OnPurchaseCompleted;

        bool CanPurchase(string itemId);
        bool TryPurchase(string itemId, bool preferHardCurrency = false);
    }
}
