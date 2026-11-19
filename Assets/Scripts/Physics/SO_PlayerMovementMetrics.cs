using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerMovementMetrics", menuName = "Scriptable Objects/SO_PlayerMovementMetrics")]
public class SO_PlayerMovementMetrics : ScriptableObject
{
    [Header("Ground movement")]
    [Tooltip("How fast the speed changes on ground")]
    public float groundAcceleration = 50f;
    [Tooltip("How fast the speed goes back to zero on ground")]
    public float groundFriction = 90f;
    [Tooltip("How fast the player can run")]
    public float maxgroundSpeed = 20f;
    [Tooltip("How much angle is needed to get extra friction to turn faster")]
    public float uTurnAngle = 30f;
    [Tooltip("How much extra friction is applied to turn faster")]
    public float uTurnFriction = 60f;
    
    [Header("Vertical movement")]
    [Tooltip("Force applied on the player at jump start")]
    public float jumpForce = 50f;
    [Tooltip("Velocity applied on the player towards the ground when in air")]
    public float gravityForce = -120f;
    [Tooltip("Maximum falling speed of the player when in air")]
    public float maxFallSpeed = -50f;
    [Tooltip("Gravity multiplier when falling")]
    public float fallMultiplier = 2.0f;
    [Tooltip("Gravity modifier when floating")]
    public float floatFactor = 0.5f;
    [Tooltip("Number of extra jumps available (1 = double jump)")]
    public int extraJumps = 1;
    
    [Header("Calculation helpers")]
    [ContextMenuItem("ComputeMetrics",  "ComputeMetrics")]
    [Tooltip("How high (in grid units) we want to jump at maximum")]
    public float jumpHeight;
    [ContextMenuItem("ComputeMetrics",  "ComputeMetrics")]
    [Tooltip("How long should it take to reach the apex of the jump")]
    public float jumpApexTime;
    
    /// <summary>
    /// Used to facilitate balancing
    /// In the inspector, set the jumpHeight and the jumpApexTime
    /// Then right click on one of these fields and select ComputeMetrics
    /// The console will display the corresponding metrics to set manually
    /// </summary>
    private void ComputeMetrics()
    {
        // Formulas are courtesy of https://github.com/DawnosaurDev/platformer-movement
        // Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        float gravityForceComputed = -2 * jumpHeight / Mathf.Pow(jumpApexTime, 2);

        // Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        float jumpForceComputed = -gravityForceComputed * jumpApexTime;
        
        Debug.Log($"For apex height: {jumpHeight}u and apex time: {jumpApexTime}s; jumpForce={jumpForceComputed}, gravityForce={gravityForceComputed}");
    }
}
