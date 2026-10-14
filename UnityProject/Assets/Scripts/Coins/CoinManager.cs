using UnityEngine;

namespace BiharSurfers.Coins
{
    /// <summary>
    /// Tracks coins collected during current run, combo multiplier, and total bank balance.
    /// </summary>
    public class CoinManager : MonoBehaviour
    {
        public static CoinManager Instance { get; private set; }

        public int sessionCoins { get; private set; } = 0;
        public int comboCount { get; private set; } = 0;
        public float comboTimer { get; private set; } = 0f;
        public int coinMultiplier = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ResetSession()
        {
            sessionCoins = 0;
            comboCount = 0;
            comboTimer = 0f;
            coinMultiplier = 1;
        }

        private void Update()
        {
            if (comboCount > 0)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0f)
                {
                    comboCount = 0;
                }
            }
        }

        public void AddCoins(int amount)
        {
            int earned = amount * coinMultiplier;
            sessionCoins += earned;
            comboCount++;
            comboTimer = 2.0f; // Reset combo decay window

            // Update global Score & Missions
            Core.ScoreManager.Instance?.AddCoinScore(earned, comboCount);
            Core.MissionManager.Instance?.UpdateProgress("m2", earned);
            Core.MissionManager.Instance?.UpdateProgress("m8", earned);
        }
    }
}
