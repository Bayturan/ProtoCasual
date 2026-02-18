using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Managers
{
    [Serializable]
    public class InventoryData
    {
        public List<string> ownedItemIds = new List<string>();
        public Dictionary<string, int> consumableAmounts = new Dictionary<string, int>();
    }

    public class InventoryManager : MonoBehaviour
    {
        private static InventoryManager instance;
        public static InventoryManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<InventoryManager>();
                }
                return instance;
            }
        }

        [SerializeField] private ItemConfig[] allItems;

        public event Action<ItemConfig> OnItemAdded;
        public event Action<ItemConfig, int> OnConsumableUsed;

        private InventoryData inventoryData = new InventoryData();
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
            LoadInventory();
        }

        public bool OwnsItem(string itemId)
        {
            return inventoryData.ownedItemIds.Contains(itemId);
        }

        public void AddItem(ItemConfig item)
        {
            if (item.itemType == ItemType.Consumable)
            {
                if (!inventoryData.consumableAmounts.ContainsKey(item.itemId))
                {
                    inventoryData.consumableAmounts[item.itemId] = 0;
                }
                inventoryData.consumableAmounts[item.itemId]++;
            }
            else
            {
                if (!inventoryData.ownedItemIds.Contains(item.itemId))
                {
                    inventoryData.ownedItemIds.Add(item.itemId);
                }
            }

            OnItemAdded?.Invoke(item);
            SaveInventory();
        }

        public bool UseConsumable(string itemId)
        {
            if (!inventoryData.consumableAmounts.ContainsKey(itemId) || 
                inventoryData.consumableAmounts[itemId] <= 0)
            {
                return false;
            }

            inventoryData.consumableAmounts[itemId]--;
            
            var item = GetItemConfig(itemId);
            if (item != null)
            {
                OnConsumableUsed?.Invoke(item, inventoryData.consumableAmounts[itemId]);
            }

            SaveInventory();
            return true;
        }

        public int GetConsumableAmount(string itemId)
        {
            return inventoryData.consumableAmounts.ContainsKey(itemId) 
                ? inventoryData.consumableAmounts[itemId] 
                : 0;
        }

        public List<ItemConfig> GetOwnedItems()
        {
            return allItems.Where(item => OwnsItem(item.itemId)).ToList();
        }

        public ItemConfig GetItemConfig(string itemId)
        {
            return allItems.FirstOrDefault(item => item.itemId == itemId);
        }

        private void LoadInventory()
        {
            if (saveService != null)
            {
                inventoryData = saveService.Load("Inventory", new InventoryData());
            }
        }

        private void SaveInventory()
        {
            if (saveService != null)
            {
                saveService.Save("Inventory", inventoryData);
            }
        }
    }
}
