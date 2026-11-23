using UnityEngine;

public class DEBUG_PlayerAnimationsPostMatch : MonoBehaviour
{
    [Header("Debug state")]
    [Tooltip("Current state of HasWon")]
    public bool flipHasWon;

    [Header("Dependencies")]
    [Tooltip("The PlayerAnimationsPostMatch to debug")]
    public PlayerAnimationsPostMatch playerAnimationsPostMatch;

    /// <summary>
    /// Debug1: flipHasWon
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            flipHasWon = !flipHasWon;
            playerAnimationsPostMatch.SetHasWon(flipHasWon);
        }
    }
}
