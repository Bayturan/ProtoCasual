using System;
using System.Collections.Generic;

namespace ProtoCasual.Core.Interfaces
{
    public interface IEquipmentService
    {
        event Action OnEquipmentChanged;

        bool Equip(string slotName, string itemId);
        bool Unequip(string slotName);
        string GetEquipped(string slotName);
        bool IsSlotEmpty(string slotName);
        IReadOnlyList<Data.EquipmentSlot> GetAll();
        void Clear();
    }
}
