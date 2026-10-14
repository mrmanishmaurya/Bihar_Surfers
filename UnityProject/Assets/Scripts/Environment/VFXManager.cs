using UnityEngine;

namespace BiharSurfers.Environment
{
    /// <summary>
    /// High-performance Particle Visual Effects manager for mobile Android.
    /// Handles Coin Sparkle, Running Dust, Jump Dust, Shield Aura, Speed Boost Trail,
    /// Coin Multiplier Sparkle, Collision Impact Burst, and Location Transition Flash.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Particle Systems")]
        public ParticleSystem coinSparkleVFX;
        public ParticleSystem runningDustVFX;
        public ParticleSystem jumpDustVFX;
        public ParticleSystem shieldAuraVFX;
        public ParticleSystem speedBoostTrailVFX;
        public ParticleSystem collisionImpactVFX;
        public ParticleSystem transitionFlashVFX;

        private Transform playerTransform;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (Player.PlayerController.Instance != null)
            {
                playerTransform = Player.PlayerController.Instance.transform;
            }
        }

        private void Update()
        {
            if (playerTransform == null && Player.PlayerController.Instance != null)
            {
                playerTransform = Player.PlayerController.Instance.transform;
            }

            if (playerTransform == null) return;

            // Running Dust Emission Logic
            if (Player.PlayerController.Instance != null && Player.PlayerController.Instance.isAlive)
            {
                bool isGroundRunning = Player.PlayerController.Instance.movement != null && Player.PlayerController.Instance.movement.isGrounded;

                if (runningDustVFX != null)
                {
                    runningDustVFX.transform.position = playerTransform.position;
                    if (isGroundRunning && !runningDustVFX.isPlaying) runningDustVFX.Play();
                    else if (!isGroundRunning && runningDustVFX.isPlaying) runningDustVFX.Stop();
                }

                // Speed Boost Trail positioning
                if (speedBoostTrailVFX != null)
                {
                    speedBoostTrailVFX.transform.position = playerTransform.position + Vector3.up * 1.0f;
                    bool isBoosting = Player.PlayerController.Instance.currentSpeedMultiplier > 1.2f;
                    if (isBoosting && !speedBoostTrailVFX.isPlaying) speedBoostTrailVFX.Play();
                    else if (!isBoosting && speedBoostTrailVFX.isPlaying) speedBoostTrailVFX.Stop();
                }

                // Shield Aura positioning
                if (shieldAuraVFX != null)
                {
                    shieldAuraVFX.transform.position = playerTransform.position + Vector3.up * 1.0f;
                    bool isShielded = Player.PlayerController.Instance.isShielded;
                    if (isShielded && !shieldAuraVFX.isPlaying) shieldAuraVFX.Play();
                    else if (!isShielded && shieldAuraVFX.isPlaying) shieldAuraVFX.Stop();
                }
            }
        }

        public void PlayCoinSparkle(Vector3 position)
        {
            if (coinSparkleVFX != null)
            {
                coinSparkleVFX.transform.position = position;
                coinSparkleVFX.Play();
            }
        }

        public void PlayJumpDust(Vector3 position)
        {
            if (jumpDustVFX != null)
            {
                jumpDustVFX.transform.position = position;
                jumpDustVFX.Play();
            }
        }

        public void PlayCollisionImpact(Vector3 position)
        {
            if (collisionImpactVFX != null)
            {
                collisionImpactVFX.transform.position = position;
                collisionImpactVFX.Play();
            }
        }

        public void PlayTransitionFlash()
        {
            if (transitionFlashVFX != null && Camera.RunnerCamera.Instance != null)
            {
                transitionFlashVFX.transform.position = Camera.RunnerCamera.Instance.transform.position + Camera.RunnerCamera.Instance.transform.forward * 2.0f;
                transitionFlashVFX.Play();
            }
        }
    }
}
