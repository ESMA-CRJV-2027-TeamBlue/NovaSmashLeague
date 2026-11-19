using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public Vector2 rawDirection;
    public bool holdsJump;
    // public bool pressedJump;
    // public bool releasedJump;
    public bool pressedAttack1;
    public bool pressedAttack2;
    public bool pressedBlock;
    
    public UnityEvent pressedJump;
    public UnityEvent releasedJump;
}