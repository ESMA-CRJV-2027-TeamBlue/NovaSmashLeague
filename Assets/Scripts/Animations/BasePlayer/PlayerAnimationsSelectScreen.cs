using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationsSelectScreen : MonoBehaviour
{
    #region Animation Parameters Cache
    
    private static readonly int IsConfirmedIndex = Animator.StringToHash("IsConfirmed");
    
    #endregion

    #region Internal dependencies
    
    private Animator _animator;
    
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Indicates to the animator to be confirmed or not
    /// </summary>
    /// <param name="isConfirmed">Whether the character selection is confirmed or not</param>
    public void SetConfirmed(bool isConfirmed)
    {
        _animator.SetBool(IsConfirmedIndex, isConfirmed);
    }
    
}
