using UnityEngine;

namespace BiharSurfers.Coins
{
    public enum CoinPatternType { Straight, JumpArc, ZigZag, LaneShift }

    /// <summary>
    /// Spawns Rupee coins in 4 tactical formation patterns:
    /// Straight Line, Jump Arc (parabola for jumping), Zig-Zag, and Lane Shift across lanes.
    /// </summary>
    public class CoinSpawner : MonoBehaviour
    {
        public static CoinSpawner Instance { get; private set; }

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

        public void SpawnPatternOnSegment(float segmentZPosition, int laneIndex = 1)
        {
            CoinPatternType pattern = (CoinPatternType)Random.Range(0, 4);
            float startX = (laneIndex - 1) * laneWidth;
            Vector3 startPos = new Vector3(startX, 0.6f, segmentZPosition + 2f);

            SpawnPattern(startPos, pattern, 6);
        }

        public void SpawnPattern(Vector3 startPos, CoinPatternType pattern, int count = 6)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = startPos;
                pos.z += i * 2.5f;

                switch (pattern)
                {
                    case CoinPatternType.Straight:
                        // Straight line on lane
                        break;

                    case CoinPatternType.JumpArc:
                        // Jump parabola arc peaking over obstacle height
                        float t = (float)i / (count - 1);
                        pos.y += Mathf.Sin(t * Mathf.PI) * 2.8f;
                        break;

                    case CoinPatternType.ZigZag:
                        pos.x += ((i % 2 == 0) ? -1 : 1) * 1.2f;
                        break;

                    case CoinPatternType.LaneShift:
                        pos.x = Mathf.Lerp(-laneWidth, laneWidth, (float)i / (count - 1));
                        break;
                }

                GameObject coinObj = Track.ObjectPool.Instance?.SpawnFromPool("RupeeCoin", pos, Quaternion.identity);
                if (coinObj == null)
                {
                    // Fallback primitive coin if pool item is absent
                    coinObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    coinObj.name = "RupeeCoin_Primitive";
                    coinObj.transform.position = pos;
                    coinObj.transform.localScale = new Vector3(0.6f, 0.08f, 0.6f);
                    coinObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    coinObj.tag = "Coin";
                    Environment.MaterialFactory.ApplyMaterial(coinObj, Environment.BiharMaterialType.Coin);

                    var col = coinObj.GetComponent<Collider>();
                    if (col != null) col.isTrigger = true;

                    if (coinObj.GetComponent<Coin>() == null)
                    {
                        coinObj.AddComponent<Coin>();
                    }
                }
            }
        }
    }
}
