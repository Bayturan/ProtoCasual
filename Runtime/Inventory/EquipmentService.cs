using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Inventory
{
    /// <summary>
    /// Manages equipment slots, allowing items to be equipped/unequipped.
    /// Works alongside InventoryService — equipping consumes an inventory item,
    /// unequipping returns it.
    /// </summary>
    public class EquipmentService : IEquipmentService
    {
        public event Action OnEquipmentChanged;

        private readonly EquipmentData data;
        private readonly PlayerDataProvider dataProvider;
        private readonly IInventoryService inventoryService;
        private readonly Dictionary<string, EquipmentSlot> lookup = new();

        public EquipmentService(PlayerDataProvider dataProvider, IInventoryService inventoryService = null)
        {
            this.dataProvider = dataProvider;
            this.inventoryService = inventoryService;
            data = dataProvider.Data.Equipment;
            RebuildLookup();
        }

        public bool Equip(string slotName, string itemId)
        {
            if (string.IsNullOrEmpty(slotName) || string.IsNullOrEmpty(itemId)) return false;

            // Check inventory has the item
            if (inventoryService != null && !inventoryService.HasItem(itemId))
            {
                Debug.LogWarning($"[EquipmentService] Item '{itemId}' not in inventory.");
                return false;
            }

            // Unequip current item in this slot first
            if (lookup.TryGetValue(slotName, out var existing) && !string.IsNullOrEmpty(existing.ItemId))
            {
                inventoryService?.AddItem(existing.ItemId, 1);
            }

            // Equip new item
            var slot = GetOrCreateSlot(slotName);
            slot.ItemId = itemId;
            inventoryService?.RemoveItem(itemId, 1);

            Save();
            return true;
        }

        public bool Unequip(string slotName)
        {
            if (string.IsNullOrEmpty(slotName)) return false;
            if (!lookup.TryGetValue(slotName, out var slot)) return false;
            if (string.IsNullOrEmpty(slot.ItemId)) return false;

            inventoryService?.AddItem(slot.ItemId, 1);
            slot.ItemId = string.Empty;

            Save();
            return true;
        }

        public string GetEquipped(string slotName)
        {
            if (string.IsNullOrEmpty(slotName)) return null;
            return lookup.TryGetValue(slotName, out var slot) ? slot.ItemId : null;
        }

        public bool IsSlotEmpty(string slotName)
        {
            if (!lookup.TryGetValue(slotName, out var slot)) return true;
            return string.IsNullOrEmpty(slot.ItemId);
        }

        public IReadOnlyList<EquipmentSlot> GetAll() => data.Slots.AsReadOnly();

        public void Clear()
        {
            // Return all equipped items to inventory
            foreach (var slot in data.Slots)
            {
                if (!string.IsNullOrEmpty(slot.ItemId))
                    inventoryService?.AddItem(slot.ItemId, 1);
            }

            data.Slots.Clear();
            lookup.Clear();
            Save();
        }

        private EquipmentSlot GetOrCreateSlot(string slotName)
        {
            if (lookup.TryGetValue(slotName, out var existing))
                return existing;

            var slot = new EquipmentSlot(slotName, string.Empty);
            data.Slots.Add(slot);
            lookup[slotName] = slot;
            return slot;
        }

        private void RebuildLookup()
        {
            lookup.Clear();
            foreach (var slot in data.Slots)
            {
                if (slot != null && !string.IsNullOrEmpty(slot.SlotName))
                    lookup[slot.SlotName] = slot;
            }
        }

        private void Save()
        {
            OnEquipmentChanged?.Invoke();
            dataProvider.Save();
        }
    }
}
