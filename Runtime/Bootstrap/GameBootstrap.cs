using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Bootstrap
{
    /// <summary>
    /// Entry point for the game. Initializes ServiceLocator, registers services, and bootstraps managers.
    /// Place this on the first GameObject in your scene hierarchy.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;

        private void Awake()
        {
            ServiceLocator.Initialize();
            Application.targetFrameRate = gameConfig != null ? gameConfig.targetFPS : 60;
        }

        private void Start()
        {
            RegisterServices();
            InitializeManagers();
        }

        /// <summary>
        /// Override or extend this to register your own services with the ServiceLocator.
        /// </summary>
        protected virtual void RegisterServices()
        {
            var inputService = FindAnyObjectByType<Systems.InputManager>();
            if (inputService != null)
                ServiceLocator.Register<IInputService>(inputService);

            var saveService = FindAnyObjectByType<Managers.SaveService>();
            if (saveService != null)
                ServiceLocator.Register<ISaveService>(saveService);
        }

        protected virtual void InitializeManagers()
        {
            if (UI.UIManager.Instance != null)
                UI.UIManager.Instance.Initialize();
        }
    }
}
