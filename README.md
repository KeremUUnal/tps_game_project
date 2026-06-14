# Pirate's Passage — Time Travel TPS Game

A third-person shooter prototype built in Unity, featuring a pirate character who travels between past and future to uncover a hidden treasure and a mysterious gateway.

> Developed as an academic project (March – June 2026)

---

## About

Pirate's Passage is a story-driven TPS where players control a time-traveling pirate navigating two distinct eras. The game blends combat, exploration, and puzzle-solving as the player collects keys, gathers gold, and fights enemies to reach the final portal.

## Gameplay Features

- **Third-Person Shooter Combat** — Aim, shoot, and take down enemies with raycast-based gunplay, muzzle flash effects, and hit feedback
- **Weapon System** — Draw/holster weapon (Q), aim with FOV zoom (Right Click), and fire (Left Click)
- **Enemy AI** — NavMesh-based enemy patrolling and attacking with health systems
- **Inventory & Collectibles** — Gold coin pickups, key collection for locked doors, and health pickups
- **UI System** — Health bar (HUD), crosshair, inventory display, pause menu, win screen, and main menu with animated buttons
- **Portal & Level Progression** — Portal system for scene transitions and a game finish sequence
- **Audio** — Background music, gunshot SFX, coin pickup sounds, punch impacts, and ambient wind
- **Graphics Settings** — In-game graphics quality manager for different hardware profiles

## Scenes

| Scene | Description |
|-------|-------------|
| `MainMenu` | Animated main menu with stylized buttons and settings |
| `Level_Past` | Main gameplay level set in the past era |

## Project Structure

```
Assets/
├── Animations/          # Character & enemy animation clips (.fbx)
├── Audio/               # Sound effects and music
├── Materials/           # Custom materials (sand, future theme)
├── Prefabs/             # Reusable game objects
├── Scenes/              # Game scenes (MainMenu, Level_Past)
├── Scripts/
│   ├── Player/          # WeaponController, CombatSystem, PlayerDeath
│   ├── Enemy/           # EnemyAI, HealthSystem
│   ├── Systems/         # Portal, GoldManager, KeyPickup, DoorController, GameFinish, GraphicsManager
│   └── UI/              # PlayerHUD, Crosshair, PauseMenu, MainMenuManager, WinScreen, HealthPickup
├── Settings/            # URP render pipeline assets
├── Starter Assets/      # Unity Starter Assets (TPS controller, input system)
└── UI/                  # UI sprites and icons
```

## Scripts Overview

### Player
| Script | Role |
|--------|------|
| `WeaponController.cs` | Weapon draw/holster, aim mode with FOV zoom, raycast shooting with bullet trails |
| `CombatSystem.cs` | Combat logic and damage dealing |
| `PlayerDeath.cs` | Player death handling |
| `IDamageable.cs` | Damage interface implemented by damageable entities |

### Enemy
| Script | Role |
|--------|------|
| `EnemyAI.cs` | NavMesh-based enemy behavior (patrol, chase, attack) |
| `HealthSystem.cs` | Enemy health, damage reception, and death |

### Systems
| Script | Role |
|--------|------|
| `Portal.cs` | Teleportation / scene transition system |
| `GoldManager.cs` | Tracks collected gold across the game |
| `GoldPickup.cs` | Individual gold coin pickup logic |
| `KeyPickup.cs` | Key collection for unlocking doors |
| `DoorController.cs` | Door open/close logic triggered by keys |
| `GameFinish.cs` | Win condition detection and game completion flow |
| `GraphicsManager.cs` | Runtime graphics quality settings |

### UI
| Script | Role |
|--------|------|
| `PlayerHUD.cs` | Health bar, gold counter, key inventory display |
| `CrosshairController.cs` | Dynamic crosshair rendering |
| `PauseMenu.cs` | Pause/resume functionality |
| `MainMenuManager.cs` | Main menu navigation and scene loading |
| `MenuAnimator.cs` | Menu transition animations |
| `MenuButtonEffect.cs` | Button hover/click visual effects |
| `MenuStyleApplier.cs` | Consistent UI styling across menus |
| `WinScreenAnimator.cs` | Victory screen animations |
| `HealthPickup.cs` | Health restoration pickup logic |
| `AutoHideText.cs` | Auto-fading UI text for notifications |

## Tech Stack

- **Engine:** Unity 6 (6000.0.68f1) with Universal Render Pipeline (URP)
- **Language:** C#
- **Input:** Unity New Input System
- **Camera:** Cinemachine (Unity.Cinemachine namespace)
- **Navigation:** Unity NavMesh (enemy AI pathfinding)
- **Base Controller:** Unity Starter Assets — Third Person Character Controller

## Asset Store Packages Used

The following packages are required but excluded from the repository due to file size. Import them via Unity Package Manager or Asset Store after cloning:

- **Pirate Customized** — Pirate character model and textures
- **Basic Bandit** — Enemy character model
- **Stylized Pirate Ship** — Environment prop
- **Island** — Environment terrain and props
- **4K Tiled Ground Textures p1** — Ground textures
- **Rust Key** — Key model for collectibles
- **SciFi Office Lite** — Future-era environment assets
- **Sci-Fi Styled Modular Pack** — Future-era modular building pieces
- **LiquidFire Package 4** — VFX effects
- **Sprite Muzzle Flashes** — Gun muzzle flash sprites
- **SlimUI** — UI framework
- **TextMesh Pro** — Text rendering (usually included with Unity)

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/KeremUUnal/tps_game_project.git
   ```

2. Open the project in **Unity 6 (6000.0.68f1)** or later

3. Import the required Asset Store packages listed above

4. Open `Assets/Scenes/MainMenu.unity` or `Assets/Scenes/Level_Past.unity`

5. Press **Play**

## Controls

| Input | Action |
|-------|--------|
| `WASD` | Move |
| `Mouse` | Look around |
| `Space` | Jump |
| `Left Shift` | Sprint |
| `Q` | Draw / Holster weapon |
| `Right Click` | Aim (zoom) |
| `Left Click` | Shoot |
| `Escape` | Pause menu |

## License

This project is developed for academic purposes.

---

*Built with Unity 6 & URP*
