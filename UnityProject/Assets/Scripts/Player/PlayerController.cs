using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Master controller for the Bihar Surfers player character.
    /// Coordinates movement, input, collisions, powerups, and animations.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerAnimationController))]
    [RequireComponent(typeof(PlayerCollision))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Sub-Components")]
        public PlayerMovement movement;
        public PlayerInput input;
        public PlayerAnimationController animationController;
        public PlayerCollision collision;

        [Header("State Flags")]
        public bool isAlive = true;
        public bool isShielded = false;
        public bool isMagnetActive = false;
        public bool isInvincible = false;
        public float currentSpeedMultiplier = 1.0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            movement = GetComponent<PlayerMovement>();
            input = GetComponent<PlayerInput>();
            animationController = GetComponent<PlayerAnimationController>();
            collision = GetComponent<PlayerCollision>();
        }

        private void Start()
        {
            ResetPlayerState();
        }

        public void ResetPlayerState()
        {
            isAlive = true;
            isShielded = false;
            isMagnetActive = false;
            isInvincible = false;
            currentSpeedMultiplier = 1.0f;

            if (movement != null) movement.ResetPosition();
            if (animationController != null) animationController.PlayRun();
        }

        public void ResetPlayer() => ResetPlayerState();

        public void OnLaneChange(int targetLane)
        {
            if (!isAlive) return;
            movement.ChangeLane(targetLane);
        }

        public void OnJump()
        {
            if (!isAlive) return;
            if (movement.Jump())
            {
                animationController.PlayJump();
                Core.AudioManager.Instance?.PlaySFX("Jump");
            }
        }

        public void OnSlide()
        {
            if (!isAlive) return;
            if (movement.Slide())
            {
                animationController.PlaySlide();
                Core.AudioManager.Instance?.PlaySFX("Slide");
            }
        }

        public void OnHitObstacle()
        {
            if (isInvincible) return;

            if (isShielded)
            {
                isShielded = false;
                Core.AudioManager.Instance?.PlaySFX("ShieldBreak");
                // Trigger shield hit particle/feedback
                return;
            }

            Die();
        }

        public void Die()
        {
            if (!isAlive) return;
            isAlive = false;

            movement.StopMovement();
            animationController.PlayDeath();
            Core.AudioManager.Instance?.PlaySFX("Collision");
            Core.AudioManager.Instance?.PlaySFX("GameOver");

            Core.GameManager.Instance?.TriggerGameOver();
        }

        public void Revive()
        {
            isAlive = true;
            isInvincible = true;
            movement.ResumeMovement();
            animationController.PlayRun();
            Invoke(nameof(DisableInvincibility), 3.0f);
        }

        private void DisableInvincibility()
        {
            isInvincible = false;
        }
    }
}
