using UnityEngine;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Systems
{
    public abstract class MechanicBase : MonoBehaviour, IMechanic
    {
        public abstract string MechanicName { get; }
        public bool IsEnabled { get; set; }

        protected bool isInitialized;

        public virtual void Initialize()
        {
            if (isInitialized) return;
            OnInitialize();
            isInitialized = true;
        }

        public virtual void Enable()
        {
            if (!isInitialized)
            {
                Initialize();
            }
            IsEnabled = true;
            OnEnable();
        }

        public virtual void Disable()
        {
            IsEnabled = false;
            OnDisable();
        }

        public virtual void UpdateMechanic(float deltaTime)
        {
            if (!IsEnabled) return;
            OnUpdate(deltaTime);
        }

        public virtual void Cleanup()
        {
            OnCleanup();
            isInitialized = false;
            IsEnabled = false;
        }

        protected abstract void OnInitialize();
        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected abstract void OnUpdate(float deltaTime);
        protected abstract void OnCleanup();
    }
}
