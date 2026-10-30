using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class PhysicsBody : MonoBehaviour
{
    protected Rigidbody rigidbody;
    protected Collider collider;

    #region Unity Messages

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    protected virtual void Start()
    {
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
