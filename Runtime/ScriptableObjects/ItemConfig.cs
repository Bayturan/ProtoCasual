using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    public enum ItemType { Consumable, Cosmetic, Equipment, Currency }

    [CreateAssetMenu(menuName = "ProtoCasual/Economy/Item Config")]
    public class ItemConfig : ScriptableObject
    {
        [Header("Identity")]
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;
        public ItemType Type;
        public string Category;

        [Header("Economy")]
        public int SoftCurrencyPrice;
        public int HardCurrencyPrice;

        [Header("Behaviour")]
        public bool IsStackable = true;
        public bool IsEquippable;

        [Header("Gameplay (optional)")]
        public GameObject Prefab;
        public float EffectValue;
        public float EffectDuration;
    }
}

