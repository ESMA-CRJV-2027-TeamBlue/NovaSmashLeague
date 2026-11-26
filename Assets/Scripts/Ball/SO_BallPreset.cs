using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BallPreset", menuName = "Scriptable Objects/SO_BallPreset")]
public class SO_BallPreset : ScriptableObject
{
    [Tooltip("The base speed of the ball, in unit/second")]
    public float baseSpeed = 1f;
    [Tooltip("The base damage of the ball")]
    public float baseDamage = 1f;
    [Tooltip("The available speed levels for the ball")]
    public List<SO_BallSpeedLevel> speedLevels;
}
