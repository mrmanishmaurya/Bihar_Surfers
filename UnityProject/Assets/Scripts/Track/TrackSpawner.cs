using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Track
{
    /// <summary>
    /// Handles procedural segment selection, backdrop decor placement,
    /// and 3-lane alignment calculation.
    /// </summary>
    public class TrackSpawner : MonoBehaviour
    {
        public static TrackSpawner Instance { get; private set; }

        [Header("Lane Dimensions")]
        public float laneWidth = 2.5f;
        public float defaultSegmentLength = 20f;

        [Header("Track Segment Pool Keys")]
        public string[] availableSegmentKeys = {
            "TrackSegment_StationPlatform",
            "TrackSegment_RailwayCrossing",
            "TrackSegment_Overbridge"
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public GameObject SpawnNextSegment(float spawnZPosition, bool spawnObstacles = true)
        {
            string key = availableSegmentKeys.Length > 0 ?
                availableSegmentKeys[Random.Range(0, availableSegmentKeys.Length)] :
                "TrackSegment_StationPlatform";

            GameObject segmentObj = ObjectPool.Instance?.SpawnFromPool(key, new Vector3(0f, 0f, spawnZPosition), Quaternion.identity);

            if (segmentObj == null)
            {
                // Fallback procedural primitive track ground if pool is empty
                segmentObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segmentObj.name = "ProceduralTrackSegment";
                segmentObj.transform.position = new Vector3(0f, -0.5f, spawnZPosition + defaultSegmentLength * 0.5f);
                segmentObj.transform.localScale = new Vector3(laneWidth * 3f + 1f, 1f, defaultSegmentLength);
            }

            TrackSegment segment = segmentObj.GetComponent<TrackSegment>();
            if (segment != null && spawnObstacles)
            {
                float difficultyFactor = Core.DifficultyManager.Instance != null ? Core.DifficultyManager.Instance.currentDifficultyMultiplier : 1.0f;
                segment.SpawnEntities(difficultyFactor);
            }

            return segmentObj;
        }

        public Vector3 GetLanePosition(int laneIndex, float zPos, float yPos = 0f)
        {
            // laneIndex: 0 = Left (-2.5), 1 = Center (0), 2 = Right (+2.5)
            float xPos = (laneIndex - 1) * laneWidth;
            return new Vector3(xPos, yPos, zPos);
        }
    }
}
