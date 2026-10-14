using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BiharSurfers.Shop
{
    /// <summary>
    /// UI interface for the Virtual Coin Shop, displaying character cards,
    /// skin items, power-up upgrades, and bonus coin reward buttons.
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        public static ShopUI Instance { get; private set; }

        [Header("Shop Panel")]
        public GameObject shopPanel;

        [Header("Header Text")]
        public TextMeshProUGUI coinBalanceText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void OpenShop()
        {
            if (shopPanel != null) shopPanel.SetActive(true);
            UpdateCoinsDisplay();
        }

        public void CloseShop()
        {
            if (shopPanel != null) shopPanel.SetActive(false);
        }

        public void UpdateCoinsDisplay()
        {
            int coins = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.coins : 0;
            if (coinBalanceText != null) coinBalanceText.text = $"{coins} 🪙";
        }

        public void OnClickUpgradeMagnet()
        {
            if (ShopManager.Instance != null && ShopManager.Instance.UpgradePowerUp(PowerUps.PowerUpType.Magnet))
            {
                UpdateCoinsDisplay();
            }
        }

        public void OnClickUpgradeShield()
        {
            if (ShopManager.Instance != null && ShopManager.Instance.UpgradePowerUp(PowerUps.PowerUpType.Shield))
            {
                UpdateCoinsDisplay();
            }
        }

        public void OnClickUpgradeSpeedBoost()
        {
            if (ShopManager.Instance != null && ShopManager.Instance.UpgradePowerUp(PowerUps.PowerUpType.SpeedBoost))
            {
                UpdateCoinsDisplay();
            }
        }

        public void OnClickBonusCoinsPack()
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.ClaimVirtualCoinPack(500);
                UpdateCoinsDisplay();
            }
        }
    }
}
