# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2026-02-25

### Fixed
- **GameManager resume bug** — `HandleStateEnter` now receives the previous state so resuming from Pause correctly preserves `gameTime` and fires `OnGameResume` instead of `OnGameStart`.
- **Singleton `applicationIsQuitting` was static** — destroying any singleton made *all* singletons inaccessible. The flag is now replaced by nulling the instance and using `Application.isPlaying`.
- **Singleton used deprecated `FindObjectOfType`** — replaced with `FindAnyObjectByType`.
- **MechanicBase `OnEnable`/`OnDisable` shadowed Unity lifecycle** — renamed abstract hooks to `OnMechanicEnable`/`OnMechanicDisable` etc.
- **LevelManager coupled to static `SaveManager`** — now uses `ISaveService` via ServiceLocator for consistent save/load.
- **Version mismatch** — `package.json` now matches README at `0.2.0`.
- **`GameEventBool`/`GameEventInt` inconsistent `CreateAssetMenu` paths** — unified under `ProtoCasual/Events/`.
- **Unused `using System.Collections` import** in Timer.cs.

### Added
- **`GameEvent<T>` generic base** — eliminates boiler-plate for typed SO events. `GameEventBool`, `GameEventInt` now derive from it.
- **`GameEventFloat`** and **`GameEventString`** — common typed event channels.
- **`IEquipmentService`** interface and **`EquipmentService`** implementation — manages equipment slots, integrates with InventoryService.
- **`ServiceLocator.TryGet<T>`** — non-throwing lookup alternative.
- **`Singleton<T>.HasInstance`** static property — checks for instance without triggering lazy creation.

### Changed
- `MechanicBase` abstract methods renamed: `OnInitialize` → `OnMechanicInitialize`, `OnEnable` → `OnMechanicEnable`, `OnDisable` → `OnMechanicDisable`, `OnUpdate` → `OnMechanicUpdate`, `OnCleanup` → `OnMechanicCleanup`.
- All three built-in mechanics (`SteeringMechanic`, `TapToJumpMechanic`, `SwipeMovementMechanic`) updated to match.

## [0.1.0] - 2026-02-18

### This is the first release of *\<Proto Casual\>*.

*Short description of this release*
