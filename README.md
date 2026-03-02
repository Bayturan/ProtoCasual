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
7. [Game Type Guides](#game-type-guides)
   - [HyperCasual](#hypercasual)
   - [Endless Runner](#endless-runner)
   - [Puzzle](#puzzle)
   - [Racing](#racing)
   - [Hybrid](#hybrid)
8. [UI System](#ui-system)
9. [Input System](#input-system)
10. [Currency System](#currency-system)
11. [Inventory System](#inventory-system)
12. [Equipment System](#equipment-system)
13. [Store System](#store-system)
14. [Save System](#save-system)
15. [Audio Manager](#audio-manager)
16. [Level Manager](#level-manager)
17. [Event System](#event-system)
18. [Mechanics System](#mechanics-system)
19. [Map & Endless Generation](#map--endless-generation)
20. [Object Pool](#object-pool)
21. [Timer](#timer)
22. [Extension Methods](#extension-methods)
23. [Haptic Service](#haptic-service)
24. [Popup System](#popup-system)
25. [Reward Service](#reward-service)
26. [Daily Reward Service](#daily-reward-service)
27. [Analytics Service](#analytics-service)
28. [Tutorial System](#tutorial-system)
29. [Leaderboard Service](#leaderboard-service)
30. [Achievement Service](#achievement-service)
31. [Ads & IAP Interfaces](#ads--iap-interfaces)
32. [Bot System](#bot-system)
33. [ScriptableObject Configs](#scriptableobject-configs)
34. [Folder Structure](#folder-structure)
35. [Samples](#samples)

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

The wizard asks 7 core questions plus optional system toggles:

| Question | Options |
|---|---|
| Game Type | HyperCasual, Hybrid, Puzzle, Racing, Endless |
| Map Type | Fixed Levels, Procedural, Endless Generation |
| Bots | None, Simple AI, Advanced AI |
| Monetization | None, Ads Only, IAP Only, Ads + IAP |
| Store | Enabled, Disabled |
| Input Type | Tap, Swipe, Drag, Steering, Mixed |
| Platform | Android, iOS, Both |

| Optional System | Default | Creates |
|---|---|---|
| Haptics | ✓ | HapticConfig, HapticService |
| Analytics | ✓ | AnalyticsConfig, DebugAnalyticsService |
| Reward System | ✓ | RewardConfig[], RewardService |
| Daily Rewards | — | DailyRewardConfig (7-day cycle) |
| Tutorial System | — | TutorialConfig (2 default steps) |
| Leaderboards | — | LeaderboardConfig ("main" board) |
| Achievements | — | AchievementConfig (2 starter achievements) |
| Popup System | ✓ | ConfirmPopup, RewardPopup prefabs + PopupManager |

**What gets generated:**
- `Assets/_Game/` folder structure (Content, Prefabs, ScriptableObjects, UI, Audio, etc.)
- Default ScriptableObject configs (GameConfig, GameModeConfig, LevelConfig, MapConfig, BotConfig + enabled optional configs)
- UI screen prefabs (Menu, Gameplay, Win, Lose, Pause, Settings, Store) + popup prefabs
- 2 scenes: `Main.unity` (menu) + `InGame.unity` (gameplay)
- Full scene hierarchy with all managers, services, and PopupManager
- Build Settings configured

### Quick Setup

**Menu → ProtoCasual → Setup Scene (Quick)**

Uses default settings and generates immediately.

---

## Architecture Overview

```
Runtime/
├── Achievements/       AchievementService
├── Analytics/          DebugAnalyticsService
├── Bootstrap/          GameBootstrap, ServiceLocator
├── Currency/           CurrencyService
├── Data/               PlayerData, PlayerDataProvider
├── Events/             GameEvent, GameEvent<T>, GameEventListener, GameEventBool, GameEventInt, GameEventFloat, GameEventString
├── GameLoop/           GameModeBase, GameState
├── Haptics/            HapticService
├── Interfaces/         25 interfaces (incl. IEquipmentService)
├── Inventory/          InventoryService, EquipmentService
├── Leaderboard/        LocalLeaderboardService
├── Managers/           GameManager, AudioManager, GameModeManager, LevelManager, SaveManager, SaveService
├── Rewards/            RewardService, DailyRewardService
├── ScriptableObjects/  13 config types
├── Store/              StoreService, ItemDatabase
├── Systems/            InputManager, MapGenerator, EndlessGenerator, MechanicBase, Mechanics/
├── Tutorial/           TutorialService
├── UI/                 UIManager, UIScreen, PopupManager, PopupBase, Screens/, Popups/
└── Utilities/          Singleton<T>, ObjectPool<T>, Timer, Extensions
```

**Key patterns:**
- **ServiceLocator** — All services registered and resolved through a central registry
- **Singleton\<T\>** — Thread-safe generic MonoBehaviour singleton with DontDestroyOnLoad. Use `Singleton<T>.HasInstance` to check without triggering lazy creation.
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

// Safe resolve (non-throwing)
if (ServiceLocator.TryGet<IMyService>(out var myService))
{
    myService.DoSomething();
}

// Check if registered
if (ServiceLocator.IsRegistered<IMyService>()) { }

// Unregister
ServiceLocator.Unregister<IMyService>();
```

### Services auto-registered by GameBootstrap

| Service | Interface | Type | Config SO |
|---|---|---|---|
| InputManager | `IInputService` | MonoBehaviour | — |
| SaveService | `ISaveService` | MonoBehaviour | — |
| PlayerDataProvider | `PlayerDataProvider` | Plain C# | — |
| CurrencyService | `ICurrencyService` | Plain C# | — |
| InventoryService | `IInventoryService` | Plain C# | — |
| StoreService | `IStoreService` | Plain C# | ItemDatabase |
| HapticService | `IHapticService` | Plain C# | HapticConfig |
| DebugAnalyticsService | `IAnalyticsService` | Plain C# | AnalyticsConfig |
| RewardService | `IRewardService` | Plain C# | RewardConfig[] |
| DailyRewardService | `IDailyRewardService` | Plain C# | DailyRewardConfig |
| TutorialService | `ITutorialService` | Plain C# | TutorialConfig |
| LocalLeaderboardService | `ILeaderboardService` | Plain C# | LeaderboardConfig |
| AchievementService | `IAchievementService` | Plain C# | AchievementConfig |
| EquipmentService | `IEquipmentService` | Plain C# | — |

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

## Game Type Guides

This section explains what the Setup Wizard generates for **each game type**, what the resulting scene hierarchy looks like, and provides a complete `GameModeBase` implementation you can use as a starting point.

> **Quick start:** Run **ProtoCasual → Create New Game**, pick your game type, click Generate. Then create your GameMode script (examples below), attach it under the `GameModes` object, and assign it in `GameModeManager`.

---

### HyperCasual

**Best for:** Stack runners, ball rollers, paint fillers, single-tap timing games — short sessions, instant restart.

#### Wizard Settings

| Setting | Recommended |
|---|---|
| Game Type | **HyperCasual** |
| Map Type | Fixed Levels or Endless Generation |
| Bots | None |
| Monetization | Ads Only or Ads + IAP |
| Store | Disabled or Enabled |
| Input Type | Tap, Swipe, or Drag |
| Platform | Both |

#### What Gets Generated

**Folders:**
```
Assets/_Game/Content/Chunks/   ← Place your level chunk prefabs here
```

**InGame Scene Hierarchy:**
```
Bootstrap               (GameBootstrap)
GameWorld/
  └── LevelLoader       ← Empty — attach your own level-loading logic
Managers/
  ├── GameManager
  ├── GameModeManager
  ├── AudioManager
  ├── LevelManager
  ├── SaveService
  └── InputManager
Canvas/
  ├── GameplayScreen
  ├── PauseScreen
  ├── WinScreen
  └── LoseScreen
EventSystem
```

#### GameMode Implementation

```csharp
using UnityEngine;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;

public class HyperCasualGameMode : GameModeBase
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private Transform playerTransform;

    private IInputService inputService;
    private float distanceTravelled;
    private float levelLength = 100f;

    public override void Initialize()
    {
        inputService = ServiceLocator.Get<IInputService>();
        distanceTravelled = 0f;

        // Load the current level prefab via LevelManager
        LevelManager.Instance.Init();
    }

    public override void OnGameStart()
    {
        inputService.IsInputEnabled = true;
        distanceTravelled = 0f;
    }

    public override void UpdateMode(float deltaTime)
    {
        if (playerTransform == null) return;

        // Auto-run forward
        playerTransform.position += Vector3.forward * moveSpeed * deltaTime;
        distanceTravelled += moveSpeed * deltaTime;

        // Win condition: reached end of level
        if (distanceTravelled >= levelLength)
        {
            GameManager.Instance.Complete();
        }
    }

    public override void OnGameComplete()
    {
        inputService.IsInputEnabled = false;
        LevelManager.Instance.NextLevel();
    }

    public override void OnGameFail()
    {
        inputService.IsInputEnabled = false;
    }

    public override void Cleanup()
    {
        LevelManager.Instance.Cleanup();
    }
}
```

#### Typical Flow

```
Menu → Play → [auto-run level] → Complete → NextLevel → Play
                               → Fail → Retry / WatchAd
```

#### Recommended Mechanics

| Mechanic | Use Case |
|---|---|
| `SwipeMovementMechanic` | Left/right lane dodging |
| `TapToJumpMechanic` | Obstacle jumping |

#### Tips

- Use `EndlessGenerator` if you want infinite runs instead of fixed levels.
- Keep the core loop under 30 seconds for hyper-casual retention.
- Add interstitial ads on every 2nd death via `IAdsService.ShowInterstitial()`.
- Use `IAnalyticsService.LevelStart()` / `LevelComplete()` / `LevelFail()` to track funnel.

---

### Endless Runner

**Best for:** Subway Surfer clones, auto-scrolling runners, infinite flyers — session length limited by skill.

#### Wizard Settings

| Setting | Recommended |
|---|---|
| Game Type | **Endless** |
| Map Type | Endless Generation |
| Bots | None |
| Monetization | Ads + IAP |
| Store | Enabled |
| Input Type | Swipe or Tap |
| Platform | Both |

#### What Gets Generated

**Folders:**
```
Assets/_Game/Content/Chunks/   ← Place your chunk prefabs here (roads, corridors, platforms)
```

**InGame Scene Hierarchy:**
```
Bootstrap               (GameBootstrap)
GameWorld/
  └── EndlessGenerator  ← EndlessGenerator component attached
Managers/
  ├── GameManager
  ├── GameModeManager
  ├── AudioManager
  ├── LevelManager
  ├── SaveService
  └── InputManager
Canvas/
  ├── GameplayScreen
  ├── PauseScreen
  ├── WinScreen
  └── LoseScreen
EventSystem
```

#### GameMode Implementation

```csharp
using UnityEngine;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.Systems;

public class EndlessRunnerGameMode : GameModeBase
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EndlessGenerator endlessGenerator;

    [Header("Difficulty")]
    [SerializeField] private float startSpeed = 6f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float accelerationPerSecond = 0.1f;

    private IInputService inputService;
    private float currentSpeed;
    private int score;

    public override void Initialize()
    {
        inputService = ServiceLocator.Get<IInputService>();
        currentSpeed = startSpeed;
        score = 0;

        if (endlessGenerator == null)
            endlessGenerator = FindAnyObjectByType<EndlessGenerator>();
    }

    public override void OnGameStart()
    {
        inputService.IsInputEnabled = true;
        currentSpeed = startSpeed;
        endlessGenerator?.Reset();
    }

    public override void UpdateMode(float deltaTime)
    {
        if (playerTransform == null) return;

        // Accelerate over time
        currentSpeed = Mathf.Min(currentSpeed + accelerationPerSecond * deltaTime, maxSpeed);

        // Move player forward
        playerTransform.position += Vector3.forward * currentSpeed * deltaTime;

        // Score = distance
        score = Mathf.FloorToInt(playerTransform.position.z);

        // Update UI
        var gameplay = UI.UIManager.Instance?.GetScreen<UI.GameplayScreen>();
        gameplay?.SetScore(score);
    }

    public override void OnGameFail()
    {
        inputService.IsInputEnabled = false;

        // Submit to leaderboard
        if (ServiceLocator.TryGet<ILeaderboardService>(out var lb))
            lb.SubmitScore("main", score);

        // Track achievement progress
        if (ServiceLocator.TryGet<IAchievementService>(out var ach))
            ach.AddProgress("distance_100", score);
    }

    public override void Cleanup()
    {
        endlessGenerator?.Reset();
    }
}
```

#### Typical Flow

```
Menu → Play → [run until death] → Fail → Retry / WatchAd (continue) / Menu
```

#### Recommended Mechanics

| Mechanic | Use Case |
|---|---|
| `SwipeMovementMechanic` | 3-lane dodging |
| `TapToJumpMechanic` | Obstacle jumping |

#### Tips

- Create 10–15 chunk prefabs for variety, assign them to `EndlessGenerator.chunkPrefabs`.
- Set `EndlessGenerator.playerTransform` to your player so chunks spawn/despawn relative to it.
- Use difficulty scaling: `accelerationPerSecond` makes the game progressively harder.
- Offer a rewarded-ad "second chance" in `LoseScreen` — revive the player and call `GameManager.Instance.Resume()`.
- Track high-scores via `ILeaderboardService.SubmitScore()`.

---

### Puzzle

**Best for:** Match-3, sliding puzzles, word games, Sudoku-likes — discrete levels, turn-based or timed.

#### Wizard Settings

| Setting | Recommended |
|---|---|
| Game Type | **Puzzle** |
| Map Type | Fixed Levels |
| Bots | None |
| Monetization | Ads + IAP |
| Store | Enabled |
| Input Type | Tap or Drag |
| Platform | Both |

#### What Gets Generated

**Folders:**
```
Assets/_Game/Content/Puzzles/   ← Place your puzzle data assets / prefabs here
```

**InGame Scene Hierarchy:**
```
Bootstrap               (GameBootstrap)
GameWorld/
  ├── GridSystem        ← Empty — attach your grid/board logic
  └── MatchLogic        ← Empty — attach your match/win detection
Managers/
  ├── GameManager
  ├── GameModeManager
  ├── AudioManager
  ├── LevelManager
  ├── SaveService
  └── InputManager
Canvas/
  ├── GameplayScreen
  ├── PauseScreen
  ├── WinScreen
  └── LoseScreen
EventSystem
```

#### GameMode Implementation

```csharp
using UnityEngine;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.UI;

public class PuzzleGameMode : GameModeBase
{
    [Header("Puzzle")]
    [SerializeField] private int gridWidth = 6;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private int targetScore = 1000;
    [SerializeField] private int maxMoves = 20;

    private int currentScore;
    private int movesRemaining;
    private IInputService inputService;

    // Reference to your custom grid/board component
    // [SerializeField] private GridBoard board;

    public override void Initialize()
    {
        inputService = ServiceLocator.Get<IInputService>();
        LevelManager.Instance.Init();
    }

    public override void OnGameStart()
    {
        currentScore = 0;
        movesRemaining = maxMoves;
        inputService.IsInputEnabled = true;

        // Generate board
        // board.GenerateGrid(gridWidth, gridHeight);

        UpdateUI();
    }

    /// <summary>
    /// Call this from your match/swap logic when the player makes a move.
    /// </summary>
    public void OnPlayerMove(int pointsScored)
    {
        currentScore += pointsScored;
        movesRemaining--;
        UpdateUI();

        // Win condition
        if (currentScore >= targetScore)
        {
            GameManager.Instance.Complete();
            return;
        }

        // Lose condition
        if (movesRemaining <= 0)
        {
            GameManager.Instance.Fail();
        }
    }

    public override void OnGameComplete()
    {
        inputService.IsInputEnabled = false;

        // Grant level reward
        if (ServiceLocator.TryGet<IRewardService>(out var rewards))
            rewards.GrantLevelReward(LevelManager.Instance.CurrentLevelIndex);

        // Track analytics
        if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
            analytics.LevelComplete(LevelManager.Instance.CurrentLevelIndex,
                                    GameManager.Instance.GetGameTime());
    }

    public override void OnGameFail()
    {
        inputService.IsInputEnabled = false;

        if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
            analytics.LevelFail(LevelManager.Instance.CurrentLevelIndex,
                                GameManager.Instance.GetGameTime());
    }

    public override void OnGamePause()
    {
        inputService.IsInputEnabled = false;
    }

    public override void OnGameResume()
    {
        inputService.IsInputEnabled = true;
    }

    public override void Cleanup()
    {
        // board?.ClearGrid();
        LevelManager.Instance.Cleanup();
    }

    private void UpdateUI()
    {
        var gameplay = UIManager.Instance?.GetScreen<GameplayScreen>();
        gameplay?.SetScore(currentScore);
        gameplay?.SetProgress((float)currentScore / targetScore);
    }
}
```

#### Typical Flow

```
Menu → Play → [solve puzzle within move limit] → Complete → NextLevel
                                                → Fail → Retry / WatchAd (+5 moves)
```

#### Tips

- Use `LevelConfig` ScriptableObjects to define per-level parameters (grid size, target score, move limit).
- The `GridSystem` and `MatchLogic` GameObjects are empty stubs — attach your own board and match-detection scripts.
- Use `IInputService.OnTap` for tile selection and `IInputService.OnDrag` for swipe-to-swap.
- Show an interstitial ad every 3 levels using `IAdsService.ShowInterstitial()`.
- Store power-ups (extra moves, hints) via `IStoreService` + `IInventoryService`.

---

### Racing

**Best for:** Kart racers, drag racers, lane-based racing, traffic dodging — timed laps or first-to-finish.

#### Wizard Settings

| Setting | Recommended |
|---|---|
| Game Type | **Racing** |
| Map Type | Fixed Levels |
| Bots | Simple AI or Advanced AI |
| Monetization | Ads + IAP |
| Store | Enabled |
| Input Type | Steering or Swipe |
| Platform | Both |

#### What Gets Generated

**Folders:**
```
Assets/_Game/Content/Tracks/    ← Place your track prefabs here
Assets/_Game/Content/Vehicles/  ← Place your vehicle prefabs / models here
Assets/_Game/Content/Bots/      ← Place bot variant configs here (if bots enabled)
```

**InGame Scene Hierarchy:**
```
Bootstrap               (GameBootstrap)
GameWorld/
  ├── TrackGenerator    ← Empty — attach your track loading/generation logic
  └── BotSpawner        ← Empty — attach your bot spawning logic (if bots enabled)
Managers/
  ├── GameManager
  ├── GameModeManager
  ├── AudioManager
  ├── LevelManager
  ├── SaveService
  └── InputManager
Canvas/
  ├── GameplayScreen
  ├── PauseScreen
  ├── WinScreen
  └── LoseScreen
EventSystem
```

#### GameMode Implementation

```csharp
using UnityEngine;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.UI;

public class RacingGameMode : GameModeBase
{
    [Header("Track")]
    [SerializeField] private Transform[] checkpoints;
    [SerializeField] private int totalLaps = 3;

    [Header("Player")]
    [SerializeField] private Transform playerVehicle;
    [SerializeField] private float topSpeed = 30f;

    [Header("Bots")]
    [SerializeField] private GameObject[] botPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    private IInputService inputService;
    private int currentCheckpoint;
    private int currentLap;
    private int playerPosition;  // 1st, 2nd, 3rd...

    public override void Initialize()
    {
        inputService = ServiceLocator.Get<IInputService>();

        // Load track from LevelConfig
        LevelManager.Instance.Init();

        // Spawn bots
        SpawnBots();
    }

    public override void OnGameStart()
    {
        inputService.IsInputEnabled = true;
        currentCheckpoint = 0;
        currentLap = 0;
    }

    public override void UpdateMode(float deltaTime)
    {
        if (playerVehicle == null) return;

        // Check if player reached the next checkpoint
        if (checkpoints != null && currentCheckpoint < checkpoints.Length)
        {
            float dist = Vector3.Distance(playerVehicle.position,
                                          checkpoints[currentCheckpoint].position);
            if (dist < 5f)
            {
                currentCheckpoint++;

                // Completed a lap?
                if (currentCheckpoint >= checkpoints.Length)
                {
                    currentCheckpoint = 0;
                    currentLap++;

                    if (currentLap >= totalLaps)
                    {
                        GameManager.Instance.Complete();
                    }
                }
            }
        }

        // Update HUD
        var gameplay = UIManager.Instance?.GetScreen<GameplayScreen>();
        gameplay?.SetProgress((float)(currentLap * checkpoints.Length + currentCheckpoint)
                              / (totalLaps * checkpoints.Length));
    }

    public override void OnGameComplete()
    {
        inputService.IsInputEnabled = false;

        // Submit race time to leaderboard
        float raceTime = GameManager.Instance.GetGameTime();
        if (ServiceLocator.TryGet<ILeaderboardService>(out var lb))
            lb.SubmitScore("race_time", Mathf.FloorToInt(raceTime * 1000));
    }

    public override void OnGameFail()
    {
        inputService.IsInputEnabled = false;
    }

    public override void Cleanup()
    {
        DestroyBots();
        LevelManager.Instance.Cleanup();
    }

    private void SpawnBots()
    {
        // Use BotConfig + IBot interface to spawn AI racers
        // at spawnPoints positions
    }

    private void DestroyBots()
    {
        // Cleanup bot instances
    }
}
```

#### Typical Flow

```
Menu → Play → [countdown 3-2-1] → Racing → [complete laps] → Complete → NextTrack
                                           → Fail (timed out / crashed) → Retry
```

#### Recommended Mechanics

| Mechanic | Use Case |
|---|---|
| `SteeringMechanic` | Full steering wheel / tilt controls |
| `SwipeMovementMechanic` | Lane-based racing (swipe left/right) |

#### Tips

- Use the `SteeringMechanic` for freeform driving or `SwipeMovementMechanic` for lane-based racing.
- Store each track as a `LevelConfig` with the track prefab. The `TrackGenerator` object is an empty stub for your track-loading code.
- Create `BotConfig` ScriptableObjects with different difficulty tiers. Use `useRubberBanding` to keep races competitive.
- Use the `Prepare` state for a countdown: `GameManager.Instance.ChangeState(GameState.Prepare)` → wait 3 seconds → `GameManager.Instance.Play()`.
- Sell vehicle skins via the Store system with `ItemType.Cosmetic`.

---

### Hybrid

**Best for:** Games that mix multiple mechanics — base-builder + runner, RPG + puzzle, merge + idle — or games that don't fit neatly into one category.

#### Wizard Settings

| Setting | Recommended |
|---|---|
| Game Type | **Hybrid** |
| Map Type | Fixed Levels, Procedural, or Endless |
| Bots | Any |
| Monetization | Ads + IAP |
| Store | Enabled |
| Input Type | Mixed |
| Platform | Both |

#### What Gets Generated

**Folders:**
```
Assets/_Game/Content/Levels/   ← Place your level data / prefabs here
```

If **Map Type** is set to Procedural or Endless, also creates a `MapGenerator` in the scene.

**InGame Scene Hierarchy:**
```
Bootstrap               (GameBootstrap)
GameWorld/
  ├── LevelLoader       ← Empty — attach your level loading logic
  └── MapGenerator      ← MapGenerator component (if procedural/endless selected)
Managers/
  ├── GameManager
  ├── GameModeManager
  ├── AudioManager
  ├── LevelManager
  ├── SaveService
  └── InputManager
Canvas/
  ├── GameplayScreen
  ├── PauseScreen
  ├── WinScreen
  └── LoseScreen
EventSystem
```

#### GameMode Implementation — Multi-Phase Example

A hybrid game often has distinct **phases** within a single session. Here's a pattern that switches between a build phase and an action phase:

```csharp
using UnityEngine;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;
using ProtoCasual.Core.Managers;
using ProtoCasual.Core.Systems;
using ProtoCasual.Core.UI;

public class HybridGameMode : GameModeBase
{
    public enum Phase { Build, Action }

    [Header("Phases")]
    [SerializeField] private float buildPhaseDuration = 30f;
    [SerializeField] private float actionPhaseDuration = 60f;

    [Header("Map")]
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private ScriptableObjects.MapConfig mapConfig;

    [Header("Mechanics")]
    [SerializeField] private MechanicBase[] buildMechanics;
    [SerializeField] private MechanicBase[] actionMechanics;

    private IInputService inputService;
    private Phase currentPhase;
    private float phaseTimer;
    private int score;

    public override void Initialize()
    {
        inputService = ServiceLocator.Get<IInputService>();

        // Generate the map
        if (mapGenerator != null && mapConfig != null)
            mapGenerator.GenerateMap(mapConfig);
    }

    public override void OnGameStart()
    {
        inputService.IsInputEnabled = true;
        score = 0;
        StartPhase(Phase.Build);
    }

    public override void UpdateMode(float deltaTime)
    {
        phaseTimer -= deltaTime;

        // Update active mechanics
        var activeMechanics = currentPhase == Phase.Build ? buildMechanics : actionMechanics;
        if (activeMechanics != null)
        {
            foreach (var m in activeMechanics)
                m.UpdateMechanic(deltaTime);
        }

        // Phase timer expired
        if (phaseTimer <= 0f)
        {
            switch (currentPhase)
            {
                case Phase.Build:
                    StartPhase(Phase.Action);
                    break;
                case Phase.Action:
                    GameManager.Instance.Complete();
                    break;
            }
        }

        // Update HUD
        var gameplay = UIManager.Instance?.GetScreen<GameplayScreen>();
        gameplay?.SetScore(score);
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public override void OnGameComplete()
    {
        inputService.IsInputEnabled = false;
        DisableAllMechanics();

        if (ServiceLocator.TryGet<IRewardService>(out var rewards))
            rewards.GrantLevelReward(LevelManager.Instance.CurrentLevelIndex);
    }

    public override void OnGameFail()
    {
        inputService.IsInputEnabled = false;
        DisableAllMechanics();
    }

    public override void Cleanup()
    {
        DisableAllMechanics();
        mapGenerator?.ClearMap();
    }

    private void StartPhase(Phase phase)
    {
        currentPhase = phase;
        DisableAllMechanics();

        switch (phase)
        {
            case Phase.Build:
                phaseTimer = buildPhaseDuration;
                EnableMechanics(buildMechanics);
                break;
            case Phase.Action:
                phaseTimer = actionPhaseDuration;
                EnableMechanics(actionMechanics);
                break;
        }
    }

    private void EnableMechanics(MechanicBase[] mechanics)
    {
        if (mechanics == null) return;
        foreach (var m in mechanics)
        {
            m.Initialize();
            m.Enable();
        }
    }

    private void DisableAllMechanics()
    {
        DisableMechanics(buildMechanics);
        DisableMechanics(actionMechanics);
    }

    private void DisableMechanics(MechanicBase[] mechanics)
    {
        if (mechanics == null) return;
        foreach (var m in mechanics) m.Disable();
    }
}
```

#### Typical Flow

```
Menu → Play → [Build Phase: 30s] → [Action Phase: 60s] → Complete → NextLevel
                                                        → Fail → Retry
```

#### Tips

- Use the `buildMechanics` / `actionMechanics` split to enable/disable different input schemes per phase.
- The `MapGenerator` is ideal for hybrid games with procedurally generated arenas.
- Combine the `Inventory` + `Equipment` + `Store` systems to create RPG-like item loops.
- Use `GameModeManager` to support multiple sub-modes within a single hybrid game (e.g., "Story" vs "Endless" modes).
- Track phase-specific analytics: `analytics.TrackEvent("phase_complete", "phase", currentPhase.ToString())`.

---

### Quick Comparison

| Feature | HyperCasual | Endless | Puzzle | Racing | Hybrid |
|---|---|---|---|---|---|
| **Session length** | 15–30s | 1–5 min | 1–3 min | 1–3 min | 2–10 min |
| **Level structure** | Fixed / Endless | Infinite chunks | Fixed levels | Track-based | Multi-phase |
| **Win condition** | Reach end | Beat high score | Target score | Finish laps | Phase-based |
| **Primary input** | Tap / Swipe | Swipe / Tap | Tap / Drag | Steering | Mixed |
| **Recommended map** | Fixed or Endless | Endless Generation | Fixed | Fixed | Procedural |
| **Bots** | Rare | None | None | Common | Optional |
| **Store** | Optional | Recommended | Recommended | Recommended | Recommended |
| **Key mechanic** | SwipeMovement | TapToJump | Custom grid | Steering | Multiple |
| **Generated folders** | `Chunks/` | `Chunks/` | `Puzzles/` | `Tracks/`, `Vehicles/` | `Levels/` |
| **Scene objects** | LevelLoader | EndlessGenerator | GridSystem, MatchLogic | TrackGenerator, BotSpawner | LevelLoader, MapGenerator |

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
| `MenuScreen` | Play, Store, Inventory, Settings buttons |
| `GameplayScreen` | Score text, time text, pause button, progress slider |
| `WinScreen` | Score, time, Next Level / Restart / Menu buttons |
| `LoseScreen` | Retry, Menu, Watch Ad (rewarded) buttons |
| `PauseScreen` | Resume, Restart, Quit buttons |
| `SettingsScreen` | Music/SFX toggles, volume sliders, vibration toggle, Reset Data (with ConfirmPopup) |
| `StoreScreen` | Browse items, buy with soft/hard currency, detail panel, live currency display |
| `InventoryScreen` | View owned items, quantity badges, detail panel, empty state |

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

## Equipment System

Slot-based equipment that integrates with the Inventory System. Equipping an item consumes it from inventory; unequipping returns it.

```csharp
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

var equip = ServiceLocator.Get<IEquipmentService>();

// Equip (removes the item from inventory)
equip.Equip("weapon", "sword_01");
equip.Equip("helmet", "iron_helm");

// Query
string weapon = equip.GetEquipped("weapon"); // "sword_01"
bool empty = equip.IsSlotEmpty("shield");     // true

// Unequip (returns item to inventory)
equip.Unequip("weapon");

// List all slots
var slots = equip.GetAll();
foreach (var slot in slots)
    Debug.Log($"[{slot.SlotName}] = {slot.ItemId}");

// Clear all (returns everything to inventory)
equip.Clear();

// React to changes
equip.OnEquipmentChanged += () => RefreshEquipmentUI();
```

> **Note:** `EquipmentService` is auto-registered by `GameBootstrap`. It takes an optional `IInventoryService` dependency — if present, item counts are kept in sync automatically.

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

### StoreScreen

A full-featured store UI that lets the player browse and purchase items. Extends `UIScreen`.

**Features:**
- Displays all items from `ItemDatabase` in a scrollable list
- Detail panel with icon, name, description, and price on item tap
- Separate **Buy (Coins)** and **Buy (Gems)** buttons
- Live currency balance header (soft + hard)
- Greys out already-owned non-stackable items
- Feedback text for purchase success/failure
- Auto-refreshes on `OnPurchaseCompleted` and `OnCurrencyChanged`

**Inspector fields:**

| Field | Type | Description |
|---|---|---|
| `itemDatabase` | ItemDatabase | Reference to the item catalogue |
| `softCurrencyText` | TMP_Text | Displays coin balance |
| `hardCurrencyText` | TMP_Text | Displays gem balance |
| `itemContainer` | Transform | Parent for spawned item entries |
| `storeItemPrefab` | GameObject | Prefab for each store item row (needs Image, TMP_Text, Button) |
| `detailPanel` | GameObject | Panel shown when an item is selected |
| `detailIcon` | Image | Item icon in detail panel |
| `detailName` | TMP_Text | Item name in detail panel |
| `detailDescription` | TMP_Text | Item description in detail panel |
| `detailPrice` | TMP_Text | Price label in detail panel |
| `buySoftButton` | Button | Purchase with soft currency |
| `buyHardButton` | Button | Purchase with hard currency |
| `closeButton` | Button | Returns to MenuScreen |
| `feedbackText` | TMP_Text | Shows "Purchased!" or "Not enough currency!" |

**Prefab requirements for `storeItemPrefab`:**
- `Image` component (child) — receives the item icon
- `TextMeshProUGUI` component (child) — receives the display name
- `Button` component (root or child) — handles item selection

**Navigation:**

```csharp
// Open from any screen
UIManager.Instance.ShowScreen(nameof(StoreScreen));
```

### InventoryScreen

Displays the player's owned items with quantities. Extends `UIScreen`.

**Features:**
- Lists all items from `IInventoryService.GetAll()`
- Resolves display info (icon, name, description) from `ItemDatabase`
- Shows quantity badges (`x3`) for stackable items
- Detail panel with item type and owned count
- Empty state panel when inventory is empty
- Auto-refreshes on `OnInventoryChanged`

**Inspector fields:**

| Field | Type | Description |
|---|---|---|
| `itemDatabase` | ItemDatabase | Resolves display info for item IDs |
| `itemContainer` | Transform | Parent for spawned inventory entries |
| `inventoryItemPrefab` | GameObject | Prefab for each inventory row (needs Image, TMP_Text, Button) |
| `detailPanel` | GameObject | Panel shown when an item is selected |
| `detailIcon` | Image | Item icon in detail panel |
| `detailName` | TMP_Text | Item name |
| `detailDescription` | TMP_Text | Item description |
| `detailQuantity` | TMP_Text | "Owned: 5" |
| `detailType` | TMP_Text | Item type (Consumable, Cosmetic, etc.) |
| `emptyStatePanel` | GameObject | Shown when inventory is empty |
| `emptyStateText` | TMP_Text | "No items yet. Visit the Store!" |
| `closeButton` | Button | Returns to MenuScreen |

**Prefab requirements for `inventoryItemPrefab`:**
- `Image` component (child) — receives the item icon
- `TextMeshProUGUI` component (child) — receives name + quantity
- `Button` component (root or child) — handles item selection

**Navigation:**

```csharp
// Open from any screen
UIManager.Instance.ShowScreen(nameof(InventoryScreen));
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

All typed events extend the generic `GameEvent<T>` base class.

**Built-in typed events:**

| Asset Menu Path | Class | Value Type |
|---|---|---|
| ProtoCasual/Events/Bool Event | `GameEventBool` | `bool` |
| ProtoCasual/Events/Int Event | `GameEventInt` | `int` |
| ProtoCasual/Events/Float Event | `GameEventFloat` | `float` |
| ProtoCasual/Events/String Event | `GameEventString` | `string` |

```csharp
// Bool event
[SerializeField] private GameEventBool onToggle;
onToggle.Raise(true);
onToggle.RegisterListener(value => Debug.Log(value));

// Int event
[SerializeField] private GameEventInt onScoreChanged;
onScoreChanged.Raise(1500);
onScoreChanged.RegisterListener(score => scoreText.text = $"{score}");

// Float event
[SerializeField] private GameEventFloat onHealthChanged;
onHealthChanged.Raise(0.75f);
onHealthChanged.RegisterListener(hp => healthBar.fillAmount = hp);

// String event
[SerializeField] private GameEventString onNotification;
onNotification.Raise("Level Up!");
onNotification.RegisterListener(msg => notificationText.text = msg);
```

### Custom Typed Events

Create your own typed event in one line by extending `GameEvent<T>`:

```csharp
using UnityEngine;
using ProtoCasual.Core.Events;

[CreateAssetMenu(menuName = "ProtoCasual/Events/Vector3 Event")]
public class GameEventVector3 : GameEvent<Vector3> { }
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

    protected override void OnMechanicInitialize()
    {
        jumpsRemaining = 2;
    }

    protected override void OnMechanicEnable()
    {
        // Subscribe to input
    }

    protected override void OnMechanicDisable()
    {
        // Unsubscribe
    }

    protected override void OnMechanicUpdate(float deltaTime)
    {
        // Check jump input, apply force
    }

    protected override void OnMechanicCleanup()
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

## Haptic Service

Platform haptic feedback via `IHapticService`. Wraps Android `Vibrator` and iOS `Handheld.Vibrate()`.

Configure durations in `HapticConfig` (Create → ProtoCasual → Config → Haptic Config).

```csharp
var haptics = ServiceLocator.Get<IHapticService>();

haptics.IsEnabled = true;

haptics.LightImpact();   // button tap
haptics.MediumImpact();  // selection confirm
haptics.HeavyImpact();   // collision
haptics.Selection();     // scroll snap
haptics.Success();       // purchase / level complete
haptics.Warning();       // low currency
haptics.Error();         // purchase failed
```

### HapticConfig Fields

| Field | Type | Default | Description |
|---|---|---|---|
| `enabled` | bool | true | Global enable/disable |
| `lightDuration` | long | 20 | Light impact (ms) |
| `mediumDuration` | long | 40 | Medium impact (ms) |
| `heavyDuration` | long | 80 | Heavy impact (ms) |
| `selectionDuration` | long | 10 | Selection feedback (ms) |
| `successDuration` | long | 50 | Success feedback (ms) |
| `warningDuration` | long | 60 | Warning feedback (ms) |
| `errorDuration` | long | 100 | Error feedback (ms) |

---

## Popup System

Overlay popup manager with stacking support. Place `PopupManager` on a dedicated high-sort-order Canvas.

```csharp
using ProtoCasual.Core.UI;

// Show by name
PopupManager.Instance.ShowPopup("ConfirmPopup", new ConfirmPopupData
{
    Title = "Quit?",
    Message = "Are you sure?",
    OnConfirm = () => Application.Quit(),
    OnCancel = () => Debug.Log("Cancelled")
});

// Show by type
PopupManager.Instance.ShowPopup<RewardPopup>(new RewardPopupData
{
    Title = "Level Complete!",
    Rewards = new[] { new RewardEntry { Type = RewardType.SoftCurrency, Amount = 500 } }
});

// Hide
PopupManager.Instance.HideTopPopup();
PopupManager.Instance.HideAll();

// Check state
bool busy = PopupManager.Instance.HasActivePopup;
```

### Built-in Popups

| Popup | Data Class | Purpose |
|---|---|---|
| `ConfirmPopup` | `ConfirmPopupData` | OK/Cancel confirmation dialog |
| `RewardPopup` | `RewardPopupData` | Shows granted rewards with Collect button |

### Custom Popup

```csharp
public class MyPopup : PopupBase
{
    public override string PopupName => "MyPopup";

    protected override void OnShow(object data) { /* populate UI */ }
    protected override void OnHide() { /* cleanup */ }
}
```

---

## Reward Service

Centralized reward granting — delegates to `ICurrencyService` and `IInventoryService`.

```csharp
var rewards = ServiceLocator.Get<IRewardService>();

// Grant arbitrary rewards
rewards.GrantRewards(
    new RewardEntry { Type = RewardType.SoftCurrency, Amount = 500 },
    new RewardEntry { Type = RewardType.HardCurrency, Amount = 10 },
    new RewardEntry { Type = RewardType.Item, RewardId = "sword_01", Amount = 1 }
);

// Grant level-mapped reward (from RewardConfig SOs)
rewards.GrantLevelReward(levelIndex: 3);

// React
rewards.OnRewardsGranted += (entries) => ShowRewardPopup(entries);
```

### RewardConfig (Create → ProtoCasual → Economy → Reward Config)

| Field | Type | Description |
|---|---|---|
| `rewardId` | string | Unique reward ID |
| `displayName` | string | UI label |
| `rewards` | RewardEntry[] | Array of currency/item rewards |
| `isLevelReward` | bool | Maps to a level index |
| `levelIndex` | int | Which level grants this reward |

---

## Daily Reward Service

Login-streak daily reward system. Persists in `PlayerData.DailyReward`.

```csharp
var daily = ServiceLocator.Get<IDailyRewardService>();

// Check availability
if (daily.CanClaimToday)
{
    var preview = daily.PeekTodayReward(); // see what you'll get
    daily.TryClaim();                       // claim and grant
}

int streak = daily.CurrentStreak;
DateTime last = daily.LastClaimTime;

// Listen
daily.OnDailyRewardClaimed += (streak, rewards) =>
    Debug.Log($"Day {streak} claimed!");

// Admin reset
daily.Reset();
```

### DailyRewardConfig (Create → ProtoCasual → Economy → Daily Reward Config)

| Field | Type | Default | Description |
|---|---|---|---|
| `cycleDays` | int | 7 | Days before the cycle loops |
| `streakExpiryHours` | int | 48 | Hours to keep the streak alive |
| `days` | DayReward[] | — | Rewards per day (label, icon, RewardEntry[]) |

---

## Analytics Service

Fire-and-forget event tracking via `IAnalyticsService`. Default implementation logs to console.
Replace with Firebase/GameAnalytics by registering your own implementation.

```csharp
var analytics = ServiceLocator.Get<IAnalyticsService>();

// Generic events
analytics.TrackEvent("button_click");
analytics.TrackEvent("screen_open", "screen", "StoreScreen");

// Typed convenience methods
analytics.LevelStart(levelIndex);
analytics.LevelComplete(levelIndex, duration);
analytics.LevelFail(levelIndex, duration);
analytics.Purchase("sword_01", 500, hardCurrency: false);
analytics.AdWatched("rewarded", "level_complete");
analytics.TutorialStep(0, "welcome");

// Toggle
analytics.IsEnabled = false;
```

### AnalyticsConfig (Create → ProtoCasual → Config → Analytics Config)

| Field | Type | Default | Description |
|---|---|---|---|
| `enabled` | bool | true | Master enable |
| `debugLogging` | bool | true | Log to console |
| `trackLevelEvents` | bool | true | Track level start/complete/fail |
| `trackPurchaseEvents` | bool | true | Track purchases |
| `trackAdEvents` | bool | true | Track ad views |
| `trackTutorialEvents` | bool | true | Track tutorial steps |

---

## Tutorial System

Step-based onboarding backed by `TutorialConfig`. Persists completion in `PlayerData.Tutorial`.

```csharp
var tutorial = ServiceLocator.Get<ITutorialService>();

// Start (checks config.autoStart on first launch)
tutorial.StartTutorial();

// Advance manually
tutorial.CompleteCurrentStep();

// Skip
tutorial.SkipTutorial(); // only if config.allowSkip == true

// Query
bool active = tutorial.IsTutorialActive;
bool done = tutorial.IsTutorialCompleted;
int step = tutorial.CurrentStepIndex;

// Listen
tutorial.OnStepStarted += (index, stepId) => ShowTutorialOverlay(index);
tutorial.OnStepCompleted += (index, stepId) => HideTutorialOverlay();
tutorial.OnTutorialCompleted += () => Debug.Log("Tutorial done!");

// Reset for testing
tutorial.Reset();
```

### TutorialConfig (Create → ProtoCasual → Config → Tutorial Config)

| Field | Type | Default | Description |
|---|---|---|---|
| `autoStart` | bool | true | Auto-start on first launch |
| `allowSkip` | bool | true | Allow player to skip |
| `steps` | TutorialStepData[] | — | Ordered list of steps |

### TutorialStepData

| Field | Type | Description |
|---|---|---|
| `stepId` | string | Unique step key |
| `title` | string | Step title |
| `message` | string | Instruction text |
| `image` | Sprite | Optional illustration |
| `highlightTarget` | string | Name of UI element to highlight |
| `completeOnTap` | bool | Advance on any tap |
| `autoAdvanceDelay` | float | Auto-advance seconds (0 = manual) |

---

## Leaderboard Service

Local leaderboard backed by `PlayerData.Leaderboards`. Replace with cloud backend by implementing `ILeaderboardService`.

```csharp
var lb = ServiceLocator.Get<ILeaderboardService>();

// Submit score (keeps best)
lb.SubmitScore("main", 15000);

// Load entries
lb.LoadLeaderboard("main", maxEntries: 10);
lb.OnLeaderboardLoaded += (id, entries) =>
{
    foreach (var e in entries)
        Debug.Log($"#{e.Rank} {e.PlayerName}: {e.Score}");
};

// Quick lookups
var best = lb.GetPlayerBest("main");
var cached = lb.GetCachedEntries("main");
```

### LeaderboardConfig (Create → ProtoCasual → Config → Leaderboard Config)

| Field | Type | Default | Description |
|---|---|---|---|
| `leaderboards` | LeaderboardDefinition[] | — | Array of board definitions |

### LeaderboardDefinition

| Field | Type | Default | Description |
|---|---|---|---|
| `leaderboardId` | string | — | Unique board ID |
| `displayName` | string | — | UI label |
| `maxEntries` | int | 100 | Max cached entries |
| `descending` | bool | true | Sort order (highest first) |

---

## Achievement Service

Condition-based unlock system. Auto-grants rewards via `IRewardService` on unlock. Persists in `PlayerData.Achievements`.

```csharp
var ach = ServiceLocator.Get<IAchievementService>();

// Increment progress
ach.AddProgress("collector", 1);
ach.AddProgress("first_win");

// Query
bool unlocked = ach.IsUnlocked("first_win");
int progress = ach.GetProgress("collector");
var all = ach.GetAll(); // List<AchievementProgress>

// Listen
ach.OnAchievementUnlocked += (id) => ShowAchievementPopup(id);
ach.OnAchievementProgress += (id, current) => UpdateProgressBar(id, current);

// Reset
ach.Reset();
```

### AchievementConfig (Create → ProtoCasual → Config → Achievement Config)

| Field | Type | Description |
|---|---|---|
| `achievements` | AchievementDefinition[] | Array of achievement definitions |

### AchievementDefinition

| Field | Type | Default | Description |
|---|---|---|---|
| `achievementId` | string | — | Unique ID |
| `displayName` | string | — | UI label |
| `description` | string | — | Description text |
| `icon` / `lockedIcon` | Sprite | — | Unlocked/locked icons |
| `requiredProgress` | int | 1 | Progress needed to unlock |
| `rewards` | RewardEntry[] | — | Granted on unlock |
| `isHidden` | bool | false | Hidden until unlocked |

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
| `HapticConfig` | Config/Haptic Config | Vibration durations per pattern |
| `AnalyticsConfig` | Config/Analytics Config | Event toggles, debug logging |
| `TutorialConfig` | Config/Tutorial Config | Step definitions, auto-start, allow skip |
| `LeaderboardConfig` | Config/Leaderboard Config | Board definitions, sort order, max entries |
| `AchievementConfig` | Config/Achievement Config | Achievement definitions, required progress, rewards |
| `ItemConfig` | Economy/Item Config | Item definition for store/inventory |
| `ItemDatabase` | Economy/Item Database | Catalogue of all items |
| `RewardConfig` | Economy/Reward Config | Reward tiers, level-mapped rewards |
| `DailyRewardConfig` | Economy/Daily Reward Config | Streak cycle, daily reward arrays |

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
│   ├── Achievements/   AchievementService
│   ├── Analytics/      DebugAnalyticsService
│   ├── Bootstrap/      GameBootstrap, ServiceLocator
│   ├── Currency/       CurrencyService
│   ├── Data/           PlayerData, PlayerDataProvider
│   ├── Events/         GameEvent, GameEvent<T>, GameEventListener, GameEventBool, GameEventInt, GameEventFloat, GameEventString
│   ├── GameLoop/       GameModeBase, GameState
│   ├── Haptics/        HapticService
│   ├── Interfaces/     25 interfaces (incl. IEquipmentService)
│   ├── Inventory/      InventoryService, EquipmentService
│   ├── Leaderboard/    LocalLeaderboardService
│   ├── Managers/       GameManager, AudioManager, GameModeManager, LevelManager, SaveManager, SaveService
│   ├── Rewards/        RewardService, DailyRewardService
│   ├── ScriptableObjects/  13 config types
│   ├── Store/          StoreService, ItemDatabase
│   ├── Systems/        InputManager, MapGenerator, EndlessGenerator, MechanicBase, Mechanics/
│   ├── Tutorial/       TutorialService
│   ├── UI/             UIManager, UIScreen, PopupManager, PopupBase, Screens/ (8), Popups/ (2)
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