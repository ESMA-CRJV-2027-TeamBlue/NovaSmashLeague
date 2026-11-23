using UnityEngine;

public class DEBUG_PlayerAnimationsSelectScreen : MonoBehaviour
{
    [Header("Debug state")]
    [Tooltip("Current state of confirmed")]
    public bool flipConfirmed;

    [Header("Dependencies")]
    [Tooltip("The PlayerAnimationsSelectScreen to debug")]
    public PlayerAnimationsSelectScreen playerAnimationsSelectScreen;

    /// <summary>
    /// Debug1: flipConfirmed
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            flipConfirmed = !flipConfirmed;
            playerAnimationsSelectScreen.SetConfirmed(flipConfirmed);
        }
    }
}
