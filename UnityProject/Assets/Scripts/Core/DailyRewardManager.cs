using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Core
{
    [System.Serializable]
    public class DailyRewardItem
    {
        public int dayNumber; // 1 to 7
        public int rewardCoins;
        public string rewardItemName;
        public bool isClaimed;
    }

    /// <summary>
    /// Manages 7-day login daily rewards (Day 1 through Day 7), streak calculations,
    /// and local persistence.
    /// </summary>
    public class DailyRewardManager : MonoBehaviour
    {
        public static DailyRewardManager Instance { get; private set; }

        public List<DailyRewardItem> dailyRewards = new List<DailyRewardItem>();
        public int currentRewardDay = 1;
        public bool canClaimToday = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitDailyRewards();
        }

        private void InitDailyRewards()
        {
            dailyRewards.Clear();
            dailyRewards.Add(new DailyRewardItem { dayNumber = 1, rewardCoins = 200, rewardItemName = "200 Coins" });
            dailyRewards.Add(new DailyRewardItem { dayNumber = 2, rewardCoins = 400, rewardItemName = "400 Coins" });
            dailyRewards.Add(new DailyRewardItem { dayNumber = 3, rewardCoins = 600, rewardItemName = "600 Coins" });
            dailyRewards.Add(new DailyRewardItem { dayNumber = 4, rewardCoins = 800, rewardItemName = "800 Coins + Chai Magnet" });
            dailyRewards.Add(new DailyRewardItem { dayNumber = 5, rewardCoins = 1000, rewardItemName = "1,000 Coins" });
            dailyRewards.Add(new DailyRewardItem { dayNumber = 6, rewardCoins = 1500, rewardItemName = "1,500 Coins + Litti Shield" });
            dailyRewards.Add(new DailyRewardItem { dayNumber = 7, rewardCoins = 2500, rewardItemName = "2,500 Coins + Festival Skin" });

            LoadClaimStatus();
        }

        public void LoadClaimStatus()
        {
            string lastClaimDateStr = PlayerPrefs.GetString("BiharSurfers_LastClaimDate", "");
            currentRewardDay = PlayerPrefs.GetInt("BiharSurfers_DailyDay", 1);

            string todayStr = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (lastClaimDateStr == todayStr)
            {
                canClaimToday = false;
            }
            else
            {
                canClaimToday = true;
            }
        }

        public bool ClaimDailyReward()
        {
            if (!canClaimToday) return false;

            int dayIndex = Mathf.Clamp(currentRewardDay - 1, 0, dailyRewards.Count - 1);
            var item = dailyRewards[dayIndex];
            item.isClaimed = true;

            SaveManager.Instance?.AddCoins(item.rewardCoins);

            string todayStr = DateTime.UtcNow.ToString("yyyy-MM-dd");
            PlayerPrefs.SetString("BiharSurfers_LastClaimDate", todayStr);

            currentRewardDay = (currentRewardDay % 7) + 1;
            PlayerPrefs.SetInt("BiharSurfers_DailyDay", currentRewardDay);
            PlayerPrefs.Save();

            canClaimToday = false;
            AudioManager.Instance?.PlaySFX("RewardClaimed");
            return true;
        }
    }
}
