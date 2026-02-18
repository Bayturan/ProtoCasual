using UnityEngine;

namespace ProtoCasual.Core.Managers
{
    /// <summary>
    /// Simple level progress tracker via PlayerPrefs.
    /// For full save/load, use SaveService which implements ISaveService.
    /// </summary>
    public static class SaveManager
    {
        private const string LEVEL_KEY = "level";

        public static int CurrentLevel
        {
            get => PlayerPrefs.GetInt(LEVEL_KEY, 0);
            set
            {
                PlayerPrefs.SetInt(LEVEL_KEY, value);
                PlayerPrefs.Save();
            }
        }

        public static void NextLevel() => CurrentLevel++;

        public static void ResetProgress()
        {
            CurrentLevel = 0;
            PlayerPrefs.Save();
        }
    }
}
