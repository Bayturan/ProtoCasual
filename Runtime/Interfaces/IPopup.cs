namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Contract for popup overlays managed by PopupManager.
    /// </summary>
    public interface IPopup
    {
        string PopupName { get; }
        bool IsVisible { get; }
        void Show(object data = null);
        void Hide();
    }
}
