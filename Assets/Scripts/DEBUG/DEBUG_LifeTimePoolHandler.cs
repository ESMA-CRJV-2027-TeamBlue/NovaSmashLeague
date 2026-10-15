using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DEBUG_LifeTimePoolHandler : MonoBehaviour
{
    [Tooltip("The pool manager to debug")]
    public PoolManager poolManager;
    [Tooltip("The time to live of pooled objects")]
    public int lifespan = 1;
    
    public void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            Spawn();
        }
    }

    /// <summary>
    /// Pool an object then deactivate it after the time to live
    /// </summary>
    public void Spawn()
    {
        GameObject instance = poolManager.PoolInstance();
        StartCoroutine(HandleInstance(instance));
    }

    /// <summary>
    /// Places an instance on a random spot, and deactivates it after the time to live
    /// </summary>
    /// <param name="instance">The instance to handle</param>
    /// <returns>Coroutine</returns>
    private IEnumerator HandleInstance(GameObject instance)
    {
        instance.transform.SetParent(transform);
        instance.transform.position = Vector3.right * Random.Range(-2f, 2f) +  Vector3.up * Random.Range(-2f, 2f) +  Vector3.forward * Random.Range(-2f, 2f);
        yield return new WaitForSeconds(lifespan);
        instance.SetActive(false);
    }
}
