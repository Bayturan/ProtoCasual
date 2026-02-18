using UnityEngine;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.ScriptableObjects;
using ProtoCasual.Core.Data;
using ProtoCasual.Core.Currency;
using ProtoCasual.Core.Inventory;
using ProtoCasual.Core.Store;

namespace ProtoCasual.Core.Bootstrap
{
    /// <summary>
    /// Entry point for the game. Initializes ServiceLocator, registers services, and bootstraps managers.
    /// Place this on the first GameObject in your scene hierarchy.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private ItemDatabase itemDatabase;

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
            // InputService
            var inputService = FindAnyObjectByType<Systems.InputManager>();
            if (inputService != null)
                ServiceLocator.Register<IInputService>(inputService);

            // SaveService
            var saveService = FindAnyObjectByType<Managers.SaveService>();
            if (saveService != null)
                ServiceLocator.Register<ISaveService>(saveService);

            // PlayerDataProvider  (loads PlayerData from save)
            var save = ServiceLocator.Get<ISaveService>();
            var dataProvider = new PlayerDataProvider(save);
            ServiceLocator.Register(dataProvider);

            // ItemDatabase
            if (itemDatabase != null)
            {
                itemDatabase.Initialise();
                ServiceLocator.Register(itemDatabase);
            }

            // CurrencyService
            var currencyService = new CurrencyService(dataProvider, save);
            ServiceLocator.Register<ICurrencyService>(currencyService);

            // InventoryService
            var inventoryService = new InventoryService(dataProvider);
            ServiceLocator.Register<IInventoryService>(inventoryService);

            // StoreService
            if (itemDatabase != null)
            {
                var storeService = new StoreService(itemDatabase, currencyService, inventoryService);
                ServiceLocator.Register<IStoreService>(storeService);
            }
        }

        protected virtual void InitializeManagers()
        {
            if (UI.UIManager.Instance != null)
                UI.UIManager.Instance.Initialize();
        }
    }
}
