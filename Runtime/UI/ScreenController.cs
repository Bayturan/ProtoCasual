using UnityEngine.UIElements;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Base class for all UI Toolkit screen controllers.
    /// Each screen has a UXML template and a C# controller that queries
    /// elements by name and wires callbacks.
    /// </summary>
    public abstract class ScreenController
    {
        protected VisualElement Root { get; private set; }
        public abstract string ScreenName { get; }
        public bool IsBound { get; private set; }

        public void Bind(VisualElement root)
        {
            Root = root;
            OnBind();
            IsBound = true;
        }

        /// <summary>Called once when the screen is first bound to its visual tree.</summary>
        protected abstract void OnBind();

        /// <summary>Called every time the screen becomes visible.</summary>
        public virtual void OnShow() { }

        /// <summary>Called every time the screen is hidden.</summary>
        public virtual void OnHide() { }

        /// <summary>Called every frame while the screen is visible.</summary>
        public virtual void OnUpdate(float deltaTime) { }

        // ─── Query Helpers ──────────────────────────────────────────

        protected T Q<T>(string name = null, string className = null) where T : VisualElement
            => Root?.Q<T>(name, className);

        protected VisualElement Q(string name = null, string className = null)
            => Root?.Q(name, className);

        protected Button Btn(string name) => Q<Button>(name);
        protected Label Lbl(string name) => Q<Label>(name);
        protected Slider Sld(string name) => Q<Slider>(name);
        protected Toggle Tgl(string name) => Q<Toggle>(name);
        protected ProgressBar PBar(string name) => Q<ProgressBar>(name);
        protected ScrollView SView(string name) => Q<ScrollView>(name);
    }
}
