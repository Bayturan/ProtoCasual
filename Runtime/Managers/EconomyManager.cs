using UnityEngine;
using System.Collections.Generic;
using System;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Managers
{
    public class EconomyManager : MonoBehaviour
    {
        private static EconomyManager instance;
        public static EconomyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EconomyManager>();
                }
                return instance;
            }
        }

        public event Action<string, int> OnCurrencyChanged;
        
        private Dictionary<string, int> currencies = new Dictionary<string, int>();
        private ISaveService saveService;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            saveService = ServiceLocator.Get<ISaveService>();
            LoadCurrencies();
        }

        public void AddCurrency(string currencyType, int amount)
        {
            if (!currencies.ContainsKey(currencyType))
            {
                currencies[currencyType] = 0;
            }

            currencies[currencyType] += amount;
            OnCurrencyChanged?.Invoke(currencyType, currencies[currencyType]);
            SaveCurrencies();
        }

        public bool SpendCurrency(string currencyType, int amount)
        {
            if (!HasEnoughCurrency(currencyType, amount))
            {
                return false;
            }

            currencies[currencyType] -= amount;
            OnCurrencyChanged?.Invoke(currencyType, currencies[currencyType]);
            SaveCurrencies();
            return true;
        }

        public int GetCurrency(string currencyType)
        {
            return currencies.ContainsKey(currencyType) ? currencies[currencyType] : 0;
        }

        public bool HasEnoughCurrency(string currencyType, int amount)
        {
            return GetCurrency(currencyType) >= amount;
        }

        private void LoadCurrencies()
        {
            if (saveService != null)
            {
                currencies["Coins"] = saveService.Load("Currency_Coins", 0);
                currencies["Gems"] = saveService.Load("Currency_Gems", 0);
            }
            else
            {
                currencies["Coins"] = 1000; // Default
                currencies["Gems"] = 50;
            }
        }

        private void SaveCurrencies()
        {
            if (saveService != null)
            {
                foreach (var currency in currencies)
                {
                    saveService.Save($"Currency_{currency.Key}", currency.Value);
                }
            }
        }
    }
}
