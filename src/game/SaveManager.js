// SaveManager.js - Manages local persistence for Bihar Surfers

const STORAGE_KEY = 'bihar_surfers_save_data_v1';

const DEFAULT_SAVE = {
  highScore: 0,
  coins: 500, // starting coins
  selectedCharacter: 'raj', // 'raj', 'motu', 'patlu', 'john', 'ghasitaram'
  unlockedCharacters: ['raj', 'motu'],
  currentDistrictIndex: 0, // 0 = Patna, 1 = Gaya, ... 37 = Gopalganj
  unlockedDistricts: [0], // district indices unlocked
  districtLevels: {}, // districtId -> level (1-5)
  powerUpUpgrades: {
    sattuRocket: 1,
    littiShield: 1,
    biharBoost: 1,
    chaiMagnet: 1,
    gangaGlide: 1,
    superAuto: 1
  },
  soundEnabled: true,
  musicEnabled: true,
  stats: {
    totalRuns: 0,
    totalCoinsCollected: 0,
    totalDistance: 0,
    trainsAvoided: 0
  }
};

export class SaveManager {
  static load() {
    try {
      const data = localStorage.getItem(STORAGE_KEY);
      if (!data) return { ...DEFAULT_SAVE };
      return { ...DEFAULT_SAVE, ...JSON.parse(data) };
    } catch (e) {
      console.warn('Could not load save data from localStorage:', e);
      return { ...DEFAULT_SAVE };
    }
  }

  static save(saveData) {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(saveData));
    } catch (e) {
      console.warn('Could not save data to localStorage:', e);
    }
  }

  static reset() {
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch (e) {}
    return { ...DEFAULT_SAVE };
  }
}
