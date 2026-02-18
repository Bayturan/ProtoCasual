namespace ProtoCasual.Core.Interfaces
{
    public interface IUIScreen
    {
        string ScreenName { get; }
        void Initialize();
        void Show();
        void Hide();
    }
}
