using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Core
{
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public bool isUnlocked;
    }

    /// <summary>
    /// Tracks achievements (First Run, Coin Collector, Long Runner, Bihar Explorer, Speed Runner, Power User).
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        public List<Achievement> achievements = new List<Achievement>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitAchievements();
        }

        private void InitAchievements()
        {
            achievements.Clear();
            achievements.Add(new Achievement { id = "ach_first_run", title = "First Run", description = "Complete your first run across Patna Junction." });
            achievements.Add(new Achievement { id = "ach_coins", title = "Coin Collector", description = "Collect 1,000 golden Rupee coins." });
            achievements.Add(new Achievement { id = "ach_long_runner", title = "Long Runner", description = "Run 5,000 meters across Bihar." });
            achievements.Add(new Achievement { id = "ach_explorer", title = "Bihar Explorer", description = "Travel across 5 Bihar stations (Patna, Ara, Buxar, Sasaram, Chhapra)." });
            achievements.Add(new Achievement { id = "ach_speed_runner", title = "Speed Runner", description = "Reach maximum running speed." });
            achievements.Add(new Achievement { id = "ach_power_user", title = "Power User", description = "Use 10 power-ups across your career." });
        }

        public void Unlock(string id)
        {
            Achievement ach = achievements.Find(a => a.id == id);
            if (ach != null && !ach.isUnlocked)
            {
                ach.isUnlocked = true;
                Debug.Log($"Achievement Unlocked: {ach.title}!");
                AudioManager.Instance?.PlaySFX("AchievementUnlocked");
                SaveManager.Instance?.SaveGame();
            }
        }
    }
}
