using UnityEngine;

public class DEBUG_PlayerAnimationsGameplay : MonoBehaviour
{
    [Header("Debug state")]
    [Tooltip("Current state of freeze")]
    public bool flipFreeze;
    [Tooltip("Current state of dead")]
    public bool flipDead;
    [Tooltip("Current state of running")]
    public bool flipRunning;
    [Tooltip("Current state of inAir")]
    public bool flipInAir;

    [Header("Dependencies")]
    [Tooltip("The PlayerAnimationsGameplay to debug")]
    public PlayerAnimationsGameplay playerAnimationsGameplay;

    /// <summary>
    /// Debug1: flipFreeze
    /// Debug2: flipDead
    /// Debug3: flipRunning
    /// Debug4: flipInAir
    /// Debug5: Hit
    /// Debug6: Jump
    /// Debug7: Attack1
    /// Debug8: Attack2
    /// Debug9: Defense 
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            flipFreeze = !flipFreeze;
            if (flipFreeze)
                playerAnimationsGameplay.Freeze();
            else
                playerAnimationsGameplay.Unfreeze();
        }
        else if (Input.GetButtonDown("Debug2"))
        {
            flipDead = !flipDead;
            playerAnimationsGameplay.SetDead(flipDead);
        }
        else if (Input.GetButtonDown("Debug3"))
        {
            flipRunning = !flipRunning;
            playerAnimationsGameplay.SetRunning(flipRunning);
        }
        else if (Input.GetButtonDown("Debug4"))
        {
            flipInAir = !flipInAir;
            playerAnimationsGameplay.SetInAir(flipInAir);
        }
        else if (Input.GetButtonDown("Debug5"))
        {
            playerAnimationsGameplay.Hit();
        }
        else if (Input.GetButtonDown("Debug6"))
        {
            playerAnimationsGameplay.Jump();
        }
        else if (Input.GetButtonDown("Debug7"))
        {
            playerAnimationsGameplay.PlayAttack1();
        }
        else if (Input.GetButtonDown("Debug8"))
        {
            playerAnimationsGameplay.PlayAttack2();
        }
        else if (Input.GetButtonDown("Debug9"))
        {
            playerAnimationsGameplay.PlayDefense();
        }
    }
}
