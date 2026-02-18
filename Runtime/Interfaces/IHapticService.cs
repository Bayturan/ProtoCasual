namespace ProtoCasual.Core.Interfaces
{
    /// <summary>
    /// Haptic feedback service. Wraps platform vibration APIs.
    /// </summary>
    public interface IHapticService
    {
        bool IsEnabled { get; set; }

        void LightImpact();
        void MediumImpact();
        void HeavyImpact();
        void Selection();
        void Success();
        void Warning();
        void Error();
    }
}
