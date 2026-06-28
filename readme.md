<h1 align="center">🏂 SNOW BOARDER — Alpine Rush</h1>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.0-black?style=for-the-badge&logo=unity&logoColor=white" alt="Unity">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/Platform-Windows%20%7C%20macOS-lightgrey?style=for-the-badge" alt="Platform">
  <img src="https://img.shields.io/badge/Course-PRU213-blue?style=for-the-badge" alt="Course">
</p>

<p align="center">
  <i>A 2D physics-based arcade snowboarding game — FPT University Lab 2 submission.</i>
</p>

---

## 📖 Project Overview

**Snow Boarder** is a fast-paced 2D arcade sports game built with the Unity Engine. Players control a snowboarder navigating dangerous downhill slopes, collecting gems, performing aerial tricks, and reaching the finish line without crashing.

The game features:
- ⛷️ Realistic slope physics via `Rigidbody2D` + `SurfaceEffector2D`
- 🔄 360° aerial spin trick detection with combo multipliers
- ❄️ Dynamic camera-following snowfall particle system
- 🏆 Persistent top-5 leaderboard using `PlayerPrefs`
- 🎵 Layered audio: BGM + contextual SFX
- 🗺️ 2 playable levels with Sprite Shape terrain

---

## 🎮 Play Online

<p align="center">
  <a href="https://play.unity.com/en/games/3722c445-4a17-4d30-97fd-4df5f9dc8d1a/snow-boarder-alpine-rush">
    <img src="https://img.shields.io/badge/▶%20Play%20Now-Unity%20Play-blueviolet?style=for-the-badge&logo=unity&logoColor=white" alt="Play on Unity Play">
  </a>
</p>

---

## 🕹️ Controls

| Input | Action |
|:---|:---|
| `A` / `←` Left Arrow | Rotate left (counter-clockwise torque) |
| `D` / `→` Right Arrow | Rotate right (clockwise torque) |
| `W` / `↑` Up Arrow | Accelerate (boost speed) |
| `S` / `↓` Down Arrow | Decelerate (slow down) |
| `Space` | Jump (works on ground and rocks) |
| `Escape` | Pause / Resume |
| `Left Click` | UI interaction |

---

## 🏆 Scoring System

| Source | Points | Notes |
|:---|:---|:---|
| Collect PointGem | +10 per gem | Configurable per gem in Inspector |
| 360° Aerial Spin | +100 × Combo | Each full rotation while airborne |
| Combo Multiplier | x1 → xN | Increments per consecutive spin |
| Combo Reset | — | Resets to x1 upon any landing |

**Risk/Reward:** The higher your combo, the more points each spin earns — but landing resets everything!

---

## 🗺️ Levels

| Level | Scene | Description |
|:---|:---|:---|
| Main Menu | `Main Menu.unity` | Title screen with Play, Options, High Score, Quit |
| Level 1 | `Level1.unity` | Introductory slopes, moderate obstacle density |
| Level 2 | `Level2.unity` | Advanced terrain, more trees and rocks |

---

## 🧩 Architecture Overview

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs   ← Movement, jump, spin detection, HUD update
│   │   ├── CrashDetector.cs      ← Head-landing crash trigger (Ground / FallZone)
│   │   ├── FinishLine.cs         ← Level completion trigger
│   │   └── DustParticles.cs      ← Snow spray particle on ground contact
│   ├── Managers/
│   │   ├── GameManager.cs        ← Pause, GameOver, Win, snowfall, rock setup
│   │   ├── MainCamera.cs         ← Main menu logic + High Score display
│   │   ├── LevelManager.cs       ← Scene loading utility
│   │   ├── LeaderboardManager.cs ← High Score panel management
│   │   └── Point.cs              ← PointGem collectible trigger
│   └── Score/
│       ├── HighestScore.cs       ← Top-5 score persistence (PlayerPrefs)
│       └── HighScoreManager.cs   ← Score manager interface
├── Scenes/
│   ├── Main Menu.unity
│   ├── Level1.unity
│   └── Level2.unity
├── Prefabs/
│   ├── GameManager.prefab        ← Canvas + UI panels + GameManager script
│   ├── PointGem.prefab           ← Collectible gem with trigger + sound
│   ├── Finish Line.prefab        ← Flag + trigger + particle + sound
│   ├── Snow Generator.prefab     ← Static snowfall (replaced by camera-follow at runtime)
│   ├── LeaderboardManager.prefab ← High score display panel
│   └── Canvas.prefab             ← HUD (Score, Speed, Distance, Combo)
├── Art/
│   └── Imange/
│       ├── Boarder_Top.png       ← Player upper body sprite
│       ├── Boarder_Bottom.png    ← Player board/lower body sprite
│       ├── Snow-Tree-1/2.png     ← Obstacle trees
│       ├── Snow-Rock.png         ← Obstacle rocks
│       ├── Bac.png               ← Level background
│       └── Snow-tile-low-res.png ← Terrain tile texture
└── Audio/
    ├── christmas-snow-176839.mp3           ← Main menu BGM
    ├── the_mountain-8-bit-retro-522443.mp3 ← Gameplay BGM
    ├── Crash SFX.ogg                       ← Player wipeout sound
    ├── Finish SFX.ogg                      ← Level complete sound
    └── diamond-found-190255.mp3            ← Gem collect sound
```

---

## ⚙️ Key Scripts Explained

### `PlayerController.cs`
Core player logic attached to **Barry** (root GameObject).

```csharp
// Speed control via SurfaceEffector2D
se.speed = boostSpeed;   // W / Up
se.speed = slowSpeed;    // S / Down
se.speed = baseSpeed;    // default

// Rotation via torque (A/D)
rb.AddTorque(TorqueAmount);   // left
rb.AddTorque(-TorqueAmount);  // right

// Jump (Space)
rb.linearVelocity = new Vector2(horizontalSpeed, jumpForce);

// Spin detection — awards 100 × comboMultiplier per 360°
if (totalRotation >= 290f) { comboMultiplier++; score += 100 * comboMultiplier; }
```

**Inspector-configurable fields:**

| Field | Default Use |
|:---|:---|
| `TorqueAmount` | Rotation force magnitude |
| `boostSpeed` | Speed when W/Up held |
| `baseSpeed` | Default downhill speed |
| `slowSpeed` | Speed when S/Down held |
| `jumpForce` | Vertical force on Space |

---

### `CrashDetector.cs`
Attached to player head area. Triggers game over when:
- Head enters a **Ground**-tagged trigger (landed upside-down)
- Player enters a **FallZone** trigger (fell off the map)

Rock contacts are handled by physics only (bouncy `PhysicsMaterial2D`) — no scripted crash.

---

### `GameManager.cs`
Manages runtime systems:
- **Pause** (Escape) — freezes `Time.timeScale`
- **Game Over / Win panels** — shows score, saves high score
- **Dynamic snowfall** — creates camera-following `ParticleSystem` at runtime in `LateUpdate()`
- **Rock obstacles** — configures `BoxCollider2D` + bouncy `PhysicsMaterial2D` on all Rock-named sprites

---

## 🔧 Setup & Running

### Requirements
- Unity **6000.0.x** (Unity 6) or later
- Universal Render Pipeline (URP) — included in project
- TextMeshPro — included in project

### Open in Unity
```
1. Clone or unzip the project
2. Open Unity Hub → Add Project → select Snow-Boarder-lab2/ folder
3. Unity will import all assets automatically
4. Open Scenes/Main Menu.unity
5. Press ▶ Play
```

### Build Settings (required scene order)
```
File → Build Settings → Scenes In Build:
  [0] Scenes/Main Menu
  [1] Scenes/Level1
  [2] Scenes/Level2
```

---

## 🐛 Known Issues & Notes

| Issue | Cause | Fix |
|:---|:---|:---|
| Finish Line doesn't trigger | Barry missing `Player` tag | Select Barry → Inspector → Tag → `Player` |
| Rocks pass through player | `BoxCollider2D` not initialized | `GameManager.Start()` calls `ConfigureRockObstacles()` at runtime |
| Snowfall stops mid-level | Static emitter out of range | Camera-following system in `GameManager` replaces it automatically |
| Leaderboard shows 0s | `LoadHighScores()` needs call | Click High Score button — scores load on `ShowHighScores()` |

---

## 👨‍💻 Author

| Field | Info |
|:---|:---|
| **Name** | Nguyễn Công Nhất |
| **Student ID** | QE190017 |
| **Class** | SE19B.NET — PRU213 |
| **Institution** | FPT University |

---

*Snow Boarder © 2025 — Nguyễn Công Nhất | QE190017*
"# SnowBoarder_NguyenCongNhat_SE19BNET_Lab01"
"# SnowBoarder_NguyenCongNhat_SE19BNET-" 
"# SnowBoarder_NguyenCongNhat_SE19BNET-" 
"# SnowBoarder_NguyenCongNhat_SE19BNET-" 
