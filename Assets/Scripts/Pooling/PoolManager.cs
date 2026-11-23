using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Tooltip("The prefab to manage with a pool")]
    public GameObject pooledPrefab;
    [Tooltip("Size of the pool")]
    public int poolSize = 10;
    
    private List<GameObject> _pool = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newInstance = Instantiate(pooledPrefab);
            _pool.Add(newInstance);
            newInstance.SetActive(false);
        }
    }

    /// <summary>
    /// Creates or activate an instance of the prefab to pool.
    /// If the pool is full of active instances, get a random instance.
    /// Instance must be deactivated to be available in the pool again.
    /// </summary>
    /// <returns>An available prefab instance</returns>
    public GameObject PoolInstance()
    {
        // Check if an object is available in the pool
        foreach (var pooledObject in _pool)
        {
            if (!pooledObject.activeInHierarchy)
            {
                pooledObject.SetActive(true);
                return pooledObject;
            }
        }
        
        return _pool[Random.Range(0, _pool.Count)];
    }
}
