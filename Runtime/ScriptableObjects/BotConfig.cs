using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Bot Config")]
    public class BotConfig : ScriptableObject
    {
        [Header("Bot Settings")]
        public string botName;
        public GameObject botPrefab;

        [Header("Movement")]
        public float moveSpeed = 5f;
        public float acceleration = 2f;
        public float turnSpeed = 180f;

        [Header("AI Behavior")]
        public float reactionTime = 0.2f;
        public float errorMargin = 0.5f;
        public bool useRubberBanding = true;
        public float rubberBandStrength = 1.2f;
    }
}
