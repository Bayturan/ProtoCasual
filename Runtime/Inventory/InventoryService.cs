using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Inventory
{
    /// <summary>
    /// ID-based inventory backed by <see cref="PlayerData.Inventory"/>.
    /// Auto-saves after every mutation. No ScriptableObject references stored.
    /// Register as <see cref="IInventoryService"/> in ServiceLocator.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        public event Action OnInventoryChanged;

        private readonly InventoryData data;
        private readonly PlayerDataProvider dataProvider;

        // O(1) lookup cache — rebuilt on Load
        private readonly Dictionary<string, InventoryItemData> lookup = new();

        public InventoryService(PlayerDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            data = dataProvider.Data.Inventory;
            RebuildLookup();
        }

        // ─── Public API ─────────────────────────────────────────────────

        public void AddItem(string itemId, int amount = 1)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0) return;

            if (lookup.TryGetValue(itemId, out var entry))
            {
                entry.Quantity += amount;
            }
            else
            {
                entry = new InventoryItemData(itemId, amount);
                data.Items.Add(entry);
                lookup[itemId] = entry;
            }

            Save();
        }

        public bool RemoveItem(string itemId, int amount = 1)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;
            if (!lookup.TryGetValue(itemId, out var entry)) return false;
            if (entry.Quantity < amount) return false;

            entry.Quantity -= amount;
            if (entry.Quantity <= 0)
            {
                data.Items.Remove(entry);
                lookup.Remove(itemId);
            }

            Save();
            return true;
        }

        public bool HasItem(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && lookup.ContainsKey(itemId) && lookup[itemId].Quantity > 0;
        }

        public int GetQuantity(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return 0;
            return lookup.TryGetValue(itemId, out var entry) ? entry.Quantity : 0;
        }

        public void Clear()
        {
            data.Items.Clear();
            lookup.Clear();
            Save();
        }

        public IReadOnlyList<InventoryItemData> GetAll() => data.Items.AsReadOnly();

        // ─── Internal ───────────────────────────────────────────────────

        private void RebuildLookup()
        {
            lookup.Clear();
            foreach (var item in data.Items)
            {
                if (item != null && !string.IsNullOrEmpty(item.ItemId))
                    lookup[item.ItemId] = item;
            }
        }

        private void Save()
        {
            OnInventoryChanged?.Invoke();
            dataProvider.Save();
        }
    }
}
