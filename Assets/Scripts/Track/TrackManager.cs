using System.Collections.Generic;
using UnityEngine;
using BiharSurfers.Obstacles;

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

        public Transform playerTransform;
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
                // Procedural primitive track segment fallback with full Bihar railway styling
                segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segment.name = $"TrackSegment_{spawnZ}";
                float centerZ = spawnZ + segmentLength * 0.5f;
                segment.transform.position = new Vector3(0f, -0.2f, centerZ);
                segment.transform.localScale = new Vector3(8.5f, 0.4f, segmentLength);
                segment.transform.SetParent(transform);
                Environment.MaterialFactory.ApplyMaterial(segment, Environment.BiharMaterialType.TrackGround);

                // Side Grass Strips (Left & Right)
                GameObject leftGrass = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leftGrass.name = "SideGrass_Left";
                leftGrass.transform.position = new Vector3(-8.25f, -0.18f, centerZ);
                leftGrass.transform.localScale = new Vector3(8.0f, 0.42f, segmentLength);
                leftGrass.transform.SetParent(segment.transform);
                Environment.MaterialFactory.ApplyMaterial(leftGrass, Environment.BiharMaterialType.SideGrass);

                GameObject rightGrass = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rightGrass.name = "SideGrass_Right";
                rightGrass.transform.position = new Vector3(8.25f, -0.18f, centerZ);
                rightGrass.transform.localScale = new Vector3(8.0f, 0.42f, segmentLength);
                rightGrass.transform.SetParent(segment.transform);
                Environment.MaterialFactory.ApplyMaterial(rightGrass, Environment.BiharMaterialType.SideGrass);

                // Wooden Railway Sleepers along segment
                for (float zOffset = -segmentLength * 0.5f + 1f; zOffset < segmentLength * 0.5f; zOffset += 2.5f)
                {
                    GameObject tie = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tie.name = "WoodSleeper";
                    tie.transform.position = new Vector3(0f, 0.02f, centerZ + zOffset);
                    tie.transform.localScale = new Vector3(7.8f, 0.05f, 0.5f);
                    tie.transform.SetParent(segment.transform);
                    Environment.MaterialFactory.ApplyMaterial(tie, Environment.BiharMaterialType.WoodSleeper);
                }

                // Steel Rails for 3 Lanes
                for (int lane = 0; lane < 3; lane++)
                {
                    float laneX = (lane - 1) * 2.5f;

                    // Steel Left Rail
                    GameObject railL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    railL.name = $"Rail_L_{lane}";
                    railL.transform.position = new Vector3(laneX - 0.7f, 0.08f, centerZ);
                    railL.transform.localScale = new Vector3(0.12f, 0.12f, segmentLength);
                    railL.transform.SetParent(segment.transform);
                    Environment.MaterialFactory.ApplyMaterial(railL, Environment.BiharMaterialType.SteelRail);

                    // Steel Right Rail
                    GameObject railR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    railR.name = $"Rail_R_{lane}";
                    railR.transform.position = new Vector3(laneX + 0.7f, 0.08f, centerZ);
                    railR.transform.localScale = new Vector3(0.12f, 0.12f, segmentLength);
                    railR.transform.SetParent(segment.transform);
                    Environment.MaterialFactory.ApplyMaterial(railR, Environment.BiharMaterialType.SteelRail);
                }

                // Station Decor Props every 40 meters
                if (Mathf.Abs(spawnZ % 40f) < 0.1f)
                {
                    GameObject signBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    signBoard.name = "StationSign_Patna";
                    signBoard.transform.position = new Vector3(-5.2f, 2.5f, centerZ);
                    signBoard.transform.localScale = new Vector3(0.3f, 1.2f, 3.5f);
                    signBoard.transform.SetParent(segment.transform);
                    Environment.MaterialFactory.ApplyMaterial(signBoard, Environment.BiharMaterialType.StationSign);

                    GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    post.name = "SignPost";
                    post.transform.position = new Vector3(-5.2f, 1.0f, centerZ);
                    post.transform.localScale = new Vector3(0.2f, 1.2f, 0.2f);
                    post.transform.SetParent(segment.transform);
                    Environment.MaterialFactory.ApplyMaterial(post, Environment.BiharMaterialType.SteelRail);
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
