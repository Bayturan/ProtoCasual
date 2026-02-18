using System;
using UnityEngine;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Currency
{
    /// <summary>
    /// Dual-currency wallet backed by <see cref="PlayerData.Currency"/>.
    /// Auto-saves through <see cref="ISaveService"/> after every mutation.
    /// Register via ServiceLocator as <see cref="ICurrencyService"/>.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        public event Action OnCurrencyChanged;

        private CurrencyData data;
        private readonly ISaveService saveService;
        private readonly PlayerDataProvider dataProvider;

        public int SoftCurrency => data.SoftCurrency;
        public int HardCurrency => data.HardCurrency;

        public CurrencyService(PlayerDataProvider dataProvider, ISaveService saveService)
        {
            this.dataProvider = dataProvider;
            this.saveService = saveService;
            data = dataProvider.Data.Currency;
        }

        // ─── Soft ───────────────────────────────────────────────────────

        public void AddSoft(int amount)
        {
            if (amount <= 0) return;
            data.SoftCurrency += amount;
            Save();
        }

        public bool SpendSoft(int amount)
        {
            if (!HasSoft(amount)) return false;
            data.SoftCurrency -= amount;
            Save();
            return true;
        }

        public bool HasSoft(int amount) => data.SoftCurrency >= amount;

        // ─── Hard ───────────────────────────────────────────────────────

        public void AddHard(int amount)
        {
            if (amount <= 0) return;
            data.HardCurrency += amount;
            Save();
        }

        public bool SpendHard(int amount)
        {
            if (!HasHard(amount)) return false;
            data.HardCurrency -= amount;
            Save();
            return true;
        }

        public bool HasHard(int amount) => data.HardCurrency >= amount;

        // ─── Internal ───────────────────────────────────────────────────

        private void Save()
        {
            OnCurrencyChanged?.Invoke();
            dataProvider.Save();
        }
    }
}
