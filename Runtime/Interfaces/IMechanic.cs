namespace ProtoCasual.Core.Interfaces
{
    public interface IMechanic
    {
        string MechanicName { get; }
        bool IsEnabled { get; set; }
        void Initialize();
        void Enable();
        void Disable();
        void UpdateMechanic(float deltaTime);
        void Cleanup();
    }
}