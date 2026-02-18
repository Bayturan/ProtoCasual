using System;

namespace ProtoCasual.Core.Interfaces
{
    public interface IIAPService
    {
        bool IsInitialized { get; }
        void Initialize(Action<bool> onComplete);
        void PurchaseProduct(string productId, Action<bool> onComplete);
        void RestorePurchases(Action<bool> onComplete);
        bool IsProductOwned(string productId);
    }
}
