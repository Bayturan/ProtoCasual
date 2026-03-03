using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Manages popup overlays within the UI Toolkit tree.
    /// Popups are stacked; HideTop() pops the most recent.
    /// </summary>
    public class PopupManager
    {
        private readonly VisualElement container;
        private readonly Dictionary<string, PopupController> popups = new();
        private readonly Stack<PopupController> activeStack = new();

        public PopupManager(VisualElement container)
        {
            this.container = container;
        }

        public void Register(string name, VisualTreeAsset layout, PopupController controller)
        {
            if (layout == null || controller == null) return;

            var instance = layout.Instantiate();
            instance.style.position = Position.Absolute;
            instance.style.left = 0;
            instance.style.right = 0;
            instance.style.top = 0;
            instance.style.bottom = 0;
            container.Add(instance);
            controller.Bind(instance);
            popups[name] = controller;
        }

        public void Show(string name, object data = null)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (!popups.TryGetValue(name, out var popup))
            {
                Debug.LogWarning($"[PopupManager] Popup '{name}' not found!");
                return;
            }
            popup.Show(data);
            activeStack.Push(popup);
        }

        public void Show<T>(object data = null) where T : PopupController
        {
            foreach (var kvp in popups)
            {
                if (kvp.Value is T)
                {
                    Show(kvp.Key, data);
                    return;
                }
            }
            Debug.LogWarning($"[PopupManager] Popup of type '{typeof(T).Name}' not found!");
        }

        public void Hide(string name)
        {
            if (popups.TryGetValue(name, out var popup))
                popup.Hide();
        }

        public void HideTop()
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

        public void HideAll()
        {
            foreach (var popup in popups.Values)
                if (popup.IsVisible) popup.Hide();
            activeStack.Clear();
        }

        public bool HasActivePopup
        {
            get
            {
                foreach (var popup in popups.Values)
                    if (popup.IsVisible) return true;
                return false;
            }
        }

        public T Get<T>() where T : PopupController
        {
            foreach (var popup in popups.Values)
                if (popup is T typed) return typed;
            return null;
        }
    }
}
