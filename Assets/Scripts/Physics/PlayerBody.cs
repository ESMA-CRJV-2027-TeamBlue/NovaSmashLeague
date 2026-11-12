using System;
using UnityEngine;

public class PlayerBody : PhysicsBody
{
    private Vector2 _currentGroundVelocity;
    private Vector2 _wantedGroundDirection;
    private float _currentVerticalSpeed;
    private Vector3 _currentFramePosition;
    private Vector3 _currentFrameMovementRemaining;
    private bool _onGround;
    private float _extentX;
    private float _extentY;
    private bool _shouldJump;
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
        _shouldJump = Input.GetButtonDown("Jump");
    }

    public override void PhysicsTick(float deltaTime)
    {
        // Init data for this frame
        _currentFramePosition = rigidbody.position;
        _onGround = Physics.Raycast(_currentFramePosition + Vector3.down * _extentY, Vector3.down, _groundCheckLength);
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
            _currentVerticalSpeed = 0f;
            if (_shouldJump)
            {
                _currentVerticalSpeed = movementMetrics.jumpForce;
            }
        }
        else
        {
            _currentVerticalSpeed = Mathf.MoveTowards(_currentVerticalSpeed, movementMetrics.maxFallSpeed, movementMetrics.gravityForce *  deltaTime);
        }
        _currentFrameMovementRemaining += Vector3.up * (_currentVerticalSpeed * deltaTime);
        #endregion

        #region Move
        // Collide and slide on bodies along the way
        while (_currentFrameMovementRemaining.magnitude > 0.01f)
        {
            if (Physics.CapsuleCast(
                    _currentFramePosition - Vector3.up * (_extentY - _extentX),
                    _currentFramePosition + Vector3.up * (_extentY - _extentX),
                    _extentX,
                    _currentFrameMovementRemaining.normalized, out var hit,
                    _currentFrameMovementRemaining.magnitude + _skinWidth))
            {
                Vector3 hitNormal = PhysicsSystem.CleanVector3(hit.normal.normalized);
                // https://nvidia-omniverse.github.io/PhysX/physx/5.2.1/docs/GeometryQueries.html
                // Capsule cast picks a hit point towards the manifold center of the two meshes colliding, so we cannot use hit.point
                // Hit distance already accounts for the radius of the capsule
                _currentFramePosition += (hit.distance - _skinWidth) * _currentFrameMovementRemaining.normalized;
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

        rigidbody.position = _currentFramePosition;

        #endregion
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_currentFramePosition - Vector3.up * (_extentY - _extentX), _extentX);
        Gizmos.DrawWireSphere(_currentFramePosition + Vector3.up * (_extentY - _extentX), _extentX);
        Gizmos.DrawRay(_currentFramePosition, _currentFrameMovementRemaining.normalized);
        Gizmos.DrawRay(_currentFramePosition + Vector3.down * _extentY, Vector3.down * _groundCheckLength);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10f, 10f + 0 * 20f, 800f, 22f), $"Current ground velocity: {_currentGroundVelocity}");
        GUI.Label(new Rect(10f, 10f + 1 * 20f, 800f, 22f), $"Wanted ground velocity: {_wantedGroundDirection}");
        GUI.Label(new Rect(10f, 10f + 2 * 20f, 800f, 22f), $"Current vertical speed: {_currentVerticalSpeed}");
        GUI.Label(new Rect(10f, 10f + 3 * 20f, 800f, 22f), $"On ground: {_onGround}");
    }
    
    
}
