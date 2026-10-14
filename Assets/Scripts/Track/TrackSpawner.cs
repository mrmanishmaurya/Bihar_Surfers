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
                // Procedural dark railway track segment with full 3-lane Bihar railway visuals
                segmentObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segmentObj.name = $"ProceduralTrackSegment_{spawnZPosition}";
                float centerZ = spawnZPosition + defaultSegmentLength * 0.5f;
                segmentObj.transform.position = new Vector3(0f, -0.2f, centerZ);
                segmentObj.transform.localScale = new Vector3(8.5f, 0.4f, defaultSegmentLength);
                Environment.MaterialFactory.ApplyMaterial(segmentObj, Environment.BiharMaterialType.TrackGround);

                // Left Grass Embankment
                GameObject leftGrass = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leftGrass.name = "SideGrass_Left";
                leftGrass.transform.position = new Vector3(-8.25f, -0.18f, centerZ);
                leftGrass.transform.localScale = new Vector3(8.0f, 0.42f, defaultSegmentLength);
                leftGrass.transform.SetParent(segmentObj.transform);
                Environment.MaterialFactory.ApplyMaterial(leftGrass, Environment.BiharMaterialType.SideGrass);

                // Right Grass Embankment
                GameObject rightGrass = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rightGrass.name = "SideGrass_Right";
                rightGrass.transform.position = new Vector3(8.25f, -0.18f, centerZ);
                rightGrass.transform.localScale = new Vector3(8.0f, 0.42f, defaultSegmentLength);
                rightGrass.transform.SetParent(segmentObj.transform);
                Environment.MaterialFactory.ApplyMaterial(rightGrass, Environment.BiharMaterialType.SideGrass);

                // Wooden Railway Sleepers
                for (float zOffset = -defaultSegmentLength * 0.5f + 1f; zOffset < defaultSegmentLength * 0.5f; zOffset += 2.5f)
                {
                    GameObject tie = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tie.name = "WoodSleeper";
                    tie.transform.position = new Vector3(0f, 0.02f, centerZ + zOffset);
                    tie.transform.localScale = new Vector3(7.8f, 0.05f, 0.5f);
                    tie.transform.SetParent(segmentObj.transform);
                    Environment.MaterialFactory.ApplyMaterial(tie, Environment.BiharMaterialType.WoodSleeper);
                }

                // Steel Rails for 3 Lanes (LEFT, CENTER, RIGHT)
                for (int lane = 0; lane < 3; lane++)
                {
                    float laneX = (lane - 1) * laneWidth;

                    GameObject railL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    railL.name = $"Rail_L_{lane}";
                    railL.transform.position = new Vector3(laneX - 0.7f, 0.08f, centerZ);
                    railL.transform.localScale = new Vector3(0.12f, 0.12f, defaultSegmentLength);
                    railL.transform.SetParent(segmentObj.transform);
                    Environment.MaterialFactory.ApplyMaterial(railL, Environment.BiharMaterialType.SteelRail);

                    GameObject railR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    railR.name = $"Rail_R_{lane}";
                    railR.transform.position = new Vector3(laneX + 0.7f, 0.08f, centerZ);
                    railR.transform.localScale = new Vector3(0.12f, 0.12f, defaultSegmentLength);
                    railR.transform.SetParent(segmentObj.transform);
                    Environment.MaterialFactory.ApplyMaterial(railR, Environment.BiharMaterialType.SteelRail);
                }

                // Station Signboard & Decor every 40m
                if (Mathf.Abs(spawnZPosition % 40f) < 0.1f)
                {
                    GameObject signBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    signBoard.name = "StationSign_Patna";
                    signBoard.transform.position = new Vector3(-5.2f, 2.5f, centerZ);
                    signBoard.transform.localScale = new Vector3(0.3f, 1.2f, 3.5f);
                    signBoard.transform.SetParent(segmentObj.transform);
                    Environment.MaterialFactory.ApplyMaterial(signBoard, Environment.BiharMaterialType.StationSign);

                    GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    post.name = "SignPost";
                    post.transform.position = new Vector3(-5.2f, 1.0f, centerZ);
                    post.transform.localScale = new Vector3(0.2f, 1.2f, 0.2f);
                    post.transform.SetParent(segmentObj.transform);
                    Environment.MaterialFactory.ApplyMaterial(post, Environment.BiharMaterialType.SteelRail);

                    // Platform structure
                    GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platform.name = "StationPlatform";
                    platform.transform.position = new Vector3(5.5f, 0.4f, centerZ);
                    platform.transform.localScale = new Vector3(2.5f, 0.8f, defaultSegmentLength);
                    platform.transform.SetParent(segmentObj.transform);
                    Environment.MaterialFactory.ApplyMaterial(platform, Environment.BiharMaterialType.Platform);
                }
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
