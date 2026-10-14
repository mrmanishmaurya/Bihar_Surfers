# Bihar Surfers 3D (बिहार सर्फर्स)

[![Unity 6.3 LTS](https://img.shields.io/badge/Unity-6000.3.25f1_LTS-blue.svg)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-Android_%7C_Windows-green.svg)](https://developer.android.com/)
[![License](https://img.shields.io/badge/License-Proprietary-red.svg)]()
[![Developer](https://img.shields.io/badge/Developer-Manish_Kumar-orange.svg)]()

> **Bihar Surfers 3D** is a high-octane 3D endless runner game built in **Unity 6 LTS** and C#, celebrating the vibrant cultural heritage, railway junctions, and all **38 districts of Bihar**. Inspired by Subway Surfers, players sprint across authentic 3D tracks, leap over obstacles, slide beneath barricades, collect gold Rupee coins, harness traditional Bihari power-ups, and unlock 5 unique characters!

---

## 🎮 Game Overview & Features

### 1. 3D Endless Runner Mechanics
- **3-Lane Running System:** Fluid switching between left, center, and right tracks.
- **Dynamic Controls:** Full touch swipe support for mobile (Left, Right, Jump, Slide) and keyboard bindings for PC testing.
- **Speed & Difficulty Progression:** Running speed dynamically accelerates as distance increases, challenging player reflexes.
- **Object Pooling:** High-performance pool for tracks, coins, and obstacles ensuring consistent 60+ FPS without garbage collection spikes.
- **Procedural Sound Engine:** In-game synthesized audio generation for coin pick-ups, swooshes, slides, collisions, and district transition fanfares.

### 2. Bihar All-38-District Progression System
Run across all 38 districts of Bihar with **1,000-meter milestones** between each district. As you cross station arches, district banners appear with bilingual English & Hindi station codes, custom atmospheric fog, and landmark scenery.

| # | District | Station Code | Hindi Name | Milestone Distance |
|---|----------|--------------|------------|-------------------|
| 1 | **Patna** | PNBE | पटना जंक्शन | Starting District (0m) |
| 2 | **Nalanda** | NLD | नालंदा | 1,000 m |
| 3 | **Gaya** | GAYA | गया जंक्शन | 2,000 m |
| 4 | **Nawada** | NWD | नवादा | 3,000 m |
| 5 | **Aurangabad** | AUBR | औरंगाबाद (अनुग्रह नारायण रोड) | 4,000 m |
| 6 | **Jehanabad** | JHD | जहानाबाद | 5,000 m |
| 7 | **Arwal** | ARWL | अरवल | 6,000 m |
| 8 | **Rohtas** | SSM | रोहतास (सासाराम) | 7,000 m |
| 9 | **Kaimur** | BBU | कैमूर (भभुआ रोड) | 8,000 m |
| 10 | **Buxar** | BXR | बक्सर | 9,000 m |
| 11 | **Bhojpur** | ARA | भोजपुर (आरा जंक्शन) | 10,000 m |
| 12 | **Saran** | CPR | सारण (छपरा जंक्शन) | 11,000 m |
| 13 | **Siwan** | SV | सीवान जंक्शन | 12,000 m |
| 14 | **Gopalganj** | THE | गोपालगंज (थावे) | 13,000 m |
| 15 | **Vaishali** | HJP | वैशाली (हाजीपुर) | 14,000 m |
| 16 | **Muzaffarpur** | MFP | मुजफ्फरपुर जंक्शन | 15,000 m |
| 17 | **Sheohar** | SHHR | शिवहर | 16,000 m |
| 18 | **Sitamarhi** | SMI | सीतामढ़ी जंक्शन | 17,000 m |
| 19 | **East Champaran** | MKI | पूर्वी चंपारण (मोतिहारी) | 18,000 m |
| 20 | **West Champaran** | BTH | पश्चिमी चंपारण (बेतिया) | 19,000 m |
| 21 | **Darbhanga** | DBG | दरभंगा जंक्शन | 20,000 m |
| 22 | **Madhubani** | MBI | मधुबनी | 21,000 m |
| 23 | **Samastipur** | SPJ | समस्तीपुर जंक्शन | 22,000 m |
| 24 | **Begusarai** | BGS | बेगूसराय | 23,000 m |
| 25 | **Munger** | JMP | मुंगेर (जमालपुर) | 24,000 m |
| 26 | **Lakhisarai** | LKR | लखीसराय जंक्शन | 25,000 m |
| 27 | **Sheikhpura** | SHK | शेखपुरा | 26,000 m |
| 28 | **Jamui** | JMU | जमुई | 27,000 m |
| 29 | **Khagaria** | KGG | खगड़िया जंक्शन | 28,000 m |
| 30 | **Bhagalpur** | BGP | भागलपुर जंक्शन | 29,000 m |
| 31 | **Banka** | BAKA | बांका | 30,000 m |
| 32 | **Saharsa** | SHC | सहरसा जंक्शन | 31,000 m |
| 33 | **Supaul** | SOU | सुपौल | 32,000 m |
| 34 | **Madhepura** | DMH | मधेपुरा | 33,000 m |
| 35 | **Purnia** | PRNA | पूर्णिया जंक्शन | 34,000 m |
| 36 | **Kishanganj** | KNE | किशनगंज | 35,000 m |
| 37 | **Araria** | ARQ | अररिया | 36,000 m |
| 38 | **Katihar** | KIR | कटिहार जंक्शन | 37,000 m |

### 3. Playable Characters & Unlock Roster
Choose from 5 distinct characters, each rendered in-game with custom Bihari outfits, gamchas, and colorways:
- **Aman (Default - Free):** Classic Bihari runner with traditional red kurta, blue jeans, and sneakers.
- **Ravi (1,500 Coins):** Royal saffron kurta and traditional tricolor gamcha scarf.
- **Sonu (3,000 Coins):** Electric green athletic jersey with sports headband.
- **Vikash (5,000 Coins):** Deep crimson modern hoodie with urban streetwear styling.
- **Arjun (8,000 Coins):** Golden festive sherwani with silk dupatta fit for a champion.

### 4. Cultural Power-Ups
- **Chai Magnet (☕):** Magnetic field automatically draws all gold Rupee coins across all 3 lanes.
- **Litti Shield (🛡️):** Grants invulnerability against 1 obstacle collision.
- **Bihar Boost / Sattu Rocket (🚀):** High-speed invincibility blast soaring above tracks.
- **2x Coin Multiplier (🪙):** Doubles all collected coins for a limited duration.

---

## 🕹️ Controls Guide

### Mobile (Android Touch)
- **Swipe Left:** Shift to Left Lane
- **Swipe Right:** Shift to Right Lane
- **Swipe Up:** Jump over barriers and obstacles
- **Swipe Down:** Slide beneath elevated signals and warning bars

### PC Keyboard (Testing / Standalone)
- **A / Left Arrow:** Move Left
- **D / Right Arrow:** Move Right
- **W / Up Arrow / Space:** Jump
- **S / Down Arrow:** Slide
- **Escape / P:** Pause / Resume

---

## 🛠️ Project Structure

```
Bihar Surfers/
├── Assets/
│   ├── Editor/
│   │   └── AndroidBuildScript.cs      # Automated Android APK & Windows Standalone build pipeline
│   ├── Resources/
│   │   └── Stations/                  # 38 District station artwork & billboard textures
│   ├── Scenes/
│   │   ├── Boot.unity                 # Bootstrap initialization & save loading
│   │   ├── MainMenu.unity             # Title screen, Character Select, District Browser, Settings
│   │   ├── Game.unity                 # Core 3D runner gameplay loop
│   │   └── GameOver.unity             # High score review & restart screen
│   ├── Scripts/
│   │   ├── Camera/RunnerCamera.cs     # Smooth 3D follow camera
│   │   ├── Coins/                     # Coin spawning, rotation, and magnet pickup
│   │   ├── Core/                      # GameManager, AudioManager, SaveManager, ScoreManager
│   │   ├── Environment/               # LocationManager (38 districts), station transitions
│   │   ├── Obstacles/                 # Static and moving barricades, trains
│   │   ├── Player/                    # PlayerController, CharacterManager, input handlers
│   │   ├── PowerUps/                  # Magnet, Shield, SpeedBoost, Multiplier
│   │   ├── Track/                     # Procedural track spawner & ObjectPool
│   │   └── UI/                        # Dynamic responsive mobile UI Canvas & Menus
│   └── Settings/                      # URP pipeline & graphics profiles
├── Builds/
│   ├── Android/
│   │   └── BiharSurfers.apk           # Installable Android APK
│   └── Windows/
│       └── BiharSurfers.exe           # Validated Standalone Windows executable
└── ProjectSettings/                   # Android PlayerSettings (Package: com.manishkumar.biharsurfers)
```

---

## 📱 Building & Installing the Android APK

### Prerequisites
- **Unity 6 (6000.3.25f1 LTS)** with Android Build Support module
- **Android SDK:** API Level 26 (Android 8.0 Oreo) minimum, Target API Level 34 (Android 14)
- **JDK:** OpenJDK 17 (bundled with Android Studio)
- **Target Architecture:** ARM64 & ARMv7

### One-Click Build via Unity Editor
1. Open the project in Unity Editor.
2. Click the top menu: **Bihar Surfers > Build Android APK**.
3. The build output will be generated at:
   ```
   Builds/Android/BiharSurfers.apk
   ```

### Command Line / Batchmode Build
Run the following PowerShell command in the project directory:
```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Unity.exe" `
  -batchmode -quit `
  -projectPath . `
  -executeMethod BiharSurfers.Editor.AndroidBuildScript.BuildAndroidAPK `
  -logFile "Logs/build_android.log"
```

### Installing on Android Device
1. Enable **Developer Options** and **USB Debugging** on your phone.
2. Connect your device via USB and install using ADB:
   ```bash
   adb install -r Builds/Android/BiharSurfers.apk
   ```
3. Alternatively, copy `BiharSurfers.apk` directly to phone storage, tap the APK in your file manager, and select **Install**.

---

## 👤 Credits & Author

- **Game Developer & Designer:** Manish Kumar
- **GitHub:** [@mrmanishmaurya](https://github.com/mrmanishmaurya)
- **Repository:** [https://github.com/mrmanishmaurya/Bihar_Surfers.git](https://github.com/mrmanishmaurya/Bihar_Surfers.git)
