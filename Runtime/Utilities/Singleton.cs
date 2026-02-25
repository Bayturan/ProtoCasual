using UnityEngine;

namespace ProtoCasual.Core.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        private static readonly object lockObject = new object();

        public static T Instance
        {
            get
            {
                if (!Application.isPlaying) return null;

                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindAnyObjectByType<T>();

                        if (instance == null)
                        {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<T>();
                            singleton.name = typeof(T).ToString() + " (Singleton)";
                            DontDestroyOnLoad(singleton);
                        }
                    }

                    return instance;
                }
            }
        }

        /// <summary>Returns true if an instance exists without triggering lazy creation.</summary>
        public static bool HasInstance => instance != null;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
