using UnityEngine;
using System;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Managers
{
    [Serializable]
    public class EquipmentData
    {
        public string equippedSkin;
        public string equippedPowerUp;
    }

    public class EquipmentManager : MonoBehaviour
    {
        private static EquipmentManager instance;
        public static EquipmentManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EquipmentManager>();
                }
                return instance;
            }
        }

        public event Action<string, ItemConfig> OnItemEquipped;
        
        private EquipmentData equipmentData = new EquipmentData();
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
            LoadEquipment();
        }

        public void EquipItem(ItemConfig item)
        {
            switch (item.itemType)
            {
                case ItemType.Skin:
                    equipmentData.equippedSkin = item.itemId;
                    break;
                case ItemType.PowerUp:
                    equipmentData.equippedPowerUp = item.itemId;
                    break;
            }

            OnItemEquipped?.Invoke(item.itemType.ToString(), item);
            SaveEquipment();
        }

        public string GetEquippedItem(ItemType type)
        {
            switch (type)
            {
                case ItemType.Skin:
                    return equipmentData.equippedSkin;
                case ItemType.PowerUp:
                    return equipmentData.equippedPowerUp;
                default:
                    return null;
            }
        }

        public bool IsEquipped(string itemId, ItemType type)
        {
            return GetEquippedItem(type) == itemId;
        }

        private void LoadEquipment()
        {
            if (saveService != null)
            {
                equipmentData = saveService.Load("Equipment", new EquipmentData());
            }
        }

        private void SaveEquipment()
        {
            if (saveService != null)
            {
                saveService.Save("Equipment", equipmentData);
            }
        }
    }
}
