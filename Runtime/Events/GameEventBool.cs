using UnityEngine;
using System;

namespace ProtoCasual.Core.Events
{
    [CreateAssetMenu(fileName = "New Bool Event", menuName = "Game/Events/Bool Event")]
    public class GameEventBool : ScriptableObject
    {
        private event Action<bool> OnEventRaised;

        public void Raise(bool value)
        {
            OnEventRaised?.Invoke(value);
        }

        public void RegisterListener(Action<bool> listener)
        {
            OnEventRaised += listener;
        }

        public void UnregisterListener(Action<bool> listener)
        {
            OnEventRaised -= listener;
        }
    }
}
