using UnityEngine.UIElements;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Base class for all UI Toolkit popup controllers.
    /// Popups are modal overlays shown on top of screens.
    /// </summary>
    public abstract class PopupController
    {
        protected VisualElement Root { get; private set; }
        public abstract string PopupName { get; }
        public bool IsVisible { get; private set; }

        public void Bind(VisualElement root)
        {
            Root = root;
            Root.style.display = DisplayStyle.None;
            OnBind();
        }

        /// <summary>Called once when the popup is first bound to its visual tree.</summary>
        protected abstract void OnBind();

        public void Show(object data = null)
        {
            Root.style.display = DisplayStyle.Flex;
            Root.pickingMode = PickingMode.Position;
            IsVisible = true;
            OnShow(data);
        }

        public void Hide()
        {
            Root.style.display = DisplayStyle.None;
            IsVisible = false;
            OnHide();
        }

        protected virtual void OnShow(object data) { }
        protected virtual void OnHide() { }

        // ─── Query Helpers ──────────────────────────────────────────

        protected T Q<T>(string name = null, string className = null) where T : VisualElement
            => Root?.Q<T>(name, className);

        protected VisualElement Q(string name = null, string className = null)
            => Root?.Q(name, className);

        protected Button Btn(string name) => Q<Button>(name);
        protected Label Lbl(string name) => Q<Label>(name);
    }
}
