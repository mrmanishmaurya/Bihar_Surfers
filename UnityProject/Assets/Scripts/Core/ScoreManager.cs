using UnityEngine;

namespace BiharSurfers.Core
{
    /// <summary>
    /// Tracks current score, distance, high score, and persists local high score.
    /// Score increases based on distance run, coins collected, combos, and multipliers.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public float currentScore { get; private set; } = 0f;
        public float currentDistance { get; private set; } = 0f;
        public int highScore { get; private set; } = 0;

        private Transform playerTransform;
        private Vector3 startPosition;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            LoadHighScore();
        }

        public void ResetScore()
        {
            currentScore = 0f;
            currentDistance = 0f;
            if (Player.PlayerController.Instance != null)
            {
                playerTransform = Player.PlayerController.Instance.transform;
                startPosition = playerTransform.position;
            }
        }

        private void Update()
        {
            if (Player.PlayerController.Instance == null || !Player.PlayerController.Instance.isAlive)
                return;

            if (playerTransform == null)
            {
                playerTransform = Player.PlayerController.Instance.transform;
                startPosition = playerTransform.position;
            }

            // Calculate distance run in meters
            currentDistance = playerTransform.position.z - startPosition.z;

            // Score increases with distance & speed
            float speed = Player.PlayerController.Instance.movement != null ? Player.PlayerController.Instance.movement.forwardSpeed : 16f;
            currentScore += speed * Time.deltaTime;

            // Mission progress
            MissionManager.Instance?.UpdateProgress("m1", (int)(speed * Time.deltaTime));

            if (currentScore > highScore)
            {
                highScore = (int)currentScore;
            }
        }

        public void AddCoinScore(int coinCount, int combo)
        {
            currentScore += coinCount * 10f * Mathf.Min(combo, 5);
        }

        public void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt("BiharSurfers_HighScore", 0);
        }

        public void SaveHighScore()
        {
            if ((int)currentScore > highScore)
            {
                highScore = (int)currentScore;
                PlayerPrefs.SetInt("BiharSurfers_HighScore", highScore);
                PlayerPrefs.Save();
            }
        }
    }
}
