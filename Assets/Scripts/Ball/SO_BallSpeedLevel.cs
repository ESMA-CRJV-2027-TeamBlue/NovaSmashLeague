using UnityEngine;

[CreateAssetMenu(fileName = "SO_BallSpeedLevel", menuName = "Scriptable Objects/SO_BallSpeedLevel")]
public class SO_BallSpeedLevel : ScriptableObject
{
    [Tooltip("The speed of the ball during the impact phase")]
    public float impactSpeed = 2f;
    [Tooltip("The speed of the ball after the impact phase")]
    public float cruiseSpeed = 1f;
    [Tooltip("The duration of the impact phase")]
    public float impactDuration = 1f;
    [Tooltip("The time before the ball loses one speed level")]
    public float decayDuration = 10f;
    [Tooltip("The damage of the ball during the impact phase")]
    public float impactDamage = 2f;
    [Tooltip("The damage of the ball after the impact phase")]
    public float cruiseDamage = 1f;
}
