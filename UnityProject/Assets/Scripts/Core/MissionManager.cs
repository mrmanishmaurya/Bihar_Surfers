using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Core
{
    [System.Serializable]
    public class Mission
    {
        public string id;
        public string title;
        public string description;
        public int targetAmount;
        public int currentAmount;
        public int coinReward;
        public int xpReward;
        public string unlockRewardId;
        public bool isCompleted;
        public bool isClaimed;
    }

    /// <summary>
    /// Tracks active gameplay missions, progress, persistence, and rewards (Coins, XP, Unlocks).
    /// </summary>
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }

        public List<Mission> activeMissions = new List<Mission>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeDefaultMissions();
        }

        private void InitializeDefaultMissions()
        {
            activeMissions.Clear();

            // 1. Collect 100 coins
            activeMissions.Add(new Mission { id = "m1", title = "Rupee Collector", description = "Collect 100 golden Rupee coins.", targetAmount = 100, coinReward = 500, xpReward = 100 });

            // 2. Run 1000 meters
            activeMissions.Add(new Mission { id = "m2", title = "Long Distance Runner", description = "Run 1,000 meters in a single run.", targetAmount = 1000, coinReward = 750, xpReward = 150 });

            // 3. Run without collision for 60 seconds
            activeMissions.Add(new Mission { id = "m3", title = "Clean Run", description = "Run without taking damage for 60 seconds.", targetAmount = 60, coinReward = 600, xpReward = 120 });

            // 4. Use 3 power-ups
            activeMissions.Add(new Mission { id = "m4", title = "Power Master", description = "Use 3 power-ups during a single run.", targetAmount = 3, coinReward = 400, xpReward = 80 });

            // 5. Reach Buxar
            activeMissions.Add(new Mission { id = "m5", title = "Reach Buxar", description = "Run past 1,600 meters to reach Buxar Ganga riverfront.", targetAmount = 1600, coinReward = 1000, xpReward = 200, unlockRewardId = "Ravi" });

            // 6. Score 10,000 points
            activeMissions.Add(new Mission { id = "m6", title = "High Scorer", description = "Score 10,000 points in Bihar Surfers.", targetAmount = 10000, coinReward = 1500, xpReward = 300 });
        }

        public void UpdateProgress(string missionId, int amount)
        {
            Mission mission = activeMissions.Find(m => m.id == missionId);
            if (mission != null && !mission.isCompleted)
            {
                mission.currentAmount += amount;
                if (mission.currentAmount >= mission.targetAmount)
                {
                    mission.isCompleted = true;
                    Debug.Log($"Mission Completed: {mission.title}!");
                    AudioManager.Instance?.PlaySFX("MissionComplete");
                    SaveManager.Instance?.SaveGame();
                }
            }
        }

        public bool ClaimReward(string missionId)
        {
            Mission mission = activeMissions.Find(m => m.id == missionId);
            if (mission != null && mission.isCompleted && !mission.isClaimed)
            {
                mission.isClaimed = true;
                SaveManager.Instance?.AddCoins(mission.coinReward);

                if (!string.IsNullOrEmpty(mission.unlockRewardId))
                {
                    Player.CharacterManager.Instance?.UnlockCharacter(mission.unlockRewardId);
                }

                SaveManager.Instance?.SaveGame();
                return true;
            }
            return false;
        }
    }
}
