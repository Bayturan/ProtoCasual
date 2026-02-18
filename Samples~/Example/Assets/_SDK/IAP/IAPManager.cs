using UnityEngine;
using System;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Managers
{
    public class IAPManager : MonoBehaviour, IIAPService
    {
        public bool IsInitialized { get; private set; }

        [Header("Product IDs")]
        [SerializeField] private string removeAdsProductId = "remove_ads";
        [SerializeField] private string coin100ProductId = "coins_100";
        [SerializeField] private string coin500ProductId = "coins_500";
        [SerializeField] private string coin1000ProductId = "coins_1000";

        public void Initialize(Action<bool> onComplete)
        {
            if (IsInitialized)
            {
                onComplete?.Invoke(true);
                return;
            }

            // Initialize IAP SDK (Unity IAP)
            Debug.Log("IAP Initializing...");
            IsInitialized = true;
            onComplete?.Invoke(true);
        }

        public void PurchaseProduct(string productId, Action<bool> onComplete)
        {
            if (!IsInitialized)
            {
                Debug.LogError("IAP not initialized");
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log($"Purchasing product: {productId}");
            
            // Process purchase
            ProcessPurchase(productId);
            onComplete?.Invoke(true);
        }

        public void RestorePurchases(Action<bool> onComplete)
        {
            if (!IsInitialized)
            {
                onComplete?.Invoke(false);
                return;
            }

            Debug.Log("Restoring purchases...");
            onComplete?.Invoke(true);
        }

        public bool IsProductOwned(string productId)
        {
            // Check if non-consumable product is owned
            return PlayerPrefs.GetInt($"IAP_{productId}", 0) == 1;
        }

        private void ProcessPurchase(string productId)
        {
            if (productId == removeAdsProductId)
            {
                PlayerPrefs.SetInt($"IAP_{productId}", 1);
                // Disable ads
            }
            else if (productId.StartsWith("coins_"))
            {
                // Grant coins
                int amount = int.Parse(productId.Split('_')[1]);
                EconomyManager.Instance?.AddCurrency("Coins", amount);
            }

            PlayerPrefs.Save();
        }
    }
}
