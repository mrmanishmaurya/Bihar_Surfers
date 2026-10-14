// UIManager.js - Manages HUD overlays, main menus, shops, journey maps, missions, and station intros

export class UIManager {
  constructor(uiContainer, game) {
    this.container = uiContainer;
    this.game = game;
    this.activeScreen = 'mainMenu';
  }

  showMainMenu() {
    this.activeScreen = 'mainMenu';
    const saveData = this.game.saveData;
    const district = this.game.journeyManager.getCurrentDistrict();

    this.container.innerHTML = `
      <div class="overlay-screen interactive">
        <h1 class="game-title">BIHAR SURFERS</h1>
        <div class="game-subtitle">RUN ACROSS BIHAR - ${district.hindiName || district.hindi} (${district.englishName || district.station})</div>

        <div class="menu-buttons">
          <button class="btn-primary" id="btn-start-game">
            <span>▶ PLAY RUN</span>
          </button>
          <button class="btn-secondary" id="btn-open-journey">
            <span>🗺️ JOURNEY MODE (38 DISTRICTS)</span>
          </button>
          <button class="btn-secondary" id="btn-open-missions">
            <span>🎯 MISSIONS & REWARDS</span>
          </button>
          <button class="btn-secondary" id="btn-open-characters">
            <span>👤 CHARACTERS & LOCKER</span>
          </button>
          <button class="btn-secondary" id="btn-open-powerups">
            <span>⚡ POWER-UP UPGRADES</span>
          </button>
        </div>

        <div style="margin-top: 24px; display: flex; gap: 20px;" class="hud-badge score">
          <span>🏆 HIGH SCORE: ${saveData.highScore}</span>
          <span style="color: var(--gold);">₹ ${saveData.coins}</span>
        </div>
      </div>
    `;

    document.getElementById('btn-start-game').onclick = () => this.showStationIntro();
    document.getElementById('btn-open-journey').onclick = () => this.showJourneyMap();
    document.getElementById('btn-open-missions').onclick = () => this.showMissions();
    document.getElementById('btn-open-characters').onclick = () => this.showCharacterShop();
    document.getElementById('btn-open-powerups').onclick = () => this.showPowerUpShop();
  }

  showStationIntro() {
    this.activeScreen = 'stationIntro';
    const district = this.game.journeyManager.getCurrentDistrict();

    this.container.innerHTML = `
      <div class="overlay-screen interactive" style="background: rgba(5, 10, 20, 0.95);">
        <div style="font-size: 1.4rem; color: var(--secondary); font-weight: 800; letter-spacing: 3px;">NEXT DESTINATION</div>
        <h2 style="font-size: 4rem; font-family: var(--font-title); color: var(--gold); margin: 10px 0;">
          ${district.hindiName || district.hindi}
        </h2>
        <div style="font-size: 2rem; font-weight: 900; color: #ffffff; letter-spacing: 2px;">
          ${district.englishName || district.station} (${district.code})
        </div>
        <p style="color: #94a3b8; margin: 16px 0; max-width: 400px; text-align: center;">
          ${district.desc}
        </p>

        <button class="btn-primary" id="btn-begin-run" style="margin-top: 24px; padding: 18px 48px; font-size: 1.8rem;">
          🚀 START RUN
        </button>
      </div>
    `;

    document.getElementById('btn-begin-run').onclick = () => this.game.startRun();
  }

  showHUD() {
    this.activeScreen = 'hud';
    const saveData = this.game.saveData;

    this.container.innerHTML = `
      <div class="hud-container">
        <div class="hud-badge coin-pill" id="hud-coins">
          <div class="coin-icon-circle">₹</div>
          <span id="val-total-coins">${(saveData.coins || 0).toLocaleString()}</span>
        </div>

        <div class="hud-badge score-pill" id="hud-score">
          <div style="font-size: 0.8rem; color: var(--gold); letter-spacing: 2px; text-transform: uppercase; font-weight: 900;">SCORE</div>
          <span id="val-score">0</span>
        </div>

        <div class="hud-right">
          <div class="hud-badge hearts">
            ❤️ <span id="val-lives">3</span>
          </div>
          <button class="btn-icon interactive" id="btn-pause">⏸</button>
        </div>
      </div>

      <!-- Middle-Right Side Stats Badge -->
      <div class="side-stats-badge" id="side-stats">
        <div class="side-stat-row">
          <span class="side-stat-icon">📍</span>
          <div style="display: flex; flex-direction: column;">
            <span class="side-stat-label">DISTANCE</span>
            <span class="side-stat-value" id="side-val-dist">0 m</span>
          </div>
        </div>
        <div class="side-stat-row">
          <span class="side-stat-icon">🪙</span>
          <div style="display: flex; flex-direction: column;">
            <span class="side-stat-label">COINS</span>
            <span class="side-stat-value" id="side-val-coins">0</span>
          </div>
        </div>
        <div class="side-stat-row">
          <span class="side-stat-icon">⭐</span>
          <div style="display: flex; flex-direction: column;">
            <span class="side-stat-label">SCORE</span>
            <span class="side-stat-value" id="side-val-score">0</span>
          </div>
        </div>
      </div>

      <!-- Near Miss Popup -->
      <div id="near-miss-banner" style="display: none; position: absolute; top: 130px; left: 50%; transform: translateX(-50%); background: linear-gradient(135deg, #ff0055, #ff5500); padding: 8px 24px; border-radius: 30px; font-weight: 900; font-size: 1.3rem; color: #ffffff; box-shadow: 0 0 20px rgba(255,0,85,0.8); z-index: 25; animation: pulse-glow 0.5s infinite alternate;">
        ⚡ NEAR MISS! +100 PTS
      </div>

      <!-- Active Power-Up Timer Bar (Bottom Right) -->
      <div class="powerup-active-bar" id="powerup-bar" style="display: none;">
        <span class="powerup-active-icon" id="powerup-icon">🚀</span>
        <div style="display: flex; flex-direction: column; flex: 1;">
          <span class="powerup-active-name" id="powerup-name">SATTU ROCKET</span>
          <div class="powerup-active-timer">
            <div class="powerup-active-fill" id="powerup-fill"></div>
          </div>
        </div>
        <span id="powerup-seconds" style="font-weight: 900; font-size: 1.1rem; color: var(--gold);">8s</span>
      </div>

      <!-- Bottom Controls -->
      <div class="mobile-controls interactive">
        <div class="dpad-container">
          <button class="dpad-btn" id="dpad-left">◄</button>
          <button class="dpad-btn" id="dpad-right">►</button>
          <button class="dpad-btn" id="dpad-up">▲</button>
          <button class="dpad-btn" id="dpad-down">▼</button>
        </div>

        <div class="action-btns">
          <button class="action-btn" id="btn-act-jump">JUMP</button>
          <button class="action-btn slide" id="btn-act-slide">SLIDE</button>
        </div>
      </div>
    `;

    document.getElementById('btn-pause').onclick = () => this.game.togglePause();

    document.getElementById('dpad-left').onclick = () => this.game.handleInput('left');
    document.getElementById('dpad-right').onclick = () => this.game.handleInput('right');
    document.getElementById('dpad-up').onclick = () => this.game.handleInput('jump');
    document.getElementById('dpad-down').onclick = () => this.game.handleInput('slide');

    document.getElementById('btn-act-jump').onclick = () => this.game.handleInput('jump');
    document.getElementById('btn-act-slide').onclick = () => this.game.handleInput('slide');
  }

  showNearMissBanner() {
    const banner = document.getElementById('near-miss-banner');
    if (banner) {
      banner.style.display = 'block';
      setTimeout(() => {
        if (banner) banner.style.display = 'none';
      }, 1200);
    }
  }

  updateHUD(score, coins, lives, activePowerUp = null) {
    if (this.activeScreen !== 'hud') return;
    const scoreVal = Math.floor(score);

    const scoreElem = document.getElementById('val-score');
    const sideDistElem = document.getElementById('side-val-dist');
    const sideCoinsElem = document.getElementById('side-val-coins');
    const sideScoreElem = document.getElementById('side-val-score');
    const livesElem = document.getElementById('val-lives');

    if (scoreElem) scoreElem.innerText = scoreVal.toLocaleString();
    if (sideDistElem) sideDistElem.innerText = `${Math.floor(scoreVal * 0.1)} m`;
    if (sideCoinsElem) sideCoinsElem.innerText = coins;
    if (sideScoreElem) sideScoreElem.innerText = scoreVal.toLocaleString();
    if (livesElem) livesElem.innerText = lives;

    const pBar = document.getElementById('powerup-bar');
    if (pBar) {
      if (activePowerUp) {
        pBar.style.display = 'flex';
        document.getElementById('powerup-name').innerText = activePowerUp.name;
        document.getElementById('powerup-icon').innerText = activePowerUp.icon;
        document.getElementById('powerup-fill').style.width = `${(activePowerUp.remaining / activePowerUp.duration) * 100}%`;
        document.getElementById('powerup-seconds').innerText = `${Math.ceil(activePowerUp.remaining)}s`;
      } else {
        pBar.style.display = 'none';
      }
    }
  }

  showJourneyMap() {
    this.activeScreen = 'journeyMap';
    const districts = this.game.journeyManager.getDistricts();

    const districtCardsHtml = districts.map(d => `
      <div class="district-card ${d.isUnlocked ? 'unlocked' : 'locked'} ${d.isActive ? 'active' : ''}" data-idx="${d.index}">
        <span class="district-num">DISTRICT #${d.index + 1}</span>
        <span class="district-name-hi">${d.hindiName || d.nameHi}</span>
        <span class="district-name-en">${d.englishName || d.station}</span>
        <span class="district-station-code">${d.isCompleted ? '✓ COMPLETED' : (d.isUnlocked ? '▶ PLAY' : '🔒 LOCKED')}</span>
      </div>
    `).join('');

    this.container.innerHTML = `
      <div class="overlay-screen interactive">
        <h2 style="font-size: 2.5rem; font-family: var(--font-title); color: var(--gold); margin-bottom: 8px;">38 DISTRICTS OF BIHAR</h2>
        <p style="color: #94a3b8; margin-bottom: 16px;">Select an unlocked district level to run on local railway scenery:</p>

        <div class="district-grid">
          ${districtCardsHtml}
        </div>

        <button class="btn-secondary" id="btn-back-main" style="margin-top: 20px;">
          ⬅ BACK TO MAIN MENU
        </button>
      </div>
    `;

    document.querySelectorAll('.district-card.unlocked').forEach(card => {
      card.onclick = () => {
        const idx = parseInt(card.dataset.idx);
        this.game.journeyManager.selectDistrict(idx);
        this.game.saveProgress();
        this.showStationIntro();
      };
    });

    document.getElementById('btn-back-main').onclick = () => this.showMainMenu();
  }

  showMissions() {
    this.activeScreen = 'missions';
    const missions = this.game.journeyManager.getMissions();

    const cardsHtml = missions.map(m => `
      <div class="shop-card" style="align-items: flex-start; text-align: left;">
        <div class="shop-card-title">${m.title}</div>
        <div class="shop-card-desc">${m.desc}</div>
        <div style="font-size: 0.9rem; color: var(--gold); margin-top: 4px;">REWARD: ₹ ${m.reward}</div>
        <div style="width: 100%; height: 8px; background: rgba(255,255,255,0.1); border-radius: 4px; margin: 8px 0; overflow: hidden;">
          <div style="width: ${Math.min((m.currentVal / m.target) * 100, 100)}%; height: 100%; background: var(--secondary);"></div>
        </div>
        ${m.isClaimed ? '<span style="color: #94a3b8; font-weight: 800;">✓ CLAIMED</span>' : (m.isComplete ? `<button class="btn-primary claim-mission-btn" data-id="${m.id}" style="font-size: 0.9rem; padding: 6px 12px;">CLAIM REWARD</button>` : `<span style="font-size: 0.85rem; color: #94a3b8;">${m.currentVal} / ${m.target}</span>`)}
      </div>
    `).join('');

    this.container.innerHTML = `
      <div class="overlay-screen interactive">
        <h2 style="font-size: 2.5rem; font-family: var(--font-title); color: var(--gold);">MISSIONS & REWARDS</h2>

        <div class="shop-grid" style="grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));">
          ${cardsHtml}
        </div>

        <button class="btn-secondary" id="btn-back-main" style="margin-top: 20px;">
          ⬅ BACK TO MAIN MENU
        </button>
      </div>
    `;

    document.querySelectorAll('.claim-mission-btn').forEach(btn => {
      btn.onclick = () => {
        const id = btn.dataset.id;
        this.game.journeyManager.claimMission(id);
        this.game.saveProgress();
        this.showMissions();
      };
    });

    document.getElementById('btn-back-main').onclick = () => this.showMainMenu();
  }

  showCharacterShop() {
    this.activeScreen = 'characterShop';
    const saveData = this.game.saveData;

    const chars = [
      { id: 'raj', name: 'Raj (Bihar Runner)', icon: '🏃‍♂️', price: 0, desc: 'Original Bihar runner in orange jersey #23.' },
      { id: 'vikram', name: 'Vikram (Sprinter)', icon: '⚡', price: 1000, desc: 'High-speed athlete with amber headband & tank top.' },
      { id: 'ananya', name: 'Ananya (Bihar Hero)', icon: '🌟', price: 2500, desc: 'Pro runner with teal hoodie & high-top sneakers.' },
      { id: 'arjun', name: 'Arjun (Champion)', icon: '🏆', price: 5000, desc: 'Bihar champion runner in royal blue #99 kit & cap.' }
    ];

    const cardsHtml = chars.map(c => {
      const isUnlocked = saveData.unlockedCharacters.includes(c.id);
      const isSelected = saveData.selectedCharacter === c.id;

      return `
        <div class="shop-card ${isSelected ? 'selected' : ''}">
          <div class="shop-card-icon">${c.icon}</div>
          <div class="shop-card-title">${c.name}</div>
          <div class="shop-card-desc">${c.desc}</div>
          <button class="btn-primary select-char-btn" data-id="${c.id}" data-price="${c.price}" style="margin-top: 8px; font-size: 1rem; padding: 8px 16px;">
            ${isSelected ? 'EQUIPPED' : (isUnlocked ? 'SELECT' : `UNLOCK (₹ ${c.price})`)}
          </button>
        </div>
      `;
    }).join('');

    this.container.innerHTML = `
      <div class="overlay-screen interactive">
        <h2 style="font-size: 2.5rem; font-family: var(--font-title); color: var(--gold);">PLAYABLE CHARACTERS</h2>
        <div class="hud-badge" style="margin: 12px 0;">₹ ${saveData.coins}</div>

        <div class="shop-grid">
          ${cardsHtml}
        </div>

        <button class="btn-secondary" id="btn-back-main" style="margin-top: 20px;">
          ⬅ BACK TO MAIN MENU
        </button>
      </div>
    `;

    document.querySelectorAll('.select-char-btn').forEach(btn => {
      btn.onclick = () => {
        const id = btn.dataset.id;
        const price = parseInt(btn.dataset.price);

        if (saveData.unlockedCharacters.includes(id)) {
          saveData.selectedCharacter = id;
          this.game.characterManager.buildCharacter(id);
        } else if (saveData.coins >= price) {
          saveData.coins -= price;
          saveData.unlockedCharacters.push(id);
          saveData.selectedCharacter = id;
          this.game.characterManager.buildCharacter(id);
        }
        this.game.saveProgress();
        this.showCharacterShop();
      };
    });

    document.getElementById('btn-back-main').onclick = () => this.showMainMenu();
  }

  showPowerUpShop() {
    this.activeScreen = 'powerUpShop';
    const saveData = this.game.saveData;

    const powerups = [
      { id: 'sattuRocket', name: 'SATTU ROCKET', icon: '🚀', desc: 'Launch forward at super speed!' },
      { id: 'littiShield', name: 'LITTI SHIELD', icon: '🛡️', desc: 'Protects from train collision.' },
      { id: 'chaiMagnet', name: 'CHAI MAGNET', icon: '🧲', desc: 'Attracts nearby golden Rupee coins.' },
      { id: 'gangaGlide', name: 'GANGA GLIDE', icon: '🏄‍♂️', desc: 'Hover above track obstacles.' }
    ];

    const cardsHtml = powerups.map(p => {
      const level = saveData.powerUpUpgrades[p.id] || 1;
      const upgradePrice = level * 500;

      return `
        <div class="shop-card">
          <div class="shop-card-icon">${p.icon}</div>
          <div class="shop-card-title">${p.name}</div>
          <div class="shop-card-desc">${p.desc}</div>
          <div style="font-weight: 800; color: var(--secondary);">LEVEL ${level} / 5</div>
          <button class="btn-primary upgrade-pu-btn" data-id="${p.id}" data-price="${upgradePrice}" style="margin-top: 8px; font-size: 1rem; padding: 8px 16px;">
            UPGRADE (₹ ${upgradePrice})
          </button>
        </div>
      `;
    }).join('');

    this.container.innerHTML = `
      <div class="overlay-screen interactive">
        <h2 style="font-size: 2.5rem; font-family: var(--font-title); color: var(--gold);">POWER-UP UPGRADES</h2>
        <div class="hud-badge" style="margin: 12px 0;">₹ ${saveData.coins}</div>

        <div class="shop-grid">
          ${cardsHtml}
        </div>

        <button class="btn-secondary" id="btn-back-main" style="margin-top: 20px;">
          ⬅ BACK TO MAIN MENU
        </button>
      </div>
    `;

    document.querySelectorAll('.upgrade-pu-btn').forEach(btn => {
      btn.onclick = () => {
        const id = btn.dataset.id;
        const price = parseInt(btn.dataset.price);
        const lvl = saveData.powerUpUpgrades[id] || 1;

        if (saveData.coins >= price && lvl < 5) {
          saveData.coins -= price;
          saveData.powerUpUpgrades[id] = lvl + 1;
          this.game.saveProgress();
          this.showPowerUpShop();
        }
      };
    });

    document.getElementById('btn-back-main').onclick = () => this.showMainMenu();
  }

  showPauseScreen() {
    this.activeScreen = 'pause';
    this.container.innerHTML = `
      <div class="overlay-screen interactive">
        <h2 style="font-size: 3.5rem; font-family: var(--font-title); color: var(--gold);">GAME PAUSED</h2>

        <div class="menu-buttons" style="margin-top: 24px;">
          <button class="btn-primary" id="btn-resume">
            ▶ RESUME RUN
          </button>
          <button class="btn-secondary" id="btn-restart">
            🔄 RESTART
          </button>
          <button class="btn-secondary" id="btn-quit">
            🏠 MAIN MENU
          </button>
        </div>
      </div>
    `;

    document.getElementById('btn-resume').onclick = () => this.game.togglePause();
    document.getElementById('btn-restart').onclick = () => this.game.startRun();
    document.getElementById('btn-quit').onclick = () => this.showMainMenu();
  }

  showGameOver(score, coins, distNameHi, distNameEn, canRevive = true) {
    this.activeScreen = 'gameOver';
    const isNewHighScore = score > this.game.saveData.highScore;

    this.container.innerHTML = `
      <div class="overlay-screen interactive">
        <h2 style="font-size: 3.5rem; font-family: var(--font-title); color: #ff3366;">RUN OVER!</h2>

        <div class="glass-panel" style="padding: 24px; text-align: center; max-width: 380px; width: 100%; margin: 16px 0;">
          <div style="font-size: 1.1rem; color: #94a3b8;">${distNameHi} (${distNameEn})</div>
          
          <div style="font-size: 3rem; font-family: var(--font-title); color: var(--gold); margin: 8px 0;">
            ${Math.floor(score)} m
          </div>

          ${isNewHighScore ? '<div style="color: #00ff66; font-weight: 900; letter-spacing: 1px;">🎉 NEW HIGH SCORE!</div>' : ''}

          <div class="hud-badge" style="display: inline-flex; margin-top: 12px;">
            <span>₹</span> +${coins} COINS
          </div>
        </div>

        <div class="menu-buttons">
          ${canRevive ? '<button class="btn-primary" id="btn-revive-run" style="background: linear-gradient(135deg, #00c853, #009624); border-color: #69f0ae;">❤️ REVIVE RUN (FREE 1x)</button>' : ''}
          <button class="btn-primary" id="btn-run-again">
            🏃 RUN AGAIN
          </button>
          <button class="btn-secondary" id="btn-next-district">
            🗺️ NEXT DISTRICT
          </button>
          <button class="btn-secondary" id="btn-go-menu">
            🏠 MAIN MENU
          </button>
        </div>
      </div>
    `;

    if (canRevive && document.getElementById('btn-revive-run')) {
      document.getElementById('btn-revive-run').onclick = () => this.game.reviveRun();
    }
    document.getElementById('btn-run-again').onclick = () => this.showStationIntro();
    document.getElementById('btn-next-district').onclick = () => {
      this.game.journeyManager.unlockNextDistrict();
      this.game.saveProgress();
      this.showStationIntro();
    };
    document.getElementById('btn-go-menu').onclick = () => this.showMainMenu();
  }
}
