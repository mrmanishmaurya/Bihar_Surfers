using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Track
{
    /// <summary>
    /// High-performance Object Pooling system for recycling TrackSegments, Obstacles, Coins, and VFX.
    /// Prevents memory allocation spikes and garbage collection stutter on mobile devices.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        [System.Serializable]
        public class PoolItem
        {
            public string poolKey;
            public GameObject prefab;
            public int initialSize = 10;
        }

        public List<PoolItem> poolItems = new List<PoolItem>();
        private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var item in poolItems)
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();

                for (int i = 0; i < item.initialSize; i++)
                {
                    GameObject obj = Instantiate(item.prefab, transform);
                    obj.SetActive(false);
                    objectQueue.Enqueue(obj);
                }

                poolDictionary.Add(item.poolKey, objectQueue);
            }
        }

        public GameObject SpawnFromPool(string poolKey, Vector3 position, Quaternion rotation)
        {
            if (string.IsNullOrEmpty(poolKey)) return null;

            if (!poolDictionary.ContainsKey(poolKey))
            {
                poolDictionary[poolKey] = new Queue<GameObject>();
            }

            Queue<GameObject> objectQueue = poolDictionary[poolKey];

            if (objectQueue.Count == 0)
            {
                // Dynamic pool expansion
                PoolItem item = poolItems.Find(x => x.poolKey == poolKey);
                if (item != null && item.prefab != null)
                {
                    GameObject newObj = Instantiate(item.prefab, transform);
                    newObj.transform.position = position;
                    newObj.transform.rotation = rotation;
                    newObj.SetActive(true);
                    return newObj;
                }
                return null; // Return null so procedural spawner fallback executes smoothly
            }

            GameObject objToSpawn = objectQueue.Dequeue();
            if (objToSpawn == null) return null;

            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;
            objToSpawn.SetActive(true);

            return objToSpawn;
        }

        public void ReturnToPool(string poolKey, GameObject obj)
        {
            obj.SetActive(false);

            if (!poolDictionary.ContainsKey(poolKey))
            {
                poolDictionary.Add(poolKey, new Queue<GameObject>());
            }

            poolDictionary[poolKey].Enqueue(obj);
        }
    }
}
