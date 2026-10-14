using UnityEngine;

namespace BiharSurfers.Track
{
    /// <summary>
    /// Represents a single modular track segment (Length, Environment Theme, Spawn Points).
    /// </summary>
    public class TrackSegment : MonoBehaviour
    {
        [Header("Segment Configuration")]
        public float segmentLength = 20f;
        public string poolKey = "DefaultTrackSegment";
        public string environmentTheme = "PatnaJunction";

        [Header("Spawn Markers")]
        public Transform[] obstacleSpawnPoints;
        public Transform[] coinSpawnPoints;
        public Transform[] powerUpSpawnPoints;

        [Header("Decorations")]
        public GameObject[] stationSigns;
        public GameObject[] platformBenches;

        public void SpawnEntities(float difficultyFactor)
        {
            // Reset existing child entities
            DeactivateEntities();

            // Spawn obstacles
            if (obstacleSpawnPoints != null && obstacleSpawnPoints.Length > 0)
            {
                foreach (var point in obstacleSpawnPoints)
                {
                    if (Random.value < 0.45f * difficultyFactor)
                    {
                        string obstacleKey = Random.value < 0.3f ? "TrainTrolley" : "RailwayBarricade";
                        ObjectPool.Instance?.SpawnFromPool(obstacleKey, point.position, point.rotation);
                    }
                }
            }

            // Spawn coins
            if (coinSpawnPoints != null && coinSpawnPoints.Length > 0)
            {
                foreach (var point in coinSpawnPoints)
                {
                    if (Random.value < 0.6f)
                    {
                        ObjectPool.Instance?.SpawnFromPool("RupeeCoin", point.position, Quaternion.identity);
                    }
                }
            }
        }

        public void DeactivateEntities()
        {
            // Return active pooled items attached to segment to pool
        }
    }
}
