using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionBody))]
public abstract class PhysicsBody : MonoBehaviour
{
    protected Rigidbody rigidbody;
    protected Collider collider;
    
    [Tooltip("Use in case we don't want to simulate this body without disabling it")]
    public bool disablePhysics = false;
    
    // Hard limit to prevent too much loops while raycasting the trajectory
    protected const int MAX_CAST_ITERATIONS = 100;

    #region Unity Messages

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    protected virtual void Start()
    {
        // Do this in Start so the PhysicsSystem singleton is initialized during Awake 
        PhysicsSystem.Instance.AddBody(this);
    }

    protected virtual void OnDisable()
    {
        PhysicsSystem.Instance.RemoveBody(this);
    }
    
    protected virtual void Reset()
    {
        // Ensure we have a Rigibody properly configurated when we add this script to a GameObject
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    
    #endregion

    /// <summary>
    /// Behaviour of this body (moving, rotating...) since previous physics frame
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void PhysicsTick(float deltaTime)
    {
        
    }


}
