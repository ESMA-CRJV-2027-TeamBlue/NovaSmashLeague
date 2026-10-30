using UnityEngine;

public class DynamicBody : PhysicsBody
{
    [Header("Behaviour")]
    [Tooltip("")]
    public bool isBouncing = true;
    
    [Tooltip("The direction of this body")]
    public Vector3 movementDirection;
    [Tooltip("The speed of this body, in unit/second")]
    public float speed = 5f;

    protected float halfExtent;
    protected Vector3 currentFramePosition;
    protected float remainingFrameDistance;

    protected override void Awake()
    {
        base.Awake();
        currentFramePosition = transform.position;
        halfExtent = collider.bounds.extents.x;
    }
    
    /// <summary>
    /// Move the body in its direction at its current speed. Collide with other bodies.
    /// </summary>
    /// <param name="deltaTime">The time ellapsed since last physics frame</param>
    public override void PhysicsTick(float deltaTime)
    {
        remainingFrameDistance = speed * deltaTime;
        while (remainingFrameDistance > 0f)
        {
            Vector3 nextMovement = movementDirection.normalized * remainingFrameDistance;
            if(Physics.SphereCast(currentFramePosition, halfExtent, movementDirection.normalized, out RaycastHit hit, remainingFrameDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (isBouncing)
                {
                    Bounce(hit);
                }
                else
                {
                    Slide(hit);
                }
            }
            else
            {
                currentFramePosition += nextMovement;
                remainingFrameDistance = 0f;
            }
        }
        transform.position = currentFramePosition;
    }

    /// <summary>
    /// Make the body bounce off from a hit.
    /// Assumes a SphereCollider.
    /// </summary>
    /// /// <param name="hit">The RaycastHit to bounce from</param>
    public void Bounce(RaycastHit hit)
    {
        Vector3 normal = PhysicsSystem.CleanVector3(hit.normal).normalized;
        currentFramePosition = hit.point + (normal * halfExtent);
        remainingFrameDistance -= hit.distance;
        Vector3 reflect = PhysicsSystem.CleanVector3(Vector3.Reflect(movementDirection, hit.normal)).normalized;
        movementDirection = reflect;
    }
    
    /// <summary>
    /// Make the body slide off from a hit.
    /// Assumes a SphereCollider.
    /// </summary>
    /// /// <param name="hit">The RaycastHit to slide from</param>
    public void Slide(RaycastHit hit)
    {
        Vector3 normal = PhysicsSystem.CleanVector3(hit.normal).normalized;
        currentFramePosition = hit.point + (normal * halfExtent);
        remainingFrameDistance -= hit.distance;
        Vector3 project = PhysicsSystem.CleanVector3(Vector3.ProjectOnPlane(movementDirection, hit.normal)).normalized;
        movementDirection = project;
    }
}
