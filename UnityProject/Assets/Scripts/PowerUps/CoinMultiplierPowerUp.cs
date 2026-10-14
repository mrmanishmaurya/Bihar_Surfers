using UnityEngine;

namespace BiharSurfers.PowerUps
{
    /// <summary>
    /// Coin Multiplier Power-Up: Doubles all collected coins for a limited duration.
    /// </summary>
    public class CoinMultiplierPowerUp : PowerUp
    {
        public int multiplier = 2;

        protected override void ActivateEffect()
        {
            if (Coins.CoinManager.Instance != null)
            {
                Coins.CoinManager.Instance.coinMultiplier = multiplier;
                Invoke(nameof(Deactivate), baseDuration);
            }
        }

        private void Deactivate()
        {
            if (Coins.CoinManager.Instance != null)
            {
                Coins.CoinManager.Instance.coinMultiplier = 1;
            }
        }
    }
}
