# BIHAR SURFERS - 3D Endless Railway Runner

**Subtitle:** Run Across Bihar  
**Package Name:** `com.biharsurfers.game`  
**Orientation:** Portrait  

---

## 🎮 1. GAME CONCEPT & OVERVIEW
**Bihar Surfers** is an original 3D endless-runner adventure game set in Bihar, India. The player controls a young Bihari runner navigating through iconic railway stations, platforms, roads, bridges, and markets inspired by Bihar's rich heritage and culture.

### Main Gameplay Loop:
`RUN → CHANGE LANES → JUMP → SLIDE → COLLECT COINS → AVOID OBSTACLES → USE POWER-UPS → INCREASE SPEED → COMPLETE MISSIONS → UNLOCK LOCATIONS/CHARACTERS → ACHIEVE HIGH SCORE`

---

## 🏛️ 2. BIHAR LOCATIONS (10 Initial Districts)
1. **Patna Junction (`PNBE`)** - Capital city with grand railway architecture & busy platforms.
2. **Ara Junction (`ARA`)** - Bhojpuri heartland with bustling town railway junctions.
3. **Buxar (`BXR`)** - Ganga riverfront region with serene green palm sceneries.
4. **Sasaram Junction (`SSM`)** - Historic fort city with stone architecture and rocky hills.
5. **Chhapra Junction (`CPR`)** - Major western rail junction with multi-platform bridges.
6. **Gaya Junction (`GAYA`)** - Ancient spiritual hub with temple-inspired background graphics.
7. **Muzaffarpur Junction (`MFP`)** - Litchi city with vibrant bazaar decor & heavy train traffic.
8. **Darbhanga Junction (`DBG`)** - Cultural capital of Mithila with royal palace gates & art.
9. **Madhubani (`MBI`)** - Famous Madhubani wall paintings on station structures.
10. **Bhagalpur Junction (`BGP`)** - Silk city with colorful markets and bustling platforms.

---

## 🛠️ 3. PROJECT ARCHITECTURE & TECH STACK

The repository contains dual engine implementations:

### A. Unity + C# Production Engine (`/UnityProject`)
- **Engine Version:** Unity 2022.3 LTS / 2023+ (URP recommended)
- **Target Platform:** Android (Portrait mode) & Windows PC for testing
- **Package ID:** `com.biharsurfers.game`
- **UI System:** TextMeshPro + Unity UI
- **Data Persistence:** JSON Local Save System (`biharsurfers_save.json`)

#### Unity C# Script Structure:
- `Assets/Scripts/Player/PlayerController.cs` - Master lifecycle, revive, health, and state manager.
- `Assets/Scripts/Player/PlayerMovement.cs` - 3-Lane smooth lerping, jump physics, sliding, and dynamic speed scaling.
- `Assets/Scripts/Player/PlayerInput.cs` - Touch swipe detection (Left, Right, Up, Down) + PC WASD/Arrow key testing.
- `Assets/Scripts/Player/PlayerAnimationController.cs` - Animator wrapper for Idle, Run, Jump, Slide, Hit, and Death.
- `Assets/Scripts/Player/PlayerCollision.cs` - Physical trigger and collision dispatcher.
- `Assets/Scripts/Track/TrackManager.cs` - Procedural endless track generator and segment recycling.
- `Assets/Scripts/Track/TrackSegment.cs` - Modular segment configuration & entity spawn points.
- `Assets/Scripts/Track/TrackSpawner.cs` - Procedural backdrop & track spawner.
- `Assets/Scripts/Track/ObjectPool.cs` - High-performance object pooling system to prevent GC spikes on mobile devices.
- `Assets/Scripts/Obstacles/Obstacle.cs` - Base static/moving obstacle logic.
- `Assets/Scripts/Obstacles/MovingObstacle.cs` - Moving train trolley and traffic barricade behavior.
- `Assets/Scripts/Obstacles/ObstacleSpawner.cs` - Difficulty-scaled obstacle generator.
- `Assets/Scripts/Coins/Coin.cs` - Collectible Rupee coin rotation, magnet attraction, and pickup effects.
- `Assets/Scripts/Coins/CoinSpawner.cs` - Coin pattern generator (Line, Arc, Zig-Zag, Lane Shift).
- `Assets/Scripts/Coins/CoinManager.cs` - Session coin counter & combo multiplier decay logic.
- `Assets/Scripts/PowerUps/PowerUp.cs` - Base powerup class.
- `Assets/Scripts/PowerUps/MagnetPowerUp.cs` - Chai Magnet (coin attraction).
- `Assets/Scripts/PowerUps/ShieldPowerUp.cs` - Litti Shield (collision protection).
- `Assets/Scripts/PowerUps/SpeedBoostPowerUp.cs` - Sattu Rocket / Bihar Boost (speed boost & invincibility).
- `Assets/Scripts/PowerUps/CoinMultiplierPowerUp.cs` - Doubles collected coins.
- `Assets/Scripts/Core/ScoreManager.cs` - Distance, score, and local high score manager.
- `Assets/Scripts/Core/DifficultyManager.cs` - Dynamic difficulty scaling (Easy, Normal, Hard, Extreme).
- `Assets/Scripts/Core/MissionManager.cs` - Missions, progress tracking, and coin rewards.
- `Assets/Scripts/Core/AchievementManager.cs` - Unlockable achievement badges.
- `Assets/Scripts/Core/SaveManager.cs` - Corrupt-safe JSON local save system.
- `Assets/Scripts/Core/GameManager.cs` - High-level game state machine.
- `Assets/Scripts/Core/AudioManager.cs` - Music and sound effects manager with mute toggles.
- `Assets/Scripts/Core/LocalizationManager.cs` - English and Hindi localization support.
- `Assets/Scripts/Core/DebugManager.cs` - Developer cheat menu (Add coins, set speed, invincibility).
- `Assets/Scripts/Camera/RunnerCamera.cs` - Third-person follow camera with speed FOV scaling and camera shake.
- `Assets/Scripts/Environment/LocationManager.cs` - District location progression.
- `Assets/Scripts/Environment/StationTransitionManager.cs` - Station banner transition overlay.
- `Assets/Scripts/UI/UIManager.cs` - HUD, Main Menu, Pause Menu, Game Over Screen, and Shop UI.
- `Assets/Scripts/Shop/ShopManager.cs` - Virtual coin shop for character and skin unlocks.
- `Assets/Scripts/Data/LocationData.cs` - ScriptableObject for district location data.
- `Assets/Scripts/Data/CharacterData.cs` - ScriptableObject for character statistics.

---

### B. Playable 3D Web Engine (`/src`)
- **Engine:** Three.js + Vite
- **Feature Highlights:**
  - Procedural 3D railway track segments & station platforms.
  - Fully sculpted Bihari player character with skeletal running, jumping, and sliding animations.
  - 38 playable Bihar district locations with custom station backdrops.
  - Interactive touch D-pad & action buttons + Keyboard controls (Arrow keys / WASD).
  - Sound synthesis engine for footsteps, jumps, slides, coin pickups, power-ups, and train horns.
  - LocalStorage persistence for coins, high scores, unlocked characters, and missions.

---

## 📱 4. HOW TO BUILD FOR ANDROID IN UNITY

1. Open **Unity Hub** and click **Add Project from Disk**. Select the `UnityProject` folder.
2. Go to **File > Build Settings**:
   - Platform: Select **Android** and click **Switch Platform**.
   - Architecture: ARM64 & ARMv7.
3. Open **Edit > Project Settings > Player**:
   - **Company Name:** BiharSurfers
   - **Product Name:** Bihar Surfers
   - **Package Name:** `com.biharsurfers.game`
   - **Default Orientation:** Portrait
   - **Target API Level:** Android 14 (API 34) or latest installed SDK.
4. Click **Build and Run** or **Build APK**.

---

## 🚀 5. HOW TO RUN THE WEB VERSION LOCALLY

1. Make sure Node.js is installed.
2. Open terminal in project root:
   ```bash
   npm install
   npm run dev
   ```
3. Open the printed localhost URL (e.g., `http://localhost:5173`) in your browser.

---

## 📖 6. EXTENSION & DEVELOPER GUIDES

### How to Add a New Bihar Location
1. Create a new `LocationData` ScriptableObject in `Assets/Settings/Locations/` (or via `Create > BiharSurfers > Location Data`).
2. Assign the station name, station code, district name, Hindi banner text, and skybox/fog colors.
3. Add the ScriptableObject to the `locationList` array on the `LocationManager` component attached to your scene's `Environment` manager object.

### How to Add a New Character
1. Create a new `CharacterData` ScriptableObject via `Create > BiharSurfers > Character Data`.
2. Define the character ID (e.g. `"Ravi"`), display name, coin unlock cost, and character 3D prefab.
3. Register the new character in `ShopManager.availableCharacters`.

### How to Add a New Obstacle
1. Create your 3D model prefab in Unity with a `BoxCollider` or `CapsuleCollider`.
2. Attach the `Obstacle.cs` or `MovingObstacle.cs` script to the prefab. Set the tag to `"Obstacle"`.
3. Add the prefab key to the `ObjectPool` component's pool list and register the key inside `ObstacleSpawner.cs`.

### How to Modify Difficulty Settings
1. Open `DifficultyManager.cs` in `Assets/Scripts/Core/`.
2. Adjust the distance thresholds (`3000f`, `1500f`, `600f`) or tweak `currentDifficultyMultiplier` speed and density scaling factors.

### How to Add New Gameplay Missions
1. Open `MissionManager.cs` in `Assets/Scripts/Core/`.
2. In `InitializeDefaultMissions()`, add a new `Mission` object specifying `id`, `title`, `description`, `targetAmount`, and `coinReward`.
3. Call `MissionManager.Instance.UpdateProgress("mission_id", amount)` at appropriate gameplay triggers.

