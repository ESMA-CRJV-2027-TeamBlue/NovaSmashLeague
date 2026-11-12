using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementMetricsSO", menuName = "Scriptable Objects/PlayerMovementMetricsSO")]
public class PlayerMovementMetricsSO : ScriptableObject
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
    [Tooltip("Velocity applied on the player when jumping")]
    public float jumpForce = 50f;
    [Tooltip("Velocity applied on the player towards the ground when in air")]
    public float gravityForce = -120f;
    [Tooltip("Maximum falling speed of the player when in air")]
    public float maxFallSpeed = -50f;
    [Tooltip("Gravity modifier when floating")]
    public float floatFactor = 0.5f;
    [Tooltip("Number of extra jumps available (1 = double jump)")]
    public int extraJumps = 1;
}
