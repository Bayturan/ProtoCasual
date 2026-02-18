using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    public enum ItemType { Consumable, Skin, PowerUp, Equipment }

    [CreateAssetMenu(menuName = "ProtoCasual/Economy/Item Config")]
    public class ItemConfig : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemId;
        public string itemName;
        public string description;
        public Sprite icon;
        public ItemType itemType;

        [Header("Economy")]
        public int price;
        public string currencyType = "Coins";
        public bool isPremium;

        [Header("Gameplay")]
        public GameObject prefab;
        public float effectValue;
        public float effectDuration;
    }
}
