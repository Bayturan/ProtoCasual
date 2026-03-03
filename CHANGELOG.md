# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2026-03-02

### Breaking — uGUI → UI Toolkit Migration
The entire UI layer has been rewritten from **Unity uGUI** (Canvas / MonoBehaviour / TMP) to **UI Toolkit** (UIDocument / UXML / USS / plain-C# controllers).
Re-run the **Setup Wizard** after updating — old scene references are no longer valid.

### Removed
- **UIManager** (MonoBehaviour singleton) — replaced by `UIToolkitManager`.
- **UIScreen** (MonoBehaviour base) — replaced by `ScreenController` (plain C#).
- **PopupBase** (MonoBehaviour base) — replaced by `PopupController` (plain C#).
- **PopupManager** (MonoBehaviour singleton) — replaced by a plain-C# `PopupManager` managed by `UIToolkitManager`.
- **MenuScreen** — replaced by `MainScreen`.
- All 14 legacy uGUI files in `Runtime/UI/`.

### Added
- **`UIToolkitManager`** — Singleton MonoBehaviour with `UIDocument`. Manages all screen and popup lifecycle, auto-subscribes to `GameManager.OnStateChanged`.
- **`ScreenController`** — Abstract plain-C# base for screens. Element queries via `Q<T>()`, `Btn()`, `Lbl()`, `Sld()`, `Tgl()`, `PBar()`, `SView()` helpers.
- **`PopupController`** — Abstract plain-C# base for popups with `Show(data)` / `Hide()`.
- **`PopupManager`** — Plain-C# class (not a singleton) with push/pop stack, `Show<T>()`, `HideTop()`, `HideAll()`.
- **`ThemeManager`** — Runtime USS theme swapping via `ApplyTheme(StyleSheet)`.
- **8 pre-made screen controllers** — `MainScreen`, `GameplayScreen`, `WinScreen`, `LoseScreen`, `PauseScreen`, `StoreScreen`, `InventoryScreen`, `SettingsScreen`. All buttons wired, all services integrated.
- **2 pre-made popup controllers** — `ConfirmPopup` (title / message / confirm / cancel) and `RewardPopup` (dynamic reward list).
- **10 UXML templates** — Full layouts for each screen and popup, every interactive element has a `name` attribute for controller queries.
- **7 USS stylesheets**:
  - `ProtoCasual-Base.uss` — Layout, typography, buttons, HUD, currency bar, overlays, progress bar.
  - `ProtoCasual-Components.uss` — Settings groups, shop/inventory cards, detail panels, pop-up overlays, reward list.
  - **5 game-type themes**: `HyperCasual.uss` (bright / playful), `Puzzle.uss` (light / minimal), `Racing.uss` (dark / neon), `Endless.uss` (retro / gradient), `Hybrid.uss` (professional / balanced). Each overrides CSS custom properties (`--color-primary`, `--color-bg`, `--border-radius`, etc.).
- **`PanelSettings`** auto-creation — `ScaleWithScreenSize`, 1080×1920 reference, 0.5 match. Created at `Assets/_Game/UI/GamePanelSettings.asset`.
- **Theme copy to project** — Setup Wizard copies the selected theme USS to `Assets/_Game/UI/Themes/GameTheme.uss` for customization. Base + component styles stay in the package for update propagation.
- **`Assets/_Game/UI/Themes/`** folder added to project structure generator.

### Changed
- **`SceneBuilder`** — Rewritten: creates `UIDocument` + `UIToolkitManager` instead of Canvas + UIManager. Wires all UXML/USS from the package. Prefers project theme copy, falls back to package theme.
- **`UIPrefabGenerator`** — Repurposed: copies theme USS to project instead of generating uGUI prefabs.
- **`FrameworkSetup`** — "UI Prefabs" counter → "UI Assets" (counts UXML + USS files). Updated completion dialog.
- **`ProjectStructureGenerator`** — Adds `UI/Themes` folder.
- **`GameBootstrap`** — References `UIToolkitManager.Instance` instead of `UIManager.Instance`.
- **`IUIScreen`** — Simplified: `Initialize()`/`Show()`/`Hide()` → `OnShow()`/`OnHide()`.
- **Editor asmdef** — Removed `Unity.TextMeshPro` reference (no longer needed for UI generation).
- **`package.json`** description updated for UI Toolkit.

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
