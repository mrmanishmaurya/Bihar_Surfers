using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Shop
{
    [System.Serializable]
    public class PowerUpUpgrade
    {
        public PowerUps.PowerUpType type;
        public string title;
        public int currentLevel = 1;
        public int maxLevel = 5;
        public int upgradeCostCoins = 500;
    }

    /// <summary>
    /// In-game virtual currency shop handling character unlocks, skin purchases,
    /// power-up duration upgrades, and virtual coin reward packs.
    /// Modular IAP integration hooks included for future monetization.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        public List<PowerUpUpgrade> powerUpUpgrades = new List<PowerUpUpgrade>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitUpgrades();
        }

        private void InitUpgrades()
        {
            powerUpUpgrades.Clear();
            powerUpUpgrades.Add(new PowerUpUpgrade { type = PowerUps.PowerUpType.Magnet, title = "Chai Magnet Duration", upgradeCostCoins = 500 });
            powerUpUpgrades.Add(new PowerUpUpgrade { type = PowerUps.PowerUpType.Shield, title = "Litti Shield Durability", upgradeCostCoins = 750 });
            powerUpUpgrades.Add(new PowerUpUpgrade { type = PowerUps.PowerUpType.SpeedBoost, title = "Sattu Rocket Speed", upgradeCostCoins = 1000 });
            powerUpUpgrades.Add(new PowerUpUpgrade { type = PowerUps.PowerUpType.CoinMultiplier, title = "Double Coin Multiplier", upgradeCostCoins = 1200 });
        }

        public bool UpgradePowerUp(PowerUps.PowerUpType type)
        {
            var upgrade = powerUpUpgrades.Find(u => u.type == type);
            if (upgrade == null || upgrade.currentLevel >= upgrade.maxLevel) return false;

            int coins = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.coins : 0;
            if (coins >= upgrade.upgradeCostCoins)
            {
                Core.SaveManager.Instance.data.coins -= upgrade.upgradeCostCoins;
                upgrade.currentLevel++;
                upgrade.upgradeCostCoins = (int)(upgrade.upgradeCostCoins * 1.5f);
                Core.SaveManager.Instance.SaveGame();
                Core.AudioManager.Instance?.PlaySFX("PurchaseSuccess");
                return true;
            }

            Core.AudioManager.Instance?.PlaySFX("PurchaseFail");
            return false;
        }

        public void ClaimVirtualCoinPack(int bonusCoins)
        {
            Core.SaveManager.Instance?.AddCoins(bonusCoins);
            Core.AudioManager.Instance?.PlaySFX("RewardClaimed");
        }
    }
}
