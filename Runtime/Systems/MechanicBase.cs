using UnityEngine;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Systems
{
    /// <summary>
    /// Base class for plug-in mechanics. Subclass and override the
    /// On* callbacks. Use Enable()/Disable() instead of
    /// gameObject.SetActive — the methods deliberately avoid shadowing
    /// MonoBehaviour.OnEnable/OnDisable.
    /// </summary>
    public abstract class MechanicBase : MonoBehaviour, IMechanic
    {
        public abstract string MechanicName { get; }
        public bool IsEnabled { get; set; }

        protected bool isInitialized;

        public virtual void Initialize()
        {
            if (isInitialized) return;
            OnMechanicInitialize();
            isInitialized = true;
        }

        public virtual void Enable()
        {
            if (!isInitialized)
            {
                Initialize();
            }
            IsEnabled = true;
            OnMechanicEnable();
        }

        public virtual void Disable()
        {
            IsEnabled = false;
            OnMechanicDisable();
        }

        public virtual void UpdateMechanic(float deltaTime)
        {
            if (!IsEnabled) return;
            OnMechanicUpdate(deltaTime);
        }

        public virtual void Cleanup()
        {
            OnMechanicCleanup();
            isInitialized = false;
            IsEnabled = false;
        }

        protected abstract void OnMechanicInitialize();
        protected abstract void OnMechanicEnable();
        protected abstract void OnMechanicDisable();
        protected abstract void OnMechanicUpdate(float deltaTime);
        protected abstract void OnMechanicCleanup();
    }
}
