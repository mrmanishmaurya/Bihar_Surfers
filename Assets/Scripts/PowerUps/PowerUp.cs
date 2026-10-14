using UnityEngine;

namespace BiharSurfers.PowerUps
{
    public enum PowerUpType
    {
        Magnet,
        Shield,
        SpeedBoost,
        CoinMultiplier,
        Invincibility
    }

    /// <summary>
    /// Base class for all collectible 3D power-ups (Chai Magnet, Litti Shield, Sattu Rocket, Bihar Boost).
    /// </summary>
    public class PowerUp : MonoBehaviour
    {
        [Header("PowerUp Configuration")]
        public PowerUpType type = PowerUpType.Magnet;
        public string powerUpName = "Power-Up";
        public float baseDuration = 8.0f;
        public bool isCollected = false;

        private void Awake()
        {
            if (!gameObject.CompareTag("PowerUp"))
            {
                gameObject.tag = "PowerUp";
            }
        }

        private void OnEnable()
        {
            isCollected = false;
        }

        private void Update()
        {
            // Floating rotation animation effect
            transform.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);
            float floatY = Mathf.Sin(Time.time * 3.0f) * 0.15f;
            transform.position = new Vector3(transform.position.x, 1.2f + floatY, transform.position.z);
        }

        public virtual void Collect()
        {
            if (isCollected) return;
            isCollected = true;

            ActivateEffect();
            PowerUpManager.Instance?.ActivatePowerUp(type, baseDuration, powerUpName);
            Core.MissionManager.Instance?.UpdateProgress("m4", 1);

            Track.ObjectPool.Instance?.ReturnToPool($"PowerUp_{type}", gameObject);
        }

        protected virtual void ActivateEffect()
        {
            // Overridden by child power-up classes
        }
    }
}
