using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationsPostMatch: MonoBehaviour
{
    #region Animation Parameters Cache
    
    private static readonly int HasWonIndex = Animator.StringToHash("HasWon");
    
    #endregion

    #region Internal dependencies
    
    private Animator _animator;
    
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Indicates to the animator to be winning or not
    /// </summary>
    /// <param name="hasWon">Whether the character has won or not</param>
    public void SetHasWon(bool hasWon)
    {
        _animator.SetBool(HasWonIndex, hasWon);
    }
    
}
