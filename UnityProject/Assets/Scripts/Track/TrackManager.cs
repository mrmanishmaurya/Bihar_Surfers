using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Track
{
    /// <summary>
    /// Manages continuous procedural 3-lane endless track segment generation,
    /// recycling old segments behind the player using ObjectPool, and guaranteeing
    /// gapless alignment.
    /// </summary>
    public class TrackManager : MonoBehaviour
    {
        public static TrackManager Instance { get; private set; }

        [Header("Track Configuration")]
        public int activeSegmentBuffer = 8;
        public float segmentLength = 20f;
        public float recycleDistanceBehindPlayer = 30f;

        private Transform playerTransform;
        private List<GameObject> activeSegments = new List<GameObject>();
        private float spawnZ = 0f;

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

            InitTrack();
        }

        public void InitTrack()
        {
            // Clear existing active segments
            foreach (var seg in activeSegments)
            {
                if (seg != null)
                {
                    TrackSegment segComp = seg.GetComponent<TrackSegment>();
                    string poolKey = segComp != null ? segComp.poolKey : "TrackSegment_StationPlatform";
                    ObjectPool.Instance?.ReturnToPool(poolKey, seg);
                }
            }
            activeSegments.Clear();
            spawnZ = 0f;

            // Spawn initial safe segments without obstacles for the first 3 segments
            for (int i = 0; i < activeSegmentBuffer; i++)
            {
                SpawnSegment(i >= 3);
            }
        }

        private void Update()
        {
            if (playerTransform == null && Player.PlayerController.Instance != null)
            {
                playerTransform = Player.PlayerController.Instance.transform;
            }

            if (playerTransform == null || activeSegments.Count == 0) return;

            // Check if player has advanced past the first active segment
            if (playerTransform.position.z - recycleDistanceBehindPlayer > (activeSegments[0].transform.position.z + segmentLength))
            {
                RecycleOldestSegment();
                SpawnSegment(true);
            }
        }

        private void SpawnSegment(bool spawnObstacles = true)
        {
            GameObject segment = null;

            if (TrackSpawner.Instance != null)
            {
                segment = TrackSpawner.Instance.SpawnNextSegment(spawnZ, spawnObstacles);
            }

            if (segment == null && ObjectPool.Instance != null)
            {
                segment = ObjectPool.Instance.SpawnFromPool("TrackSegment_StationPlatform", new Vector3(0f, 0f, spawnZ), Quaternion.identity);
            }

            if (segment == null)
            {
                // Procedural primitive track segment fallback for instant test execution
                segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segment.name = $"TrackSegment_{spawnZ}";
                segment.transform.position = new Vector3(0f, -0.2f, spawnZ + segmentLength * 0.5f);
                segment.transform.localScale = new Vector3(8.0f, 0.4f, segmentLength);
                segment.transform.SetParent(transform);

                // Add test track lines
                for (int lane = 0; lane < 3; lane++)
                {
                    float laneX = (lane - 1) * 2.5f;
                    GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    line.transform.position = new Vector3(laneX, 0.01f, spawnZ + segmentLength * 0.5f);
                    line.transform.localScale = new Vector3(0.2f, 0.02f, segmentLength);
                    line.transform.SetParent(segment.transform);
                }
            }

            activeSegments.Add(segment);

            TrackSegment segComp = segment.GetComponent<TrackSegment>();
            if (segComp != null)
            {
                segmentLength = segComp.segmentLength;
            }

            // Spawn obstacles if enabled
            if (spawnObstacles && ObstacleSpawner.Instance != null)
            {
                float difficulty = Core.DifficultyManager.Instance != null ? Core.DifficultyManager.Instance.currentDifficultyMultiplier : 1.0f;
                ObstacleSpawner.Instance.SpawnObstaclePatternOnSegment(spawnZ, difficulty);
            }

            spawnZ += segmentLength;
        }

        private void RecycleOldestSegment()
        {
            if (activeSegments.Count > 0)
            {
                GameObject oldest = activeSegments[0];
                activeSegments.RemoveAt(0);

                TrackSegment segComp = oldest.GetComponent<TrackSegment>();
                if (segComp != null)
                {
                    ObjectPool.Instance?.ReturnToPool(segComp.poolKey, oldest);
                }
                else
                {
                    Destroy(oldest);
                }
            }
        }
    }
}
