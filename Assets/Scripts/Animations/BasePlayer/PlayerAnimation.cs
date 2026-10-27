using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    #region Animation Parameters Cache

    private static readonly int IsRunningIndex = Animator.StringToHash("IsRunning");
    private static readonly int IsDeadIndex = Animator.StringToHash("IsDead");
    private static readonly int IsInAirIndex = Animator.StringToHash("IsInAir");
    private static readonly int HitIndex = Animator.StringToHash("Hit");
    private static readonly int JumpIndex = Animator.StringToHash("Jump");
    private static readonly int AttackIndex = Animator.StringToHash("Attack");
    private static readonly int Attack1Index = Animator.StringToHash("Attack1");
    private static readonly int Attack2Index = Animator.StringToHash("Attack2");
    private static readonly int DefenseIndex = Animator.StringToHash("Defense");

    #endregion

    #region Internal dependencies
    
    private Animator _animator;
    
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Indicates to the animator to be running or not
    /// </summary>
    /// <param name="isRunning">Whether the player is running or not</param>
    public void SetRunning(bool isRunning)
    {
        _animator.SetBool(IsRunningIndex, isRunning);
    }

    /// <summary>
    /// Indicates to the animator to be dead or not
    /// </summary>
    /// <param name="isDead">Whether the player is dead not</param>
    public void SetDead(bool isDead)
    {
        _animator.SetBool(IsDeadIndex, isDead);
    }

    /// <summary>
    /// Indicates to the animator be in air or not
    /// </summary>
    /// <param name="isInAir">Whether the player is in air or not</param>
    public void SetInAir(bool isInAir)
    {
        _animator.SetBool(IsInAirIndex, isInAir);
    }

    /// <summary>
    /// Indicates to the animator be hit
    /// </summary>
    public void Hit()
    {
        _animator.SetTrigger(HitIndex);
    }

    /// <summary>
    /// Indicates to the animator to jump
    /// </summary>
    public void Jump()
    {
        _animator.SetTrigger(JumpIndex);
    }

    /// <summary>
    /// Indicates to the animator to attack
    /// </summary>
    public void Attack()
    {
        _animator.SetTrigger(AttackIndex);
    }
    
    /// <summary>
    /// Indicates to the animator to play the Attack1
    /// </summary>
    public void PlayAttack1()
    {
        _animator.Play(Attack1Index);
    }
    
    /// <summary>
    /// Indicates to the animator to play the Attack2
    /// </summary>
    public void PlayAttack2()
    {
        _animator.Play(Attack2Index);
    }
    
    /// <summary>
    /// Indicates to the animator to play the Defense
    /// </summary>
    public void PlayDefense()
    {
        _animator.Play(DefenseIndex);
    }

    /// <summary>
    /// Halts the animator in the current state, nothing will happen until <see cref="Unfreeze"/> is called
    /// </summary>
    public void Freeze()
    {
        _animator.speed = 0;
    }
    
    /// <summary>
    /// Makes the animator resume current state, must be called after <see cref="Freeze"/>
    /// </summary>
    public void Unfreeze()
    {
        _animator.speed = 1;
    }
}
