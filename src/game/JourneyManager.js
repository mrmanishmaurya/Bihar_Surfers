// JourneyManager.js - Loads stations.json and manages Bihar Journey progression
import STATIONS_DATA from '../../data/stations.json';
import MISSIONS_DATA from '../../data/missions.json';

export class JourneyManager {
  constructor(saveData) {
    this.saveData = saveData;
    this.stations = STATIONS_DATA;
    this.missions = MISSIONS_DATA;
  }

  getDistricts() {
    return this.stations.map((d, index) => {
      const isUnlocked = this.saveData.unlockedDistricts.includes(index);
      const isCompleted = this.saveData.completedDistricts ? this.saveData.completedDistricts.includes(index) : false;
      const stars = this.saveData.districtStars ? (this.saveData.districtStars[index] || 0) : (isCompleted ? 3 : 0);

      return {
        ...d,
        index,
        isUnlocked,
        isCompleted,
        stars,
        isActive: this.saveData.currentDistrictIndex === index
      };
    });
  }

  getCurrentDistrict() {
    const idx = this.saveData.currentDistrictIndex || 0;
    return this.stations[idx] || this.stations[0];
  }

  selectDistrict(index) {
    if (this.saveData.unlockedDistricts.includes(index)) {
      this.saveData.currentDistrictIndex = index;
      return true;
    }
    return false;
  }

  unlockNextDistrict() {
    const current = this.saveData.currentDistrictIndex || 0;
    const next = current + 1;

    // Mark current completed
    if (!this.saveData.completedDistricts) this.saveData.completedDistricts = [];
    if (!this.saveData.completedDistricts.includes(current)) {
      this.saveData.completedDistricts.push(current);
    }

    if (next < this.stations.length && !this.saveData.unlockedDistricts.includes(next)) {
      this.saveData.unlockedDistricts.push(next);
      this.saveData.currentDistrictIndex = next;
      return true;
    }
    return false;
  }

  getMissions() {
    const progress = this.saveData.missionProgress || {};
    const claimed = this.saveData.claimedMissions || [];

    return this.missions.map(m => {
      const currentVal = progress[m.id] || 0;
      const isComplete = currentVal >= m.target;
      const isClaimed = claimed.includes(m.id);

      return {
        ...m,
        currentVal,
        isComplete,
        isClaimed
      };
    });
  }

  claimMission(missionId) {
    if (!this.saveData.claimedMissions) this.saveData.claimedMissions = [];
    const mission = this.missions.find(m => m.id === missionId);

    if (mission && !this.saveData.claimedMissions.includes(missionId)) {
      this.saveData.claimedMissions.push(missionId);
      this.saveData.coins += mission.reward;
      return mission.reward;
    }
    return 0;
  }
}
