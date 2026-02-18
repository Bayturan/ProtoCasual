# Proto Casual - Game Factory Framework

A reusable Unity framework for building casual mobile games from a single modular system.

## Features
- **Genre-agnostic**: HyperCasual, Puzzle, Racing, Endless, Hybrid
- **GameMode-driven**: Plug-in architecture for game modes and mechanics
- **ScriptableObject-based**: Data-driven configuration
- **Monetization-ready**: Ads and IAP service interfaces
- **Single scene architecture**: One scene, state-machine driven
- **UPM compatible**: Proper package structure with assembly definitions

## Quick Start
1. Install the package via Unity Package Manager
2. Go to **ProtoCasual > Setup Scene** in the menu bar
3. Configure `GameConfig` ScriptableObject and assign to Bootstrap
4. Add your GameMode components
5. Assign UI Screen references

## Architecture
```
Runtime/
  Bootstrap/     - Entry point & ServiceLocator
  Events/        - ScriptableObject event system
  GameLoop/      - GameModeBase, GameState
  Interfaces/    - All contracts (IAdsService, IGameMode, etc.)
  Managers/      - GameManager, AudioManager, Economy, Inventory, Equipment, Level, Save
  ScriptableObjects/ - Config assets (GameConfig, BotConfig, MapConfig, etc.)
  Systems/       - Input, MapGenerator, MechanicBase, Mechanics
  UI/            - UIManager, UIScreen, Screens
  Utilities/     - Singleton, ObjectPool, Timer, Extensions
```

## Requirements
- Unity 6000.3+
- TextMeshPro