using UnityEngine;

namespace BiharSurfers.PowerUps
{
    /// <summary>
    /// Sattu Rocket / Bihar Boost Power-Up: Temporarily increases running speed & grants invincibility.
    /// </summary>
    public class SpeedBoostPowerUp : PowerUp
    {
        public float speedMultiplier = 1.6f;

        protected override void ActivateEffect()
        {
            if (Player.PlayerController.Instance != null)
            {
                Player.PlayerController.Instance.currentSpeedMultiplier = speedMultiplier;
                Player.PlayerController.Instance.isInvincible = true;
                Invoke(nameof(Deactivate), baseDuration);
            }
        }

        private void Deactivate()
        {
            if (Player.PlayerController.Instance != null)
            {
                Player.PlayerController.Instance.currentSpeedMultiplier = 1.0f;
                Player.PlayerController.Instance.isInvincible = false;
            }
        }
    }
}
