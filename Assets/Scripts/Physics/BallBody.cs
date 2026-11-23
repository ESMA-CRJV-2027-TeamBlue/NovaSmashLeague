using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallBody : PhysicsBody
{
    [Header("Behaviour")]
    [Tooltip("The current direction of the ball")]
    public Vector3 movementDirection;
    [Tooltip("The current speed of the ball, in unit/second")]
    public float speed;
    [Tooltip("The current damage of the ball")]
    public float damage;
    [Header("Data")]
    [Tooltip("The data for the ball behaviour")]
    public SO_BallPreset ballPreset;
    
    private SO_BallSpeedLevel _currentSpeedLevel;
    private int _currentSpeedIndex;
    
    private Coroutine _impactCoroutine;
    private Coroutine _decayCoroutine;
    
    [Tooltip("The current velocity applied to the ball in world coordinates (a Vector3 in unit/second")]
    public Vector3 velocity => movementDirection * speed;

    private float halfExtent;
    private Vector3 currentFramePosition;
    private float remainingFrameDistance;

    [Header("Events / feedbacks")]
    [Tooltip("Emitted when the ball collides with something. Gives the CollisionBody we collided with, and the RaycastHit of the collision.")]
    public UnityEvent<CollisionBody, RaycastHit> OnBodyCollision;
    [Tooltip("Emitted when the ball changes its speed level. Gives the current speed level and the maximum (starts at 0).")]
    public UnityEvent<int, int> OnSpeedLevelChanged;
    [Tooltip("Emitted when the ball start its impact phase")]
    public UnityEvent OnImpactStarted;
    [Tooltip("Emitted when the ball ends its impact phase")]
    public UnityEvent OnImpactEnded;
    [Tooltip("Emitted when the ball decays its speed level")]
    public UnityEvent OnDecay;
    [Tooltip("Emitted when the ball bounces somewhere. Gives the world position of the bounce")]
    public UnityEvent<Vector3> OnBounce;
    
    protected override void Awake()
    {
        base.Awake();
        currentFramePosition = transform.position;
        halfExtent = collider.bounds.extents.x;
    }

    protected override void Start()
    {
        base.Start();
        ChangeSpeedLevel(0);
    }

    /// <summary>
    /// Move the ball in its current direction and collide with other game elements
    /// </summary>
    /// <param name="deltaTime">The time ellapsed since last physics frame</param>
    public override void PhysicsTick(float deltaTime)
    {
        remainingFrameDistance = speed * deltaTime;
        int currentIteration = 0;
        while (remainingFrameDistance > 0f && currentIteration++ < MAX_CAST_ITERATIONS)
        {
            Vector3 nextMovement = movementDirection.normalized * remainingFrameDistance;
            if(Physics.SphereCast(currentFramePosition, halfExtent, movementDirection.normalized, out RaycastHit hit, remainingFrameDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if(hit.collider.TryGetComponent(out CollisionBody body))
                {
                    OnBodyCollision?.Invoke(body, hit);
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
    /// Make the ball bounce off from a hit.
    /// </summary>
    /// /// <param name="hit">The RaycastHit to bounce from</param>
    public void Bounce(RaycastHit hit)
    {
        Vector3 normal = PhysicsSystem.CleanVector3(hit.normal).normalized;
        currentFramePosition = hit.point + (normal * halfExtent);
        remainingFrameDistance -= hit.distance;
        Vector3 reflect = PhysicsSystem.CleanVector3(Vector3.Reflect(movementDirection, hit.normal)).normalized;
        movementDirection = reflect;
        
        OnBounce?.Invoke(hit.point);
    }

    /// <summary>
    /// Snaps the ball to a collision point for the remaining of the frame
    /// End the movement but does not change the speed
    /// </summary>
    /// <param name="hit">The RaycastHit to snap at</param>
    public void Snap(RaycastHit hit)
    {
        Vector3 normal = PhysicsSystem.CleanVector3(hit.normal).normalized;
        currentFramePosition = hit.point + (normal * halfExtent);
        remainingFrameDistance = 0;
    }

    /// <summary>
    /// Change the speed level index
    /// Cannot go outside the range of available speedLevels
    /// If called for a hit, should be paired with <see cref="Impact"/> and <see cref="Decay"/>
    /// </summary>
    /// <param name="delta">How much to add or subtract from the current speed level index</param>
    public void ChangeSpeedLevel(int delta)
    {
        _currentSpeedIndex = Math.Clamp(_currentSpeedIndex + delta, 0, ballPreset.speedLevels.Count - 1);
        _currentSpeedLevel = ballPreset.speedLevels[_currentSpeedIndex];
        speed = ballPreset.baseSpeed * _currentSpeedLevel.cruiseSpeed;
        damage =  ballPreset.baseDamage * _currentSpeedLevel.cruiseDamage;
        
        OnSpeedLevelChanged?.Invoke(_currentSpeedIndex, ballPreset.speedLevels.Count);
    }

    /// <summary>
    /// Forces a new direction for the ball movement
    /// </summary>
    /// <param name="newDirection">The new direction in world coordinates</param>
    public void ChangeDirection(Vector3 newDirection)
    {
        movementDirection = newDirection;
    }

    /// <summary>
    /// Starts the impact phase for the ball
    /// Handles the <see cref="ImpactCoroutine"/>
    /// </summary>
    public void Impact()
    {
        // Stop previous coroutine if needed
        if (_impactCoroutine != null)
        {
            StopCoroutine(_impactCoroutine);
            _impactCoroutine = null;
        }
        _impactCoroutine =  StartCoroutine(ImpactCoroutine());
    }

    /// <summary>
    /// Behaviour of the impact phase for the ball
    /// Changes the speed and damage during the impact phase, and after the impact phase
    /// </summary>
    /// <returns>A Coroutine</returns>
    private IEnumerator ImpactCoroutine()
    {
        speed = ballPreset.baseSpeed * _currentSpeedLevel.impactSpeed;
        damage = ballPreset.baseDamage * _currentSpeedLevel.impactDamage;
        OnImpactStarted?.Invoke();
        yield return new WaitForSeconds(_currentSpeedLevel.impactDuration);
        speed = ballPreset.baseSpeed * _currentSpeedLevel.cruiseSpeed;
        damage = ballPreset.baseDamage * _currentSpeedLevel.cruiseDamage;
        OnImpactEnded?.Invoke();
    }
    
    /// <summary>
    /// Starts the decay for the ball
    /// Handles the <see cref="DecayCoroutine"/>
    /// </summary>
    public void Decay()
    {
        // Stop previous coroutine if needed
        if (_decayCoroutine != null)
        {
            StopCoroutine(_decayCoroutine);
            _decayCoroutine = null;
        }
        _decayCoroutine =  StartCoroutine(DecayCoroutine());
    }

    /// <summary>
    /// Makes the ball lose one speed level after a time not being touched
    /// </summary>
    /// <returns>A Coroutine</returns>
    private IEnumerator DecayCoroutine()
    {
        yield return new WaitForSeconds(_currentSpeedLevel.decayDuration);
        ChangeSpeedLevel(-1);
        OnDecay?.Invoke();
        // Rolling decay
        Decay();
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying || speed==0)
        {
            return;
        }
        var copy_remainingFrameDistance = speed * Time.deltaTime;
        var copy_movementDirection = movementDirection;
        var copy_currentFramePosition = currentFramePosition;
        var trajectorySteps = new List<Vector3>(){copy_currentFramePosition};
        var bounceSpots = new List<Vector3>();
        int currentIteration = 0;
        while (copy_remainingFrameDistance > 0f && currentIteration++ < MAX_CAST_ITERATIONS)
        {
            Vector3 nextMovement = copy_movementDirection.normalized * copy_remainingFrameDistance;
            if(Physics.SphereCast(copy_currentFramePosition, halfExtent, copy_movementDirection.normalized, out RaycastHit hit, copy_remainingFrameDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if(hit.collider.TryGetComponent(out CollisionBody body))
                {
                    // Copy of Bounce
                    Vector3 normal = PhysicsSystem.CleanVector3(hit.normal).normalized;
                    copy_currentFramePosition = hit.point + (normal * halfExtent);
                    copy_remainingFrameDistance -= hit.distance;
                    Vector3 reflect = PhysicsSystem.CleanVector3(Vector3.Reflect(copy_movementDirection, hit.normal)).normalized;
                    copy_movementDirection = reflect;
                    trajectorySteps.Add(copy_currentFramePosition);
                    bounceSpots.Add(copy_currentFramePosition);
                }
            }
            else
            {
                copy_currentFramePosition += nextMovement;
                copy_remainingFrameDistance = 0f;
            }
        }

        var lastStep = trajectorySteps[0];
        trajectorySteps.RemoveAt(0);
        
        Gizmos.color = Color.yellow;
        foreach (var trajectoryStep in trajectorySteps)
        {
            Gizmos.DrawRay(lastStep, trajectoryStep);
            lastStep = trajectoryStep;
        }
        Gizmos.color = Color.orange;
        foreach (var spot in bounceSpots)
        {
            Gizmos.DrawWireSphere(spot, halfExtent);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(copy_currentFramePosition, halfExtent);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10f, 10f + 0 * 20f, 800f, 22f), ""+movementDirection);
    }
}
