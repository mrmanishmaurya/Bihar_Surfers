using UnityEngine;

namespace BiharSurfers.PowerUps
{
    /// <summary>
    /// Chai Magnet Power-Up: Attracts nearby coins automatically.
    /// </summary>
    public class MagnetPowerUp : PowerUp
    {
        protected override void ActivateEffect()
        {
            if (Player.PlayerController.Instance != null)
            {
                Player.PlayerController.Instance.isMagnetActive = true;
                Invoke(nameof(Deactivate), baseDuration);
            }
        }

        private void Deactivate()
        {
            if (Player.PlayerController.Instance != null)
            {
                Player.PlayerController.Instance.isMagnetActive = false;
            }
        }
    }
}
