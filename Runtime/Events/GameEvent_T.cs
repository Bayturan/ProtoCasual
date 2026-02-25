using System;
using UnityEngine;

namespace ProtoCasual.Core.Events
{
    /// <summary>
    /// Generic typed ScriptableObject event channel.
    /// Create concrete subclasses to make them visible in the CreateAssetMenu:
    /// <code>
    /// [CreateAssetMenu(menuName = "ProtoCasual/Events/Float Event")]
    /// public class GameEventFloat : GameEvent&lt;float&gt; { }
    /// </code>
    /// </summary>
    public abstract class GameEvent<T> : ScriptableObject
    {
        private event Action<T> OnEventRaised;

        public void Raise(T value)
        {
            OnEventRaised?.Invoke(value);
        }

        public void RegisterListener(Action<T> listener)
        {
            OnEventRaised += listener;
        }

        public void UnregisterListener(Action<T> listener)
        {
            OnEventRaised -= listener;
        }
    }
}
