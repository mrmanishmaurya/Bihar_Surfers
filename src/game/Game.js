// Game.js - Core Endless Runner Game Engine for Bihar Surfers
import { SaveManager } from './SaveManager.js';
import { JourneyManager } from './JourneyManager.js';
import { AudioManager } from './AudioManager.js';
import { SceneManager } from './SceneManager.js';
import { AssetFactory } from './AssetFactory.js';
import { CharacterManager } from './CharacterManager.js';
import { TrackManager } from './TrackManager.js';
import { InputManager } from './InputManager.js';
import { UIManager } from '../ui/UIManager.js';

export class Game {
  constructor(gameContainer, uiContainer) {
    this.gameContainer = gameContainer;
    this.uiContainer = uiContainer;

    // Load save data
    this.saveData = SaveManager.load();

    // Initialize Subsystems
    this.sceneManager = new SceneManager(this.gameContainer);
    this.assetFactory = new AssetFactory();
    this.journeyManager = new JourneyManager(this.saveData);
    this.audioManager = new AudioManager(this.saveData);
    this.characterManager = new CharacterManager(this.sceneManager.scene, this.assetFactory);
    this.trackManager = new TrackManager(this.sceneManager.scene, this.assetFactory, this.journeyManager);

    this.uiManager = new UIManager(this.uiContainer, this);
    this.inputManager = new InputManager((action) => this.handleInput(action));

    // Game state
    this.state = 'PLAYING';
    this.score = 0;
    this.sessionCoins = 0;
    this.sessionJumps = 0;
    this.sessionPowerUps = 0;
    this.lives = 3;
    this.hasRevived = false;

    this.baseSpeed = 16;
    this.currentSpeed = 16;

    this.comboCount = 0;
    this.comboTimer = 0;
    this.footstepTimer = 0;

    this.activePowerUp = null;
    this.isShielded = false;

    this.lastFrameTime = performance.now();

    // Build Patna Junction first level 3D scene & start running immediately!
    this.journeyManager.selectDistrict(0); // 0 = Patna Junction
    this.characterManager.buildCharacter(this.saveData.selectedCharacter);
    this.startRun();

    // Start render loop
    requestAnimationFrame((t) => this.loop(t));
  }

  startRun() {
    this.saveProgress();
    const currentDist = this.journeyManager.getCurrentDistrict();

    this.score = 0;
    this.sessionCoins = 0;
    this.sessionJumps = 0;
    this.sessionPowerUps = 0;
    this.lives = 3;
    this.hasRevived = false;
    this.currentSpeed = this.baseSpeed;

    this.activePowerUp = null;
    this.isShielded = false;
    this.trackManager.magnetActive = false;

    // Initialize 3D Track & Patna Station Backdrop
    this.trackManager.init(currentDist);

    // Reset character position to center lane
    this.characterManager.currentLane = 1;
    this.characterManager.targetX = 0;
    this.characterManager.currentX = 0;
    this.characterManager.jumpY = 0;
    this.characterManager.isJumping = false;
    this.characterManager.isSliding = false;

    this.state = 'PLAYING';
    this.uiManager.showHUD();
    this.audioManager.startMusic();
  }

  reviveRun() {
    if (this.hasRevived) return;
    this.hasRevived = true;
    this.lives = 1;
    this.characterManager.jumpY = 2.0;
    this.state = 'PLAYING';
    this.uiManager.showHUD();
    this.audioManager.startMusic();
  }

  togglePause() {
    if (this.state === 'PLAYING') {
      this.state = 'PAUSED';
      this.audioManager.stopMusic();
      this.uiManager.showPauseScreen();
    } else if (this.state === 'PAUSED') {
      this.state = 'PLAYING';
      this.audioManager.startMusic();
      this.uiManager.showHUD();
    }
  }

  handleInput(action) {
    if (this.state !== 'PLAYING') {
      if (action === 'pause' && this.state === 'PAUSED') {
        this.togglePause();
      }
      return;
    }

    switch (action) {
      case 'left':
        this.characterManager.moveLeft();
        break;

      case 'right':
        this.characterManager.moveRight();
        break;

      case 'jump':
        if (!this.characterManager.isJumping) {
          this.sessionJumps++;
          this.updateMissionProgress('m3', 1);
        }
        this.characterManager.jump();
        this.audioManager.playJump();
        break;

      case 'slide':
        this.characterManager.slide();
        this.audioManager.playSlide();
        break;

      case 'pause':
        this.togglePause();
        break;
    }
  }

  activatePowerUp(type) {
    this.sessionPowerUps++;
    this.updateMissionProgress('m4', 1);

    const lvl = this.saveData.powerUpUpgrades[type] || 1;
    const baseDuration = 6 + lvl * 2;

    let name = 'POWER-UP';
    let icon = '⚡';

    if (type === 'sattuRocket') {
      name = 'SATTU ROCKET';
      icon = '🚀';
      this.characterManager.jumpY = 4.0; // High speed jet hover
    } else if (type === 'littiShield') {
      name = 'LITTI SHIELD';
      icon = '🛡️';
      this.isShielded = true;
    } else if (type === 'chaiMagnet') {
      name = 'CHAI MAGNET';
      icon = '🧲';
      this.trackManager.magnetActive = true;
    } else if (type === 'biharBoost' || type === 'superAuto') {
      name = type === 'superAuto' ? 'SUPER AUTO' : 'BIHAR BOOST';
      icon = type === 'superAuto' ? '🛺' : '⚡';
      this.currentSpeed = this.baseSpeed * 1.6;
    }

    this.activePowerUp = {
      type,
      name,
      icon,
      duration: baseDuration,
      remaining: baseDuration
    };

    this.audioManager.playPowerUp();
  }

  updateMissionProgress(missionId, amount) {
    if (!this.saveData.missionProgress) this.saveData.missionProgress = {};
    const current = this.saveData.missionProgress[missionId] || 0;
    this.saveData.missionProgress[missionId] = current + amount;
  }

  update(delta) {
    if (this.state !== 'PLAYING') return;

    this.currentSpeed += delta * 0.18;
    this.score += this.currentSpeed * delta;

    this.updateMissionProgress('m1', Math.floor(this.currentSpeed * delta));
    this.updateMissionProgress('m5', Math.floor(this.currentSpeed * delta));

    // Footstep audio timing
    if (!this.characterManager.isJumping && !this.characterManager.isSliding) {
      this.footstepTimer += delta * (this.currentSpeed / 16);
      if (this.footstepTimer > 0.35) {
        this.footstepTimer = 0;
        this.audioManager.playFootstep();
      }
    }

    // Combo timer decay
    if (this.comboCount > 0) {
      this.comboTimer -= delta;
      if (this.comboTimer <= 0) {
        this.comboCount = 0;
      }
    }

    if (this.activePowerUp) {
      this.activePowerUp.remaining -= delta;
      if (this.activePowerUp.remaining <= 0) {
        if (this.activePowerUp.type === 'chaiMagnet') {
          this.trackManager.magnetActive = false;
        } else if (this.activePowerUp.type === 'littiShield') {
          this.isShielded = false;
        } else if (this.activePowerUp.type === 'biharBoost' || this.activePowerUp.type === 'superAuto') {
          this.currentSpeed = this.baseSpeed;
        }
        this.activePowerUp = null;
      }
    }

    this.characterManager.update(delta, this.currentSpeed, Boolean(this.activePowerUp));
    const pBounds = this.characterManager.getBounds();

    this.trackManager.update(delta, this.currentSpeed, pBounds);

    // Near Miss Check
    if (this.trackManager.checkNearMiss(pBounds)) {
      this.score += 100;
      this.audioManager.playNearMiss();
      this.uiManager.showNearMissBanner();
      this.updateMissionProgress('m7', 1);
    }

    // Coin Pickup Collision & Combo System
    const coinsCollected = this.trackManager.checkCoinCollisions(pBounds);
    if (coinsCollected > 0) {
      this.comboCount += coinsCollected;
      this.comboTimer = 2.0; // Reset combo decay window
      this.sessionCoins += coinsCollected;
      this.score += coinsCollected * 10 * Math.min(this.comboCount, 5);

      this.updateMissionProgress('m2', coinsCollected);
      this.updateMissionProgress('m8', coinsCollected);

      this.audioManager.playCoin(this.comboCount);
      this.sceneManager.particleEffect.spawnCoinSparks(pBounds.minX, pBounds.minY + 0.8, pBounds.z);
    }

    // Power-Up Pickup Collision
    const powerUpHit = this.trackManager.checkPowerUpCollisions(pBounds);
    if (powerUpHit) {
      this.activatePowerUp(powerUpHit);
    }

    // Track Obstacle Collision Check
    const hitObstacle = this.trackManager.checkObstacleCollisions(pBounds);
    if (hitObstacle) {
      this.sceneManager.addCameraShake(0.55);
      this.characterManager.triggerHitAnimation();

      if (this.isShielded) {
        this.isShielded = false;
        this.activePowerUp = null;
        this.audioManager.playCrash();
      } else {
        this.lives--;
        this.audioManager.playTrainHorn();
        this.audioManager.playCrash();

        if (this.lives <= 0) {
          this.handleGameOver();
        } else {
          this.characterManager.jumpY = 1.2;
        }
      }
    }

    this.uiManager.updateHUD(this.score, this.sessionCoins, this.lives, this.activePowerUp);

    const speedRatio = this.currentSpeed / this.baseSpeed;
    this.sceneManager.updateCamera(this.characterManager.currentX, this.characterManager.jumpY, speedRatio, delta);
  }

  handleGameOver() {
    this.state = 'GAMEOVER';
    this.audioManager.stopMusic();

    if (this.score > this.saveData.highScore) {
      this.saveData.highScore = Math.floor(this.score);
    }
    this.saveData.coins += this.sessionCoins;
    this.saveProgress();

    const dist = this.journeyManager.getCurrentDistrict();
    const distHi = dist.hindiName || dist.hindi;
    const distEn = dist.englishName || dist.station;
    this.uiManager.showGameOver(this.score, this.sessionCoins, distHi, distEn, !this.hasRevived);
  }

  saveProgress() {
    SaveManager.save(this.saveData);
  }

  loop(timestamp) {
    const delta = Math.min((timestamp - this.lastFrameTime) / 1000, 0.1);
    this.lastFrameTime = timestamp;

    this.update(delta);
    this.sceneManager.render();

    requestAnimationFrame((t) => this.loop(t));
  }
}
