using UnityEngine;
using System.Collections.Generic;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Store
{
    /// <summary>
    /// Runtime catalogue of all items. Loads from a Resources folder or from a
    /// serialised array assigned in the Inspector.
    /// Provides O(1) lookup by ID. Validates no duplicate IDs on load.
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Economy/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private ItemConfig[] items = new ItemConfig[0];

        private Dictionary<string, ItemConfig> lookup;

        /// <summary>All items in the database.</summary>
        public IReadOnlyList<ItemConfig> All => items;

        /// <summary>
        /// Call once at startup (or use <see cref="Get"/> which auto-initialises).
        /// </summary>
        public void Initialise()
        {
            lookup = new Dictionary<string, ItemConfig>(items.Length);
            foreach (var item in items)
            {
                if (item == null) continue;
                if (string.IsNullOrEmpty(item.Id))
                {
                    Debug.LogWarning($"[ItemDatabase] Item '{item.name}' has empty Id — skipped.");
                    continue;
                }
                if (lookup.ContainsKey(item.Id))
                {
                    Debug.LogError($"[ItemDatabase] Duplicate Id '{item.Id}' on '{item.name}' — skipped.");
                    continue;
                }
                lookup.Add(item.Id, item);
            }
            Debug.Log($"[ItemDatabase] Loaded {lookup.Count} items.");
        }

        /// <summary>
        /// Returns the <see cref="ItemConfig"/> for <paramref name="id"/>, or null.
        /// </summary>
        public ItemConfig Get(string id)
        {
            if (lookup == null) Initialise();
            return lookup.TryGetValue(id, out var cfg) ? cfg : null;
        }

        /// <summary>
        /// Returns true if an item with <paramref name="id"/> exists.
        /// </summary>
        public bool Contains(string id)
        {
            if (lookup == null) Initialise();
            return lookup.ContainsKey(id);
        }

        /// <summary>
        /// Returns all items matching <paramref name="category"/>.
        /// </summary>
        public List<ItemConfig> GetByCategory(string category)
        {
            if (lookup == null) Initialise();
            var result = new List<ItemConfig>();
            foreach (var item in items)
            {
                if (item != null && item.Category == category)
                    result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Returns all items of a given <see cref="ItemType"/>.
        /// </summary>
        public List<ItemConfig> GetByType(ItemType type)
        {
            if (lookup == null) Initialise();
            var result = new List<ItemConfig>();
            foreach (var item in items)
            {
                if (item != null && item.Type == type)
                    result.Add(item);
            }
            return result;
        }
    }
}
