using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProtoCasual.Core.Bootstrap
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Initialize()
        {
            services.Clear();
        }

        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"Service {type.Name} already registered. Overwriting.");
                services[type] = service;
            }
            else
            {
                services.Add(type, service);
            }
        }

        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return service as T;
            }

            Debug.LogError($"Service {type.Name} not found!");
            return null;
        }

        public static bool IsRegistered<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
            }
        }
    }
}
