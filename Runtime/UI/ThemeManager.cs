using UnityEngine;
using UnityEngine.UIElements;

namespace ProtoCasual.Core.UI
{
    /// <summary>
    /// Manages UI Toolkit theme switching.
    /// Themes are USS files that override CSS custom properties.
    /// </summary>
    public class ThemeManager
    {
        private readonly VisualElement root;
        private StyleSheet currentTheme;

        public ThemeManager(VisualElement root)
        {
            this.root = root;
        }

        /// <summary>Add base and component stylesheets (called once at init).</summary>
        public void ApplyBaseStyles(StyleSheet baseStyles, StyleSheet componentStyles)
        {
            if (baseStyles != null) root.styleSheets.Add(baseStyles);
            if (componentStyles != null) root.styleSheets.Add(componentStyles);
        }

        /// <summary>Swap the active theme stylesheet at runtime.</summary>
        public void ApplyTheme(StyleSheet theme)
        {
            if (currentTheme != null)
                root.styleSheets.Remove(currentTheme);

            if (theme != null)
            {
                root.styleSheets.Add(theme);
                currentTheme = theme;
            }
        }

        /// <summary>The currently active theme stylesheet.</summary>
        public StyleSheet CurrentTheme => currentTheme;
    }
}
