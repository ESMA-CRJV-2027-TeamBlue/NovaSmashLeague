using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
    // Singleton
    public static PhysicsSystem Instance;
    
    private List<PhysicsBody> _bodies = new();

    private void Awake()
    {
        // Singleton initialisation
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        // Send physics frame data to all bodies so they can move accordingly
        foreach (PhysicsBody body in _bodies)
        {
            body.PhysicsTick(Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Adds a body to the simulation. If the body is already registered, does nothing.
    /// </summary>
    /// <param name="body">The PhysicsBody to add to the simulation</param>
    public void AddBody(PhysicsBody body)
    {
        if (_bodies.Contains(body)) return;
        
        _bodies.Add(body);
    }
    
    /// <summary>
    /// Removes a body from the simulation. If the body is not registered, does nothing.
    /// </summary>
    /// <param name="body">The PhysicsBody to remove to the simulation</param>
    public void RemoveBody(PhysicsBody body)
    {
        if (!_bodies.Contains(body)) return;
        
        _bodies.Remove(body);
    }

    /// <summary>
    /// Cleans a Vector3 by setting its components to 0 if their absolute value are below the given threshold.
    /// Used to account for Unity's Vector3 float imprecisions
    /// </summary>
    /// <param name="vector3">The Vector3 to clean</param>
    /// <param name="threshold">The threshold used to cut the components values</param>
    /// <returns></returns>
    public static Vector3 CleanVector3(Vector3 vector3, float threshold = 0.001f)
    {
        if (Mathf.Abs(vector3.x) < threshold) vector3.x = 0f;
        if (Mathf.Abs(vector3.y) < threshold) vector3.y = 0f;
        if (Mathf.Abs(vector3.z) < threshold) vector3.z = 0f;

        return vector3;
    }
}
