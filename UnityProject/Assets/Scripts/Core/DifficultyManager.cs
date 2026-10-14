using UnityEngine;

namespace BiharSurfers.Core
{
    public enum DifficultyLevel { Easy, Normal, Hard, Extreme }

    /// <summary>
    /// Scales game difficulty automatically as distance increases.
    /// Gradually increases player running speed, obstacle spawn frequency, and combination patterns.
    /// </summary>
    public class DifficultyManager : MonoBehaviour
    {
        public static DifficultyManager Instance { get; private set; }

        [Header("Difficulty State")]
        public DifficultyLevel currentLevel = DifficultyLevel.Easy;
        public float currentDifficultyMultiplier { get; private set; } = 1.0f;

        [Header("Base Speed Scaling")]
        public float baseSpeed = 16f;
        public float maxSpeed = 35f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ResetDifficulty()
        {
            currentLevel = DifficultyLevel.Easy;
            currentDifficultyMultiplier = 1.0f;

            if (Player.PlayerController.Instance != null && Player.PlayerController.Instance.movement != null)
            {
                Player.PlayerController.Instance.movement.forwardSpeed = baseSpeed;
            }
        }

        private void Update()
        {
            if (ScoreManager.Instance == null) return;

            float distance = ScoreManager.Instance.currentDistance;

            // Determine difficulty level & multiplier based on distance run
            if (distance > 2500f)
            {
                currentLevel = DifficultyLevel.Extreme;
                currentDifficultyMultiplier = 2.2f;
            }
            else if (distance > 1200f)
            {
                currentLevel = DifficultyLevel.Hard;
                currentDifficultyMultiplier = 1.7f;
            }
            else if (distance > 500f)
            {
                currentLevel = DifficultyLevel.Normal;
                currentDifficultyMultiplier = 1.3f;
            }
            else
            {
                currentLevel = DifficultyLevel.Easy;
                currentDifficultyMultiplier = 1.0f;
            }

            // Gradually scale forward running speed with distance
            if (Player.PlayerController.Instance != null && Player.PlayerController.Instance.movement != null)
            {
                float targetSpeed = Mathf.Min(maxSpeed, baseSpeed + (distance * 0.005f));
                Player.PlayerController.Instance.movement.forwardSpeed = targetSpeed;
            }
        }
    }
}
