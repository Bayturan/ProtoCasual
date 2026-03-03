namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Contract for UI screens managed by UIToolkitManager.
    /// Implemented by ScreenController base class.
    /// </summary>
    public interface IUIScreen
    {
        string ScreenName { get; }
        void OnShow();
        void OnHide();
    }
}
