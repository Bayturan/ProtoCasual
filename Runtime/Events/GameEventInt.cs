using UnityEngine;
using System.Collections.Generic;
using System;

namespace ProtoCasual.Core.Events
{
    [CreateAssetMenu(fileName = "New Int Event", menuName = "Game/Events/Int Event")]
    public class GameEventInt : ScriptableObject
    {
        private event Action<int> OnEventRaised;

        public void Raise(int value)
        {
            OnEventRaised?.Invoke(value);
        }

        public void RegisterListener(Action<int> listener)
        {
            OnEventRaised += listener;
        }

        public void UnregisterListener(Action<int> listener)
        {
            OnEventRaised -= listener;
        }
    }
}
