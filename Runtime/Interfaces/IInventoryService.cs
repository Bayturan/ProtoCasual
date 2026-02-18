using System;
using System.Collections.Generic;
using ProtoCasual.Core.Data;

namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// ID-based inventory operations. No ScriptableObject references in the API.
    /// </summary>
    public interface IInventoryService
    {
        event Action OnInventoryChanged;

        void AddItem(string itemId, int amount = 1);
        bool RemoveItem(string itemId, int amount = 1);
        bool HasItem(string itemId);
        int GetQuantity(string itemId);
        void Clear();
        IReadOnlyList<InventoryItemData> GetAll();
    }
}
