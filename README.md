# Proto Casual — Game Factory Framework

A production-ready Unity UPM package for building casual mobile games. Genre-agnostic, modular, monetization-ready, and clone-and-ship ready.

**Package:** `com.bayturan.protocasual`  
**Version:** 0.2.0  
**Unity:** 6000.3+  
**Namespace:** `ProtoCasual.Core`

---

## Table of Contents

1. [Installation](#installation)
2. [Setup Wizard](#setup-wizard)
3. [Architecture Overview](#architecture-overview)
4. [Bootstrap & ServiceLocator](#bootstrap--servicelocator)
5. [Game State Machine](#game-state-machine)
6. [Game Modes](#game-modes)
7. [UI System](#ui-system)
8. [Input System](#input-system)
9. [Currency System](#currency-system)
10. [Inventory System](#inventory-system)
11. [Store System](#store-system)
12. [Save System](#save-system)
13. [Audio Manager](#audio-manager)
14. [Level Manager](#level-manager)
15. [Event System](#event-system)
16. [Mechanics System](#mechanics-system)
17. [Map & Endless Generation](#map--endless-generation)
18. [Object Pool](#object-pool)
19. [Timer](#timer)
20. [Extension Methods](#extension-methods)
21. [Ads & IAP Interfaces](#ads--iap-interfaces)
22. [Bot System](#bot-system)
23. [ScriptableObject Configs](#scriptableobject-configs)
24. [Folder Structure](#folder-structure)
25. [Samples](#samples)

---

## Installation

**Option A — From disk:**
1. Copy the package folder into your project's `Packages/` directory.
2. Unity will auto-detect it.

**Option B — Package Manager UI:**
1. **Window → Package Manager → + → Add package from disk…**
2. Select `Packages/com.bayturan.protocasual/package.json`.

**Dependencies** (auto-resolved):
- `com.unity.textmeshpro` 3.0.6
- `com.unity.test-framework` 1.6.0

---

## Setup Wizard

### Full Setup (Recommended)

**Menu → ProtoCasual → Create New Game**

The wizard asks 7 questions and generates everything:

| Question | Options |
|---|---|
| Game Type | HyperCasual, Hybrid, Puzzle, Racing, Endless |
| Map Type | Fixed Levels, Procedural, Endless Generation |
| Bots | None, Simple AI, Advanced AI |
| Monetization | None, Ads Only, IAP Only, Ads + IAP |
| Store | Enabled, Disabled |
| Input Type | Tap, Swipe, Drag, Steering, Mixed |
| Platform | Android, iOS, Both |

**What gets generated:**
- `Assets/_Game/` folder structure (Content, Prefabs, ScriptableObjects, UI, Audio, etc.)
- Default ScriptableObject configs (GameConfig, GameModeConfig, LevelConfig, MapConfig, BotConfig)
- UI screen prefabs (Menu, Gameplay, Win, Lose, Pause, Settings, Store)
- 2 scenes: `Main.unity` (menu) + `InGame.unity` (gameplay)
- Full scene hierarchy with all managers and components
- Build Settings configured

### Quick Setup

**Menu → ProtoCasual → Setup Scene (Quick)**

Uses default settings and generates immediately.

---

## Architecture Overview

```
Runtime/
├── Bootstrap/          GameBootstrap, ServiceLocator
├── Currency/           CurrencyService
├── Data/               PlayerData, PlayerDataProvider
├── Events/             GameEvent, GameEventListener, GameEventBool, GameEventInt
├── GameLoop/           GameModeBase, GameState
├── Interfaces/         All contracts (16 interfaces)
├── Inventory/          InventoryService
├── Managers/           GameManager, AudioManager, GameModeManager, LevelManager, SaveManager, SaveService
├── ScriptableObjects/  GameConfig, LevelConfig, MapConfig, ItemConfig, BotConfig, GameModeConfig
├── Store/              StoreService, ItemDatabase
├── Systems/            InputManager, MapGenerator, EndlessGenerator, MechanicBase, Mechanics/
├── UI/                 UIManager, UIScreen, Screens/
└── Utilities/          Singleton<T>, ObjectPool<T>, Timer, Extensions
```

**Key patterns:**
- **ServiceLocator** — All services registered and resolved through a central registry
- **Singleton\<T\>** — Thread-safe generic MonoBehaviour singleton with DontDestroyOnLoad
- **State Machine** — `GameManager` drives game flow through `GameState` enum
- **ScriptableObject Events** — Decoupled event channels
- **Interface-driven** — Every system has a contract; implementations are swappable

---

## Bootstrap & ServiceLocator

`GameBootstrap` is the entry point. Place it on the first GameObject in your scene.

```csharp
using ProtoCasual.Core.Bootstrap;

// The bootstrap runs automatically on Awake/Start.
// It initializes ServiceLocator and registers all core services.
```

### ServiceLocator API

```csharp
using ProtoCasual.Core.Bootstrap;

// Register a service
ServiceLocator.Register<IMyService>(myServiceInstance);

// Resolve a service
var service = ServiceLocator.Get<IMyService>();

// Check if registered
if (ServiceLocator.IsRegistered<IMyService>()) { }

// Unregister
ServiceLocator.Unregister<IMyService>();
```

### Services auto-registered by GameBootstrap

| Service | Interface | Type |
|---|---|---|
| InputManager | `IInputService` | MonoBehaviour |
| SaveService | `ISaveService` | MonoBehaviour |
| PlayerDataProvider | `PlayerDataProvider` | Plain C# |
| CurrencyService | `ICurrencyService` | Plain C# |
| InventoryService | `IInventoryService` | Plain C# |
| StoreService | `IStoreService` | Plain C# (requires ItemDatabase) |

### Custom Bootstrap

Override `GameBootstrap` to register your own services:

```csharp
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

public class MyBootstrap : GameBootstrap
{
    protected override void RegisterServices()
    {
        base.RegisterServices(); // registers all core services
        
        var myAds = FindAnyObjectByType<MyAdsService>();
        if (myAds != null)
            ServiceLocator.Register<IAdsService>(myAds);
    }
}
```

---

## Game State Machine

`GameManager` is the central state machine. All game flow goes through it.

### States

```csharp
public enum GameState
{
    Boot,       // App starting
    Menu,       // Main menu
    Prepare,    // Loading / countdown
    Playing,    // Gameplay active
    Paused,     // Paused (timeScale = 0)
    Completed,  // Player won
    Failed      // Player lost
}
```

### GameManager API

```csharp
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.GameLoop;

// Access singleton
GameManager.Instance

// State transitions
GameManager.Instance.Play();        // → Playing
GameManager.Instance.Pause();       // → Paused
GameManager.Instance.Resume();      // → Playing
GameManager.Instance.Complete();    // → Completed
GameManager.Instance.Fail();        // → Failed
GameManager.Instance.Restart();     // cleanup → reinitialize → Playing
GameManager.Instance.ReturnToMenu();// cleanup → Menu

// Direct state change
GameManager.Instance.ChangeState(GameState.Prepare);

// Read state
GameState current = GameManager.Instance.CurrentState;
float elapsed = GameManager.Instance.GetGameTime();

// Listen to state changes
GameManager.Instance.OnStateChanged += (previous, current) =>
{
    Debug.Log($"State: {previous} → {current}");
};

// Set active game mode
GameManager.Instance.SetGameMode(myGameMode);
```

### ScriptableObject Events

Assign these in the Inspector to trigger UnityEvents on state changes:

| Field | Raised when |
|---|---|
| `onGameStart` | State enters Playing |
| `onGamePause` | State enters Paused |
| `onGameResume` | State returns to Playing from Paused |
| `onGameComplete` | State enters Completed |
| `onGameFail` | State enters Failed |

---

## Game Modes

Extend `GameModeBase` to create genre-specific gameplay:

```csharp
using UnityEngine;
using ProtoCasual.Core.GameLoop;

public class MyPuzzleMode : GameModeBase
{
    public override void Initialize()
    {
        // Setup board, load level data
    }

    public override void OnGameStart()
    {
        // Player tapped play — start spawning pieces
    }

    public override void OnGamePause()
    {
        // Freeze animations
    }

    public override void OnGameResume()
    {
        // Resume animations
    }

    public override void OnGameComplete()
    {
        // Show confetti, grant rewards
    }

    public override void OnGameFail()
    {
        // Shake camera, show fail effect
    }

    public override void UpdateMode(float deltaTime)
    {
        // Called every frame while game is running
        // Check win/lose conditions here
    }

    public override void Cleanup()
    {
        // Destroy spawned objects, reset state
    }
}
```

### GameModeManager

Manages multiple game modes. Assign modes in the Inspector:

```csharp
using ProtoCasual.Core.Managers;

// Switch by name
GameModeManager.Instance.SetGameMode("Puzzle");

// Switch by index
GameModeManager.Instance.SetGameModeByIndex(0);

// Get names of all available modes
string[] modes = GameModeManager.Instance.GetAvailableGameModeNames();
```

---

## UI System

### UIManager

State-driven screen management. Automatically shows/hides screens when `GameManager` state changes.

```csharp
using ProtoCasual.Core.UI;

// Show a screen by name
UIManager.Instance.ShowScreen("MenuScreen");

// Hide current screen
UIManager.Instance.HideCurrentScreen();

// Get a typed screen reference
var gameplay = UIManager.Instance.GetScreen<GameplayScreen>();
gameplay.SetScore(1500);
```

**State → Screen mapping** (configurable in Inspector):

| GameState | Screen |
|---|---|
| Menu | MenuScreen |
| Playing | GameplayScreen |
| Completed | WinScreen |
| Failed | LoseScreen |
| Paused | PauseScreen |

### Custom Screen

```csharp
using UnityEngine;
using UnityEngine.UI;
using ProtoCasual.Core.UI;

public class MyCustomScreen : UIScreen
{
    [SerializeField] private Button closeButton;

    protected override void OnInitialize()
    {
        closeButton.onClick.AddListener(() => Hide());
    }

    protected override void OnShow()
    {
        // Animate entrance, refresh data
    }

    protected override void OnHide()
    {
        // Cleanup
    }
}
```

### Built-in Screens

| Screen | Key Features |
|---|---|
| `MenuScreen` | Play, Store, Settings buttons |
| `GameplayScreen` | Score text, time text, pause button, progress slider |
| `WinScreen` | Score, time, Next Level / Restart / Menu buttons |
| `LoseScreen` | Retry, Menu, Watch Ad (rewarded) buttons |
| `PauseScreen` | Resume, Restart, Quit buttons |

---

## Input System

`InputManager` implements `IInputService`. Handles tap, swipe, drag, and hold detection.

```csharp
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

var input = ServiceLocator.Get<IInputService>();

// Enable / disable
input.IsInputEnabled = true;

// Subscribe to events
input.OnTap += (position) => Debug.Log($"Tap at {position}");
input.OnSwipe += (start, end) => Debug.Log($"Swipe {start} → {end}");
input.OnDragStart += (pos) => { };
input.OnDrag += (pos) => { };
input.OnDragEnd += () => { };
input.OnHoldStart += (pos) => { };
input.OnHold += (pos) => { };
input.OnHoldEnd += () => { };
```

Configure thresholds in the Inspector:
- `swipeThreshold` — minimum distance for swipe detection (default: 50)
- `holdThreshold` — minimum time for hold detection (default: 0.5s)

---

## Currency System

Dual-currency wallet accessed via `ICurrencyService`. Auto-saves after every mutation.

```csharp
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

var currency = ServiceLocator.Get<ICurrencyService>();

// Read
int coins = currency.SoftCurrency;
int gems  = currency.HardCurrency;

// Add
currency.AddSoft(500);
currency.AddHard(10);

// Spend (returns false if insufficient)
bool spent = currency.SpendSoft(200);

// Check
if (currency.HasSoft(100)) { /* can afford */ }
if (currency.HasHard(5))   { /* can afford */ }

// React to changes (for UI)
currency.OnCurrencyChanged += () =>
{
    coinText.text = $"{currency.SoftCurrency}";
    gemText.text  = $"{currency.HardCurrency}";
};
```

---

## Inventory System

ID-based inventory with O(1) lookups. No ScriptableObject references in saved data.

```csharp
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

var inventory = ServiceLocator.Get<IInventoryService>();

// Add items
inventory.AddItem("sword_01");
inventory.AddItem("potion_hp", 5);

// Remove
inventory.RemoveItem("potion_hp", 2);  // returns false if not enough

// Query
bool has = inventory.HasItem("sword_01");
int qty  = inventory.GetQuantity("potion_hp");

// Get all items
var all = inventory.GetAll();
foreach (var entry in all)
    Debug.Log($"{entry.ItemId} x{entry.Quantity}");

// Clear everything
inventory.Clear();

// React to changes
inventory.OnInventoryChanged += () => RefreshInventoryUI();
```

---

## Store System

Orchestrates purchases: validates price → deducts currency → grants item → fires event.

### Setup

1. Create `ItemConfig` assets: **Create → ProtoCasual → Economy → Item Config**
2. Create `ItemDatabase` asset: **Create → ProtoCasual → Economy → Item Database**
3. Drag all `ItemConfig` assets into the database's `items` array
4. Assign the `ItemDatabase` to `GameBootstrap`'s `itemDatabase` field

### Usage

```csharp
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

var store = ServiceLocator.Get<IStoreService>();

// Check if player can afford
if (store.CanPurchase("sword_01"))
{
    // Purchase (tries soft currency first)
    bool success = store.TryPurchase("sword_01");
    
    // Or prefer hard currency
    store.TryPurchase("sword_01", preferHardCurrency: true);
}

// React to purchases
store.OnPurchaseCompleted += (result) =>
{
    Debug.Log($"Bought {result.ItemId} for {result.PricePaid} " +
              $"{(result.UsedHardCurrency ? "gems" : "coins")}");
};
```

### ItemConfig Fields

| Field | Type | Description |
|---|---|---|
| `Id` | string | Unique identifier |
| `DisplayName` | string | UI display name |
| `Description` | string | Item description |
| `Icon` | Sprite | UI icon |
| `Type` | ItemType | Consumable, Cosmetic, Equipment, Currency |
| `Category` | string | Grouping key (e.g., "Weapons", "Skins") |
| `SoftCurrencyPrice` | int | Price in coins |
| `HardCurrencyPrice` | int | Price in gems |
| `IsStackable` | bool | Can own multiples |
| `IsEquippable` | bool | Can be equipped |
| `Prefab` | GameObject | Optional gameplay prefab |
| `EffectValue` | float | Optional stat value |
| `EffectDuration` | float | Optional duration |

### ItemDatabase API

```csharp
using ProtoCasual.Core.Store;

// Get from ServiceLocator
var db = ServiceLocator.Get<ItemDatabase>();

// Lookup
ItemConfig item = db.Get("sword_01");
bool exists = db.Contains("sword_01");

// Filter
var weapons  = db.GetByCategory("Weapons");
var cosmetics = db.GetByType(ItemType.Cosmetic);

// All items
IReadOnlyList<ItemConfig> all = db.All;
```

---

## Save System

### SaveService (Full JSON save/load)

```csharp
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

var save = ServiceLocator.Get<ISaveService>();

// Save any serializable object
save.Save("MyKey", myData);

// Load with default fallback
var data = save.Load("MyKey", new MyData());

// Check / delete
bool exists = save.HasKey("MyKey");
save.Delete("MyKey");
save.DeleteAll();
```

### PlayerDataProvider

All player data (currency, inventory, equipment) is stored in a single `PlayerData` object:

```csharp
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Data;

var provider = ServiceLocator.Get<PlayerDataProvider>();

// Access raw data
int coins = provider.Data.Currency.SoftCurrency;

// Force save
provider.Save();

// Reset to defaults
provider.Reset();
```

### SaveManager (Quick level progress)

```csharp
using ProtoCasual.Core.Managers;

int level = SaveManager.CurrentLevel;
SaveManager.NextLevel();
SaveManager.ResetProgress();
```

---

## Audio Manager

```csharp
using ProtoCasual.Core.Managers;

// Play music (loops automatically)
AudioManager.Instance.PlayMusic(bgmClip);
AudioManager.Instance.StopMusic();

// Play SFX (one-shot)
AudioManager.Instance.PlaySFX(hitClip);

// Play UI sound
AudioManager.Instance.PlayUI(clickClip);

// Toggle
AudioManager.Instance.SetMusicEnabled(false);
AudioManager.Instance.SetSfxEnabled(false);

// Check state
bool musicOn = AudioManager.Instance.IsMusicEnabled;
bool sfxOn   = AudioManager.Instance.IsSfxEnabled;
```

Auto-creates `AudioSource` components if not assigned in the Inspector.

---

## Level Manager

```csharp
using ProtoCasual.Core.Managers;

// Load a specific level
LevelManager.Instance.LoadLevel(0);

// Next level (auto-increments + saves)
LevelManager.Instance.NextLevel();

// Query
int current = LevelManager.Instance.CurrentLevelIndex;
int total   = LevelManager.Instance.TotalLevels;

// Cleanup
LevelManager.Instance.Cleanup();
```

Assign `LevelConfig[]` in the Inspector. Each `LevelConfig` has:
- `levelName` — display name
- `levelPrefab` — instantiated when loaded
- `levelIndex` — sorting index

---

## Event System

ScriptableObject-based events for decoupled communication.

### Create Events

**Create → ProtoCasual → Events → Game Event**

### Raise from Code

```csharp
using ProtoCasual.Core.Events;

[SerializeField] private GameEvent onPlayerDied;

void Die()
{
    onPlayerDied.Raise();
}
```

### Listen from Code

```csharp
onPlayerDied.RegisterAction(HandleDeath);
// ...
onPlayerDied.UnregisterAction(HandleDeath);
```

### Listen from Inspector

Add `GameEventListener` component → assign the `GameEvent` → wire `UnityEvent` response.

### Typed Events

```csharp
// Bool event
[SerializeField] private GameEventBool onToggle;
onToggle.Raise(true);
onToggle.RegisterListener(value => Debug.Log(value));

// Int event
[SerializeField] private GameEventInt onScoreChanged;
onScoreChanged.Raise(1500);
onScoreChanged.RegisterListener(score => scoreText.text = $"{score}");
```

---

## Mechanics System

Plug-in gameplay mechanics. Extend `MechanicBase`:

```csharp
using ProtoCasual.Core.Systems;

public class DoubleJumpMechanic : MechanicBase
{
    public override string MechanicName => "DoubleJump";

    private int jumpsRemaining;

    protected override void OnInitialize()
    {
        jumpsRemaining = 2;
    }

    protected override void OnEnable()
    {
        // Subscribe to input
    }

    protected override void OnDisable()
    {
        // Unsubscribe
    }

    protected override void OnUpdate(float deltaTime)
    {
        // Check jump input, apply force
    }

    protected override void OnCleanup()
    {
        jumpsRemaining = 0;
    }
}
```

### Built-in Mechanics

| Mechanic | Namespace | Description |
|---|---|---|
| `TapToJumpMechanic` | `ProtoCasual.Core.Mechanics` | Tap to jump |
| `SwipeMovementMechanic` | `ProtoCasual.Core.Mechanics` | Swipe-based lane movement |
| `SteeringMechanic` | `ProtoCasual.Core.Mechanics` | Drag/tilt steering |

---

## Map & Endless Generation

### MapGenerator

Procedural or fixed map generation from `MapConfig`:

```csharp
using ProtoCasual.Core.Systems;
using ProtoCasual.Core.ScriptableObjects;

var generator = FindAnyObjectByType<MapGenerator>();
generator.GenerateMap(myMapConfig);
generator.ClearMap();
generator.RegenerateMap();
```

### EndlessGenerator

Infinite chunk-based scrolling:

```csharp
using ProtoCasual.Core.Systems;

// Configure in Inspector:
// - chunkPrefabs (array of level chunks)
// - chunkLength (distance between chunks)
// - activeChunkCount (visible chunks at once)
// - playerTransform (tracks player position)

// Reset (e.g., on restart)
var endless = FindAnyObjectByType<EndlessGenerator>();
endless.Reset();
```

---

## Object Pool

Generic, type-safe pooling:

```csharp
using ProtoCasual.Core.Utilities;

// Create a pool
var bulletPool = new ObjectPool<Bullet>(bulletPrefab, initialSize: 20);

// Get from pool
Bullet b = bulletPool.Get();

// Return to pool
bulletPool.Return(b);

// Return all active objects
bulletPool.ReturnAll();

// Destroy everything
bulletPool.Clear();

// Stats
int active    = bulletPool.ActiveCount;
int available = bulletPool.AvailableCount;
```

---

## Timer

Lightweight, Update-driven timer:

```csharp
using ProtoCasual.Core.Utilities;

var timer = new Timer(5f); // 5 seconds

timer.Start(
    onComplete: () => Debug.Log("Done!"),
    onTick: (elapsed) => Debug.Log($"{elapsed:F1}s")
);

// Call in Update
timer.Update(Time.deltaTime);

// Controls
timer.Pause();
timer.Resume();
timer.Stop();
timer.Reset();

// Read
float progress = timer.Progress;   // 0..1
bool running   = timer.IsRunning;
```

### TimerManager (Coroutine-based)

```csharp
TimerManager.WaitForSeconds(2f, () => Debug.Log("2 seconds later"));
TimerManager.WaitForFrames(5, () => Debug.Log("5 frames later"));
```

---

## Extension Methods

```csharp
using ProtoCasual.Core.Utilities;

// GameObject
gameObject.GetOrAddComponent<Rigidbody>();

// Transform
transform.DestroyAllChildren();
transform.Reset();
transform.FindDeepChild("NestedChild");

// Layer
gameObject.SetLayerRecursively(LayerMask.NameToLayer("Enemy"));

// Vector3
Vector3 v = pos.WithX(0).WithY(5).WithZ(10);
Vector2 flat = pos.ToVector2XZ();

// Vector2
Vector3 world = flat.ToVector3XZ(y: 0);
```

---

## Ads & IAP Interfaces

These are **interfaces only** — implement them with your SDK of choice (AdMob, IronSource, Unity Ads, etc.).

### IAdsService

```csharp
public interface IAdsService
{
    bool IsInitialized { get; }
    bool IsRewardedAdReady { get; }
    bool IsInterstitialAdReady { get; }
    
    void Initialize();
    void ShowInterstitial(Action onClosed = null, Action onFailed = null);
    void ShowRewarded(Action<bool> onComplete);
    void ShowBanner();
    void HideBanner();
}
```

### IIAPService

```csharp
public interface IIAPService
{
    bool IsInitialized { get; }
    void Initialize(Action<bool> onComplete);
    void PurchaseProduct(string productId, Action<bool> onComplete);
    void RestorePurchases(Action<bool> onComplete);
    bool IsProductOwned(string productId);
}
```

### Implementation Example

```csharp
public class MyAdsService : MonoBehaviour, IAdsService
{
    // Implement all IAdsService methods with your SDK
    // Register in your custom GameBootstrap:
    // ServiceLocator.Register<IAdsService>(this);
}
```

---

## Bot System

`IBot` interface for AI opponents:

```csharp
public interface IBot
{
    GameObject GameObject { get; }
    Transform Transform { get; }
    bool IsActive { get; set; }
    void Initialize(BotConfig config);
    void UpdateBot(float deltaTime);
    void Reset();
    void Destroy();
}
```

Configure via `BotConfig` ScriptableObject:
- `botName`, `botPrefab`
- `moveSpeed`, `acceleration`, `turnSpeed`
- `reactionTime`, `errorMargin`
- `useRubberBanding`, `rubberBandStrength`

---

## ScriptableObject Configs

Create via **Create → ProtoCasual →** menu:

| Config | Menu Path | Purpose |
|---|---|---|
| `GameConfig` | Config/Game Config | Target FPS, sound, vibration |
| `GameModeConfig` | Config/GameMode Config | Mode type, name, description |
| `LevelConfig` | Config/Level Config | Level name, prefab, index |
| `MapConfig` | Config/Map Config | Map size, tile size, spawn chances, prefab arrays |
| `BotConfig` | Config/Bot Config | AI params, movement, rubber banding |
| `ItemConfig` | Economy/Item Config | Item definition for store/inventory |
| `ItemDatabase` | Economy/Item Database | Catalogue of all items |

---

## Folder Structure

```
com.bayturan.protocasual/
├── package.json
├── README.md
├── CHANGELOG.md
├── Editor/
│   └── SetupWizard/
│       ├── FrameworkSetup.cs          (Main wizard window)
│       ├── GameSetupConfig.cs         (Config enums + data)
│       ├── SceneBuilder.cs            (Scene generation)
│       ├── ProjectStructureGenerator.cs (Folder + SO creation)
│       └── UIPrefabGenerator.cs       (UI prefab creation)
├── Runtime/
│   ├── Bootstrap/      GameBootstrap, ServiceLocator
│   ├── Currency/       CurrencyService
│   ├── Data/           PlayerData, PlayerDataProvider
│   ├── Events/         GameEvent, GameEventListener, GameEventBool, GameEventInt
│   ├── GameLoop/       GameModeBase, GameState
│   ├── Interfaces/     16 interfaces
│   ├── Inventory/      InventoryService
│   ├── Managers/       GameManager, AudioManager, GameModeManager, LevelManager, SaveManager, SaveService
│   ├── ScriptableObjects/  6 config types
│   ├── Store/          StoreService, ItemDatabase
│   ├── Systems/        InputManager, MapGenerator, EndlessGenerator, MechanicBase, Mechanics/
│   ├── UI/             UIManager, UIScreen, Screens/ (5 screens)
│   └── Utilities/      Singleton, ObjectPool, Timer, Extensions
├── Samples~/
│   └── Example/        Sample game modes, SDK stubs, bots, scenes
└── Tests/
    ├── Editor/         Editor tests
    └── Runtime/        Runtime tests
```

---

## Samples

Import via **Package Manager → Proto Casual → Samples → Import "Example Game"**.

Includes:
- `EndlessRunnerGameMode` — full endless runner with difficulty scaling
- `PuzzleGameMode` — basic puzzle mode template
- `HyperCasualGameMode` — minimal hyper-casual template
- `EndlessGameMode` — endless survival template
- `HybridGameMode` — multi-mechanic hybrid template
- `RacingGameMode` — racing template
- `AdsManager` — sample IAdsService implementation
- `IAPManager` — sample IIAPService implementation
- `BotSpawner` — bot management
- Sample scenes and content

---

## License

See [Third Party Notices.md](Third%20Party%20Notices.md) for details.