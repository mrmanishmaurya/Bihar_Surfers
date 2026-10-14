using UnityEngine;

namespace BiharSurfers.Coins
{
    /// <summary>
    /// Collectible golden Rupee coin logic (continuous rotation, magnet attraction, pickup audio & VFX).
    /// </summary>
    public class Coin : MonoBehaviour
    {
        [Header("Coin Settings")]
        public int coinValue = 1;
        public float rotationSpeed = 120f;
        public float magnetAttractionSpeed = 15f;
        public bool isCollected = false;

        private Transform playerTransform;

        private void OnEnable()
        {
            isCollected = false;
            if (Player.PlayerController.Instance != null)
            {
                playerTransform = Player.PlayerController.Instance.transform;
            }
        }

        private void Update()
        {
            // Rotation animation
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Magnet Power-Up Attraction Logic
            if (!isCollected && Player.PlayerController.Instance != null && Player.PlayerController.Instance.isMagnetActive)
            {
                if (playerTransform == null) playerTransform = Player.PlayerController.Instance.transform;

                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance < 10f)
                {
                    transform.position = Vector3.Lerp(transform.position, playerTransform.position + Vector3.up * 0.8f, Time.deltaTime * magnetAttractionSpeed);
                }
            }
        }

        public void Collect()
        {
            if (isCollected) return;
            isCollected = true;

            CoinManager.Instance?.AddCoins(coinValue);
            Core.AudioManager.Instance?.PlaySFX("CoinPickup");

            // Return to pool or deactivate
            Track.ObjectPool.Instance?.ReturnToPool("RupeeCoin", gameObject);
        }
    }
}
