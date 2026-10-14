using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Handles physical trigger and collision events with Obstacles, Coins, and PowerUps.
    /// </summary>
    public class PlayerCollision : MonoBehaviour
    {
        private PlayerController playerController;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerController == null || !playerController.isAlive) return;

            // Coin Collision
            if (other.CompareTag("Coin"))
            {
                Coins.Coin coin = other.GetComponent<Coins.Coin>();
                if (coin != null && !coin.isCollected)
                {
                    coin.Collect();
                }
            }
            // PowerUp Collision
            else if (other.CompareTag("PowerUp"))
            {
                PowerUps.PowerUp powerUp = other.GetComponent<PowerUps.PowerUp>();
                if (powerUp != null && !powerUp.isCollected)
                {
                    powerUp.Collect();
                }
            }
            // Obstacle Collision
            else if (other.CompareTag("Obstacle"))
            {
                Obstacles.Obstacle obstacle = other.GetComponent<Obstacles.Obstacle>();
                if (obstacle != null)
                {
                    playerController.OnHitObstacle();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (playerController == null || !playerController.isAlive) return;

            if (collision.gameObject.CompareTag("Obstacle"))
            {
                playerController.OnHitObstacle();
            }
        }
    }
}
