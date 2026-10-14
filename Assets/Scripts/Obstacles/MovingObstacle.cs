using UnityEngine;

namespace BiharSurfers.Obstacles
{
    /// <summary>
    /// Derived obstacle for moving objects such as oncoming train trolleys,
    /// traffic barricades, and moving road vehicles.
    /// </summary>
    public class MovingObstacle : Obstacle
    {
        [Header("Movement Settings")]
        public float moveSpeed = 8f;
        public Vector3 moveDirection = Vector3.back; // Moves toward the player

        private void Update()
        {
            if (Player.PlayerController.Instance == null || !Player.PlayerController.Instance.isAlive)
                return;

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
