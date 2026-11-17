using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBody : PhysicsBody
{
    // Data needed for movement
    private Vector2 _currentGroundVelocity;
    private float _currentVerticalSpeed;
    private Vector3 _currentFramePosition;
    private Vector3 _currentFrameMovementRemaining;
    // Array to avoid creating new objects every frame
    private RaycastHit[] _capsuleHits = new RaycastHit[10];
    private Collider[] _overlapCapsuleHits = new Collider[10];
    // How many objects we collided with last cast
    private int _nbCaspuleHits;
    
    // Indications of how the body should react
    [Header("Behaviour")]
    public Vector2 localGroundDirection;
    [Tooltip("Indicates to the body it should jump whenever possible")]
    public bool shouldJump;
    [Tooltip("Indicates to the body it should extrajump whenever possible")]
    public bool shouldExtraJump;
    [Tooltip("Indicates to the body it should float whenever possible")]
    public bool shouldFloat;
    private bool _onGround;
    private int _extraJumpsRemaining;
    
    // Properties used to facilitate physics casts
    private Vector3 _capsulePoint1 => _currentFramePosition - Vector3.up * (_extentY - _extentX);
    private Vector3 _capsulePoint2 => _currentFramePosition + Vector3.up * (_extentY - _extentX);
    private float _extentX;
    private float _extentY;
   
    [Tooltip("The metrics used for tweaking the movements")]
    public PlayerMovementMetricsSO movementMetrics;
    [Tooltip("Extra space used for raycasts to prevent interpenetrating a wall")]
    [SerializeField] private float _skinWidth = 0.01f;
    [Tooltip("Length of the raycast to know we are touching ground")]
    [SerializeField] private float _groundCheckLength = 0.1f;

    
    // Events
    public UnityEvent OnJumped;
    public UnityEvent OnExtraJumped;

    protected override void Start()
    {
        base.Start();
        _extentX = collider.bounds.extents.x;
        _extentY = collider.bounds.extents.y;
    }

    public override void PhysicsTick(float deltaTime)
    {
        if (disablePhysics) return;
        
        // Init data for this frame
        _currentFramePosition = rigidbody.position;
        RaycastHit groundHit;
        _onGround = Physics.Raycast(_capsulePoint1 + Vector3.down * _extentX, Vector3.down, out groundHit, _groundCheckLength);
        _currentFrameMovementRemaining = Vector3.zero;
        
        #region Compute new horizontal velocity
        Vector3 worldGroundDirection = transform.right *  localGroundDirection.x + transform.forward * localGroundDirection.y;
        worldGroundDirection = new Vector2(worldGroundDirection.x, worldGroundDirection.z).normalized;
        Vector2 wantedVelocity = worldGroundDirection * movementMetrics.maxgroundSpeed;
        
        // Base acceleration when we want to move
        float realAcceleration = movementMetrics.groundAcceleration;
        
        // If we want to stop, apply ground friction
        if (wantedVelocity.magnitude < float.Epsilon)
        {
            realAcceleration = movementMetrics.groundFriction;
        }
        // If we want to turn, use ground friction AND extra turn friction
        else if (Vector2.Angle(_currentGroundVelocity, wantedVelocity) > movementMetrics.uTurnAngle)
        {
            realAcceleration = movementMetrics.groundFriction + movementMetrics.uTurnFriction;
        }
        
        _currentGroundVelocity = Vector2.MoveTowards(_currentGroundVelocity, wantedVelocity, realAcceleration * deltaTime);
        
        // Align movement to ground for slopes
        Vector3 movement = new Vector3(_currentGroundVelocity.x, 0, _currentGroundVelocity.y) * deltaTime;
        if (_onGround)
        {
            movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
        }
        
        _currentFrameMovementRemaining += movement;
        #endregion

        #region Compute new vertical velocity

        if (_onGround)
        {
            _extraJumpsRemaining = movementMetrics.extraJumps;
            _currentVerticalSpeed = 0f;
            // Normal jump
            if (shouldJump)
            {
                _currentVerticalSpeed = movementMetrics.jumpForce;
                OnJumped.Invoke();
            }
        }
        else // In air
        {
            float targetVerticalSpeed = movementMetrics.maxFallSpeed;
            // Extra jump
            if (shouldExtraJump && _extraJumpsRemaining > 0)
            {
                _currentVerticalSpeed = movementMetrics.jumpForce;
                _extraJumpsRemaining--;
                OnJumped.Invoke();
            }
            // Floating
            else if (shouldFloat)
            {
                targetVerticalSpeed = movementMetrics.maxFallSpeed * movementMetrics.floatFactor;
            }
            _currentVerticalSpeed = Mathf.Max(targetVerticalSpeed, _currentVerticalSpeed + movementMetrics.gravityForce *  deltaTime);
        }
        _currentFrameMovementRemaining += Vector3.up * (_currentVerticalSpeed * deltaTime);
        #endregion

        #region Move

        int currentIteration = 0;
        // Collide and slide on bodies along the way
        while (_currentFrameMovementRemaining.magnitude > 0.01f && currentIteration++ < MAX_CAST_ITERATIONS)
        {
            if (Physics.CapsuleCast(
                    _capsulePoint1,
                    _capsulePoint2,
                    _extentX,
                    _currentFrameMovementRemaining.normalized, out var hit,
                    _currentFrameMovementRemaining.magnitude + _skinWidth))
            {
                Vector3 hitNormal = hit.normal;
                if (_onGround && hitNormal.y < 0 )
                {
                    hitNormal = Vector3.ProjectOnPlane(hitNormal, groundHit.normal);
                }
                // https://nvidia-omniverse.github.io/PhysX/physx/5.2.1/docs/GeometryQueries.html
                // Capsule cast picks a hit point towards the manifold center of the two meshes colliding, so we cannot use hit.point
                // Hit distance already accounts for the radius of the capsule
                _currentFramePosition += Mathf.Max(0f, hit.distance - _skinWidth) * _currentFrameMovementRemaining.normalized;
                Vector3 project = PhysicsSystem
                    .CleanVector3(Vector3.ProjectOnPlane(_currentFrameMovementRemaining, hitNormal));
                _currentFrameMovementRemaining = project;
                
            }
            else
            {
                _currentFramePosition += _currentFrameMovementRemaining;
                _currentFrameMovementRemaining = Vector3.zero;
            }
        }
        
        // Not really working
        Depenetrate();
        
        rigidbody.position = _currentFramePosition;

        #endregion
        
    }

    private void Depenetrate()
    {
        int hitCount = Physics.OverlapCapsuleNonAlloc(_capsulePoint1, _capsulePoint2, _extentX, _overlapCapsuleHits);
        for (int i = 0; i < hitCount; ++i) {
            Collider overlap = _overlapCapsuleHits[i];
            if (overlap == collider)
            {
                continue;
            }
            Vector3 otherPosition = collider.gameObject.transform.position;
            Quaternion otherRotation = collider.gameObject.transform.rotation;
            if (Physics.ComputePenetration(collider, _currentFramePosition, Quaternion.identity, collider,
                    otherPosition, otherRotation, out var direction, out var distance))
            {
                Debug.DrawRay(otherPosition, direction * distance, Color.yellow);
                _currentFramePosition -= direction * (distance);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_capsulePoint1, _extentX);
        Gizmos.DrawWireSphere(_capsulePoint2, _extentX);
        Gizmos.DrawRay(_currentFramePosition, _currentGroundVelocity.normalized);
        Gizmos.DrawRay(_currentFramePosition + Vector3.down * _extentY, Vector3.down * _groundCheckLength);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_currentFramePosition, _currentGroundVelocity);  
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10f, 10f + 0 * 20f, 800f, 22f), $"Current ground velocity: {_currentGroundVelocity}");
        GUI.Label(new Rect(10f, 10f + 1 * 20f, 800f, 22f), $"Wanted ground velocity: {localGroundDirection}");
        GUI.Label(new Rect(10f, 10f + 2 * 20f, 800f, 22f), $"Current vertical speed: {_currentVerticalSpeed}");
        GUI.Label(new Rect(10f, 10f + 3 * 20f, 800f, 22f), $"On ground: {_onGround}");
        GUI.Label(new Rect(10f, 10f + 4 * 20f, 800f, 22f), $"Extra jumps: {_extraJumpsRemaining}");
    }
    
    
}
