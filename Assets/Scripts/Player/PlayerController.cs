using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerBody))]
[RequireComponent(typeof(HumanPlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerBody _body;
    private PlayerInput _playerInput;
    
    [Tooltip("Buffer time for inputs")]
    public float bufferTime = 0.1f;
    
    private Coroutine _pressedJumpBufferingCoroutine;
    private Coroutine _releasedJumpBufferingCoroutine;

    private void Awake()
    {
        // Init internal references 
        _body = GetComponent<PlayerBody>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // Jump action inputs
        _playerInput.pressedJump.AddListener(OnPressedJump);
        _playerInput.releasedJump.AddListener(OnReleasedJump);
        // Jump action consuming
        _body.OnJumped.AddListener(ConsumePressedJump);
        _body.OnExtraJumped.AddListener(ConsumePressedJump);
    }

    private void OnDisable()
    {
        // Remove the listeners previously set up
        // Jump action inputs
        _playerInput.pressedJump.RemoveListener(OnPressedJump);
        _playerInput.pressedJump.RemoveListener(OnReleasedJump);
        // Jump action consuming
        _body.OnJumped.RemoveListener(ConsumePressedJump);
        _body.OnExtraJumped.RemoveListener(ConsumePressedJump);
    }

    private void Update()
    {
        // Continuously updates the movement inputs 
        _body.localGroundDirection = _playerInput.rawDirection;
    }
    
    /// <summary>
    /// Informs the body that it should take actions according to the jump input:
    /// Jumping, ExtraJumping, Floating
    /// Also starts a Coroutine that handles input buffering (<see cref="JumpBufferCoroutine"/>)
    /// </summary>
    private void OnPressedJump()
    {
        _body.shouldJump = true;
        _body.shouldExtraJump = true;
        _body.shouldFloat = true;
        // Reset current couroutine state
        if (_pressedJumpBufferingCoroutine != null)
        {
            StopCoroutine(_pressedJumpBufferingCoroutine);
        }
        _pressedJumpBufferingCoroutine = StartCoroutine(JumpBufferCoroutine());
    }
    
    /// <summary>
    /// Allows to consider that the player pressed jump for a bit extra time
    /// Useful to account for unprecise timings
    /// Also makes sure any script that needs to read this value has a chance to execute
    /// </summary>
    /// <returns>A Coroutine</returns>
    private IEnumerator JumpBufferCoroutine()
    {
        yield return new WaitForSeconds(bufferTime);
        _body.shouldExtraJump = false;
        _pressedJumpBufferingCoroutine = null;
    }
    
    /// <summary>
    /// Prevents the body from jumping an additional time
    /// Also stop the current Coroutine
    /// </summary>
    private void ConsumePressedJump()
    {
        _body.shouldExtraJump = false;
        if (_pressedJumpBufferingCoroutine != null)
        {
            StopCoroutine(_pressedJumpBufferingCoroutine);
        }
        _pressedJumpBufferingCoroutine = null;
    }

    /// <summary>
    /// Informs the body that it should take actions according to releasing the jump input:
    /// Stop Jump, stop ExtraJump, stop Float
    /// </summary>
    private void OnReleasedJump()
    {
        _body.shouldJump = false;
        _body.shouldExtraJump = false;
        _body.shouldFloat = false;
    }
}
