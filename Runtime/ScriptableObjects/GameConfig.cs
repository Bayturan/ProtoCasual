using UnityEngine;

namespace ProtoCasual.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ProtoCasual/Config/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Performance")]
        public int targetFPS = 60;

        [Header("Settings")]
        public bool vibrationEnabled;
        public bool soundEnabled = true;
    }
}
