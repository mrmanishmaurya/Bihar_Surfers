using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Obstacles
{
    /// <summary>
    /// Spawns 3-lane obstacle patterns (Barricade, Box, Railway Barrier, Bench, Construction Barrier)
    /// with difficulty-scaled combinations requiring Jump, Slide, or Lane Switching.
    /// </summary>
    public class ObstacleSpawner : MonoBehaviour
    {
        public static ObstacleSpawner Instance { get; private set; }

        [Header("Obstacle Pool Keys")]
        public string[] lowJumpObstacleKeys = { "Obstacle_Barricade", "Obstacle_Box", "Obstacle_Bench" };
        public string[] highSlideObstacleKeys = { "Obstacle_RailwayBarrier" };
        public string[] fullBlockageObstacleKeys = { "Obstacle_ConstructionBarrier" };

        [Header("Lane Dimensions")]
        public float laneWidth = 2.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SpawnObstaclePatternOnSegment(float segmentZPosition, float difficultyFactor)
        {
            // Calculate number of obstacle rows per segment based on difficulty
            int obstacleRows = difficultyFactor > 1.8f ? 3 : (difficultyFactor > 1.2f ? 2 : 1);
            float rowSpacing = 20.0f / (obstacleRows + 1);

            for (int r = 1; r <= obstacleRows; r++)
            {
                float zPos = segmentZPosition + r * rowSpacing;
                SpawnObstacleRow(zPos, difficultyFactor);
            }
        }

        private void SpawnObstacleRow(float zPosition, float difficultyFactor)
        {
            // Choose pattern type: Single Lane, Double Lane, or Combined Jump/Slide
            float rnd = Random.value;

            // Guarantee at least 1 lane is open for safety
            List<int> availableLanes = new List<int> { 0, 1, 2 };

            if (rnd < 0.60f)
            {
                // Single Lane Obstacle
                int lane = availableLanes[Random.Range(0, availableLanes.Count)];
                SpawnRandomObstacleAtLane(lane, zPosition);
            }
            else if (rnd < 0.90f && difficultyFactor >= 1.2f)
            {
                // Double Lane Obstacles (Leave 1 open lane or 1 jumpable/slidable option)
                int openLane = Random.Range(0, 3);
                for (int l = 0; l < 3; l++)
                {
                    if (l != openLane)
                    {
                        SpawnRandomObstacleAtLane(l, zPosition);
                    }
                }
            }
            else
            {
                // Alternating Jump / Slide combo
                int jumpLane = Random.Range(0, 3);
                int slideLane = (jumpLane + 1) % 3;

                SpawnObstacleAtLane(jumpLane, zPosition, "Obstacle_Barricade");
                SpawnObstacleAtLane(slideLane, zPosition, "Obstacle_RailwayBarrier");
            }
        }

        private void SpawnRandomObstacleAtLane(int laneIndex, float zPosition)
        {
            float typeRnd = Random.value;
            string key;

            if (typeRnd < 0.50f)
            {
                key = lowJumpObstacleKeys[Random.Range(0, lowJumpObstacleKeys.Length)];
            }
            else if (typeRnd < 0.80f)
            {
                key = highSlideObstacleKeys[Random.Range(0, highSlideObstacleKeys.Length)];
            }
            else
            {
                key = fullBlockageObstacleKeys[Random.Range(0, fullBlockageObstacleKeys.Length)];
            }

            SpawnObstacleAtLane(laneIndex, zPosition, key);
        }

        public GameObject SpawnObstacleAtLane(int laneIndex, float zPosition, string obstacleKey)
        {
            float xPos = (laneIndex - 1) * laneWidth;
            Vector3 pos = new Vector3(xPos, 0f, zPosition);

            GameObject obj = Track.ObjectPool.Instance?.SpawnFromPool(obstacleKey, pos, Quaternion.identity);

            if (obj == null)
            {
                // Fallback procedural primitive obstacle if pool is empty
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.name = $"Obstacle_{obstacleKey}";
                obj.tag = "Obstacle";

                Obstacle obsComp = obj.AddComponent<Obstacle>();
                obsComp.laneIndex = laneIndex;

                if (obstacleKey.Contains("RailwayBarrier"))
                {
                    // High barrier (Slide under)
                    obj.transform.position = new Vector3(xPos, 1.8f, zPosition);
                    obj.transform.localScale = new Vector3(2.2f, 0.4f, 0.4f);
                    obsComp.canJumpOver = false;
                    obsComp.canSlideUnder = true;
                    obsComp.obstacleType = ObstacleType.RailwayBarrier;
                }
                else if (obstacleKey.Contains("ConstructionBarrier"))
                {
                    // Full blockage (Switch lane)
                    obj.transform.position = new Vector3(xPos, 1.0f, zPosition);
                    obj.transform.localScale = new Vector3(2.4f, 2.0f, 0.5f);
                    obsComp.canJumpOver = false;
                    obsComp.canSlideUnder = false;
                    obsComp.obstacleType = ObstacleType.ConstructionBarrier;
                }
                else
                {
                    // Low hurdle (Jump over)
                    obj.transform.position = new Vector3(xPos, 0.5f, zPosition);
                    obj.transform.localScale = new Vector3(2.2f, 1.0f, 0.5f);
                    obsComp.canJumpOver = true;
                    obsComp.canSlideUnder = false;
                    obsComp.obstacleType = ObstacleType.Barricade;
                }
            }

            return obj;
        }
    }
}
