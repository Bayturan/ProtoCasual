using UnityEngine;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Interfaces
{
    public interface IBot
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        bool IsActive { get; set; }
        void Initialize(BotConfig config);
        void UpdateBot(float deltaTime);
        void Reset();
        void Destroy();
    }
}