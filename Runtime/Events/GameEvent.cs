using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProtoCasual.Core.Events
{
    [CreateAssetMenu(menuName = "ProtoCasual/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> listeners = new();
        private event Action OnEvent;

        public void Raise()
        {
            OnEvent?.Invoke();
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised();
        }

        public void Register(GameEventListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void Unregister(GameEventListener listener)
        {
            listeners.Remove(listener);
        }

        public void RegisterAction(Action action) => OnEvent += action;
        public void UnregisterAction(Action action) => OnEvent -= action;
    }
}