using System.Collections.Generic;
using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Utilities;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Manages overlay popups (confirmation dialogs, reward popups, etc.).
    /// Supports stacking — multiple popups can be open at once.
    /// Place on the same Canvas as UIManager or on a dedicated higher-sort-order Canvas.
    /// </summary>
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField] private PopupBase[] popups;

        private readonly Dictionary<string, PopupBase> popupDict = new();
        private readonly Stack<PopupBase> activeStack = new();

        protected override void Awake()
        {
            base.Awake();
            RegisterPopups();
        }

        private void RegisterPopups()
        {
            if (popups == null || popups.Length == 0)
                popups = GetComponentsInChildren<PopupBase>(true);

            popupDict.Clear();
            foreach (var popup in popups)
            {
                if (popup != null && !popupDict.ContainsKey(popup.PopupName))
                {
                    popupDict.Add(popup.PopupName, popup);
                    popup.Hide();
                }
            }
        }

        /// <summary>
        /// Show a popup by name. Pushes onto the stack.
        /// </summary>
        public void ShowPopup(string popupName, object data = null)
        {
            if (string.IsNullOrEmpty(popupName)) return;
            if (!popupDict.TryGetValue(popupName, out var popup))
            {
                Debug.LogWarning($"[PopupManager] Popup '{popupName}' not found!");
                return;
            }

            popup.Show(data);
            activeStack.Push(popup);
        }

        /// <summary>
        /// Show a popup by type.
        /// </summary>
        public void ShowPopup<T>(object data = null) where T : PopupBase
        {
            foreach (var kvp in popupDict)
            {
                if (kvp.Value is T)
                {
                    ShowPopup(kvp.Key, data);
                    return;
                }
            }
            Debug.LogWarning($"[PopupManager] Popup of type '{typeof(T).Name}' not found!");
        }

        /// <summary>
        /// Hide a specific popup by name.
        /// </summary>
        public void HidePopup(string popupName)
        {
            if (popupDict.TryGetValue(popupName, out var popup))
                popup.Hide();
        }

        /// <summary>
        /// Hide the topmost popup on the stack.
        /// </summary>
        public void HideTopPopup()
        {
            while (activeStack.Count > 0)
            {
                var top = activeStack.Pop();
                if (top != null && top.IsVisible)
                {
                    top.Hide();
                    return;
                }
            }
        }

        /// <summary>
        /// Hide all open popups.
        /// </summary>
        public void HideAll()
        {
            foreach (var popup in popupDict.Values)
            {
                if (popup.IsVisible) popup.Hide();
            }
            activeStack.Clear();
        }

        /// <summary>
        /// True if any popup is currently visible.
        /// </summary>
        public bool HasActivePopup
        {
            get
            {
                foreach (var popup in popupDict.Values)
                    if (popup.IsVisible) return true;
                return false;
            }
        }

        /// <summary>
        /// Get a typed popup reference.
        /// </summary>
        public T GetPopup<T>() where T : PopupBase
        {
            foreach (var popup in popupDict.Values)
                if (popup is T typed) return typed;
            return null;
        }
    }
}
