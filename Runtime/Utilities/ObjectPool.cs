using UnityEngine;
using System.Collections.Generic;

namespace ProtoCasual.Core.Utilities
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly Queue<T> availableObjects = new Queue<T>();
        private readonly HashSet<T> activeObjects = new HashSet<T>();

        public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private T CreateNewObject()
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            availableObjects.Enqueue(obj);
            return obj;
        }

        public T Get()
        {
            T obj;
            
            if (availableObjects.Count > 0)
            {
                obj = availableObjects.Dequeue();
            }
            else
            {
                obj = CreateNewObject();
            }

            obj.gameObject.SetActive(true);
            activeObjects.Add(obj);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null) return;

            obj.gameObject.SetActive(false);
            
            if (activeObjects.Remove(obj))
            {
                availableObjects.Enqueue(obj);
            }
        }

        public void ReturnAll()
        {
            foreach (var obj in activeObjects)
            {
                if (obj != null)
                {
                    obj.gameObject.SetActive(false);
                    availableObjects.Enqueue(obj);
                }
            }
            activeObjects.Clear();
        }

        public void Clear()
        {
            ReturnAll();
            
            while (availableObjects.Count > 0)
            {
                T obj = availableObjects.Dequeue();
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }
        }

        public int ActiveCount => activeObjects.Count;
        public int AvailableCount => availableObjects.Count;
    }
}
