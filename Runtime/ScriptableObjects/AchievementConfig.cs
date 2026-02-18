using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    /// <summary>
    /// Achievement definitions.
    /// Create via: Create → ProtoCasual → Config → Achievement Config
    /// </summary>
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Achievement Config")]
    public class AchievementConfig : ScriptableObject
    {
        [Header("Achievements")]
        public AchievementDefinition[] achievements;
    }

    [System.Serializable]
    public class AchievementDefinition
    {
        public string achievementId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public Sprite lockedIcon;

        [Header("Completion")]
        [Tooltip("Required progress to unlock (e.g., 10 = collect 10 items).")]
        public int requiredProgress = 1;

        [Header("Reward")]
        public Interfaces.RewardEntry[] rewards;

        [Header("Display")]
        public bool isHidden;
    }
}
