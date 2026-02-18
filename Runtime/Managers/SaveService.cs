using UnityEngine;
using System;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Managers
{
    /// <summary>
    /// JSON-based save service with PlayerPrefs fallback.
    /// Implements ISaveService for use with ServiceLocator.
    /// Supports save versioning for future data migration.
    /// </summary>
    public class SaveService : MonoBehaviour, ISaveService
    {
        private const string VERSION_KEY = "SaveVersion";
        private const int CURRENT_VERSION = 1;

        private void Awake()
        {
            MigrateIfNeeded();
        }

        public void Save<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("[SaveService] Cannot save with null/empty key.");
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                // Fallback for primitive types
                if (data is int intVal)
                    PlayerPrefs.SetInt(key, intVal);
                else if (data is float floatVal)
                    PlayerPrefs.SetFloat(key, floatVal);
                else if (data is string strVal)
                    PlayerPrefs.SetString(key, strVal);
                else
                    Debug.LogError($"[SaveService] Failed to save '{key}': {e.Message}");

                PlayerPrefs.Save();
            }
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            if (!HasKey(key)) return defaultValue;

            try
            {
                string json = PlayerPrefs.GetString(key, null);
                if (string.IsNullOrEmpty(json)) return defaultValue;
                return JsonUtility.FromJson<T>(json);
            }
            catch
            {
                // Fallback for primitive types
                try
                {
                    if (typeof(T) == typeof(int))
                        return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
                    if (typeof(T) == typeof(float))
                        return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
                    if (typeof(T) == typeof(string))
                        return (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
                }
                catch { }

                return defaultValue;
            }
        }

        public bool HasKey(string key) => PlayerPrefs.HasKey(key);

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        private void MigrateIfNeeded()
        {
            int savedVersion = PlayerPrefs.GetInt(VERSION_KEY, 0);
            if (savedVersion < CURRENT_VERSION)
            {
                PerformMigration(savedVersion, CURRENT_VERSION);
                PlayerPrefs.SetInt(VERSION_KEY, CURRENT_VERSION);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Override this method to handle data migration between versions.
        /// </summary>
        protected virtual void PerformMigration(int fromVersion, int toVersion)
        {
            // Version migration logic goes here
            // Example: if (fromVersion < 1) { migrate v0 -> v1 }
        }
    }
}
