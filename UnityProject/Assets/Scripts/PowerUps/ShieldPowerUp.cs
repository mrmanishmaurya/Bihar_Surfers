using UnityEngine;

namespace BiharSurfers.PowerUps
{
    /// <summary>
    /// Litti Shield Power-Up: Protects player from one obstacle collision.
    /// </summary>
    public class ShieldPowerUp : PowerUp
    {
        protected override void ActivateEffect()
        {
            if (Player.PlayerController.Instance != null)
            {
                Player.PlayerController.Instance.isShielded = true;
            }
        }
    }
}
