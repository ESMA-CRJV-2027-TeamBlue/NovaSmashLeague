using System;
using UnityEngine;

public class PlayerBody : PhysicsBody
{
    // Data needed for movement
    private Vector2 _currentGroundVelocity;
    private Vector2 _wantedGroundDirection;
    private float _currentVerticalSpeed;
    private Vector3 _currentFramePosition;
    private Vector3 _currentFrameMovementRemaining;
    // Array to avoid creating new objects every frame
    private RaycastHit[] _capsuleHits = new RaycastHit[10];
    // How many objects we collided with last cast
    private int _nbCaspuleHits;
    
    // Indications of how the body should react
    [Header("Behaviour")]
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


    protected override void Start()
    {
        base.Start();
        _extentX = collider.bounds.extents.x;
        _extentY = collider.bounds.extents.y;
    }

    private void Update()
    {
        // Read inputs to know what (world) direction we want to go
        Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 localGroundDirection = transform.right *  inputDirection.x + transform.forward * inputDirection.y;
        _wantedGroundDirection = new Vector2(localGroundDirection.x, localGroundDirection.z).normalized;
    }

    public override void PhysicsTick(float deltaTime)
    {
        // Init data for this frame
        _currentFramePosition = rigidbody.position;
        _onGround = Physics.Raycast(_capsulePoint1 + Vector3.down * _extentX, Vector3.down, _groundCheckLength);
        _currentFrameMovementRemaining = Vector3.zero;
        
        #region Compute new horizontal velocity
        Vector2 wantedVelocity = _wantedGroundDirection * movementMetrics.maxgroundSpeed;
        
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
        
        _currentFrameMovementRemaining += new Vector3(_currentGroundVelocity.x, 0, _currentGroundVelocity.y) * deltaTime;
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
        Vector3 lastSafePosition = _currentFramePosition;
        // Collide and slide on bodies along the way
        while (_currentFrameMovementRemaining.magnitude > 0.01f && currentIteration++ < MAX_CAST_ITERATIONS)
        {
            // Check all collisions ahead
            _nbCaspuleHits = Physics.CapsuleCastNonAlloc(
                _capsulePoint1,
                _capsulePoint2,
                _extentX,
                _currentFrameMovementRemaining.normalized,
                _capsuleHits,
                _currentFrameMovementRemaining.magnitude + _skinWidth
            );
            RaycastHit hit = FindClosestHit();
            if (hit.collider is not null)
            {
                Debug.Log(hit.collider.name);
            // if (Physics.CapsuleCast(
            //         _capsulePoint1,
            //         _capsulePoint2,
            //         _extentX,
            //         _currentFrameMovementRemaining.normalized, out var hit,
            //         _currentFrameMovementRemaining.magnitude + _skinWidth))
            // {
                Vector3 hitNormal = PhysicsSystem.CleanVector3(hit.normal.normalized);
                // https://nvidia-omniverse.github.io/PhysX/physx/5.2.1/docs/GeometryQueries.html
                // Capsule cast picks a hit point towards the manifold center of the two meshes colliding, so we cannot use hit.point
                // Hit distance already accounts for the radius of the capsule
                _currentFramePosition += (hit.distance - _skinWidth) * _currentFrameMovementRemaining.normalized;
                Vector3 project = PhysicsSystem
                    .CleanVector3(Vector3.ProjectOnPlane(_currentFrameMovementRemaining, hitNormal));
                _currentFrameMovementRemaining = project;
                
                // // THIS DOESNT WORK
                // // Check if we can slide along the surface hit
                // int hits = Physics.OverlapCapsuleNonAlloc(_capsulePoint1, _capsulePoint2, _extentX, _overlapCapsuleHits);
                // for (int i = 0; i < hits; i++)
                // {
                //     // If we overlap with another body, stop movement on last available position
                //     Collider colliderHit = _overlapCapsuleHits[i];
                //     if(colliderHit != collider)
                //     {
                //         _currentFramePosition = lastSafePosition;
                //         _currentFrameMovementRemaining = Vector3.zero;
                //         break;
                //     }
                // }
                // // If we don't overlap with anything else, store the safe position
                // lastSafePosition = _currentFramePosition;
            }
            else
            {
                _currentFramePosition += _currentFrameMovementRemaining;
                _currentFrameMovementRemaining = Vector3.zero;
            }
        }

        rigidbody.position = _currentFramePosition;

        #endregion
        
    }

    /// <summary>
    /// Checks the last result of a Physics.CapsuleCastNonAlloc to find the raycast ht closest to the current position of the body
    /// </summary>
    /// <returns>New RaycastHit if no collision, else the RaycastHit with the least distance</returns>
    private RaycastHit FindClosestHit()
    {
        // Find the RaycastHit that has the least distance
        int indexMin = -1;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < _nbCaspuleHits; i++)
        {
            // Ignore own body
            if (_capsuleHits[i].collider != collider && _capsuleHits[i].distance < minDistance)
            {
                minDistance = _capsuleHits[i].distance;
                indexMin = i;
            }
        }
        
        if (indexMin == -1)
        {
            // Couldn't make it work with returning null value
            return new RaycastHit();
        }

        return _capsuleHits[indexMin];
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
        GUI.Label(new Rect(10f, 10f + 1 * 20f, 800f, 22f), $"Wanted ground velocity: {_wantedGroundDirection}");
        GUI.Label(new Rect(10f, 10f + 2 * 20f, 800f, 22f), $"Current vertical speed: {_currentVerticalSpeed}");
        GUI.Label(new Rect(10f, 10f + 3 * 20f, 800f, 22f), $"On ground: {_onGround}");
        GUI.Label(new Rect(10f, 10f + 4 * 20f, 800f, 22f), $"Extra jumps: {_extraJumpsRemaining}");
    }
    
    
}
