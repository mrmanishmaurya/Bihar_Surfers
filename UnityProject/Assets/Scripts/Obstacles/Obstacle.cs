using UnityEngine;

namespace BiharSurfers.Obstacles
{
    public enum ObstacleType
    {
        Barricade,           // Low barrier - Jump over
        Box,                 // Low stack - Jump over
        RailwayBarrier,      // Overhead barrier - Slide under
        Bench,               // Low bench - Jump over
        ConstructionBarrier  // Full blockage - Switch lane
    }

    /// <summary>
    /// Track obstacle behavior supporting low (Jumpable), high (Slidable),
    /// and full blockage (Lane-switch required) collision handling.
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        [Header("Obstacle Configuration")]
        public ObstacleType obstacleType = ObstacleType.Barricade;
        public string obstacleName = "Barricade";
        public float obstacleHeight = 1.2f;
        public bool canJumpOver = true;
        public bool canSlideUnder = false;
        public int laneIndex = 1; // 0 = Left (-2.5), 1 = Center (0), 2 = Right (2.5)

        protected virtual void OnEnable()
        {
            // Reset pooled obstacle state
        }

        public virtual void OnHit()
        {
            Core.AudioManager.Instance?.PlaySFX("Collision");
            Player.PlayerController.Instance?.Die();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<Player.PlayerController>() != null)
            {
                CheckCollisionSafety();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<Player.PlayerController>() != null)
            {
                CheckCollisionSafety();
            }
        }

        private void CheckCollisionSafety()
        {
            var player = Player.PlayerController.Instance;
            if (player == null || !player.isAlive) return;

            // Check if player safely jumps over low obstacle
            if (canJumpOver && player.movement != null && !player.movement.isGrounded && player.transform.position.y > 0.8f)
            {
                // Safely cleared jump obstacle
                return;
            }

            // Check if player safely slides under high barrier
            if (canSlideUnder && player.movement != null && player.movement.isSliding)
            {
                // Safely cleared slide obstacle
                return;
            }

            // Hit obstacle
            player.OnHitObstacle();
        }
    }
}
