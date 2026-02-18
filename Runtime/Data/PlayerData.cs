using System;
using System.Collections.Generic;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Data
{
    // ─── Inventory ──────────────────────────────────────────────────

    /// <summary>
    /// Represents a single inventory entry — ID + quantity.
    /// No ScriptableObject references — purely serializable.
    /// </summary>
    [Serializable]
    public class InventoryItemData
    {
        public string ItemId;
        public int Quantity;

        public InventoryItemData() { }

        public InventoryItemData(string itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }

    /// <summary>
    /// Flat list of owned items with quantities.
    /// Serialized to JSON by SaveService.
    /// </summary>
    [Serializable]
    public class InventoryData
    {
        public List<InventoryItemData> Items = new List<InventoryItemData>();
    }

    // ─── Currency ───────────────────────────────────────────────────

    /// <summary>
    /// Dual-currency wallet: SoftCurrency (coins) and HardCurrency (gems/diamonds).
    /// </summary>
    [Serializable]
    public class CurrencyData
    {
        public int SoftCurrency;
        public int HardCurrency;
    }

    // ─── Equipment ──────────────────────────────────────────────────

    /// <summary>
    /// Equipment slots — keyed by slot name → item ID.
    /// </summary>
    [Serializable]
    public class EquipmentData
    {
        public List<EquipmentSlot> Slots = new List<EquipmentSlot>();
    }

    [Serializable]
    public class EquipmentSlot
    {
        public string SlotName;
        public string ItemId;

        public EquipmentSlot() { }

        public EquipmentSlot(string slotName, string itemId)
        {
            SlotName = slotName;
            ItemId = itemId;
        }
    }

    // ─── Daily Reward ───────────────────────────────────────────────

    [Serializable]
    public class DailyRewardData
    {
        public int Streak;
        public long LastClaimTimeTicks;
    }

    // ─── Tutorial ───────────────────────────────────────────────────

    [Serializable]
    public class TutorialSaveData
    {
        public bool IsCompleted;
        public int CurrentStep;
    }

    // ─── Leaderboard ────────────────────────────────────────────────

    [Serializable]
    public class LeaderboardSaveData
    {
        public List<LeaderboardBoardData> Boards = new List<LeaderboardBoardData>();
    }

    [Serializable]
    public class LeaderboardBoardData
    {
        public string LeaderboardId;
        public List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();
    }

    // ─── Achievements ───────────────────────────────────────────────

    [Serializable]
    public class AchievementSaveData
    {
        public List<AchievementProgressData> Entries = new List<AchievementProgressData>();
    }

    [Serializable]
    public class AchievementProgressData
    {
        public string AchievementId;
        public int CurrentProgress;
        public bool IsUnlocked;
    }

    // ─── Root ───────────────────────────────────────────────────────

    /// <summary>
    /// Root save container. Everything the player owns lives here.
    /// Saved/loaded as a single JSON blob by SaveService.
    /// Bump <see cref="Version"/> when adding fields for migration support.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public int Version = 2;
        public CurrencyData Currency = new CurrencyData();
        public InventoryData Inventory = new InventoryData();
        public EquipmentData Equipment = new EquipmentData();
        public DailyRewardData DailyReward = new DailyRewardData();
        public TutorialSaveData Tutorial = new TutorialSaveData();
        public LeaderboardSaveData Leaderboards = new LeaderboardSaveData();
        public AchievementSaveData Achievements = new AchievementSaveData();
    }
}
