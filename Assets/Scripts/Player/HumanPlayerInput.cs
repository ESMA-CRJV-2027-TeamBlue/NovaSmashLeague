using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class HumanPlayerInput : MonoBehaviour
{
    private PlayerInput _playerInput;
    
    [Tooltip("Whether this player uses a gamepad (else keyboard/mouse)")]
    public bool isGamepad;
    [Tooltip("If the player uses a gamepad, which one is it (0 = first controller)")]
    public int gamepadIndex;

    [Tooltip("Buffer time for inputs")]
    public float bufferTime = 0.1f;
    
    private Coroutine _pressedJumpBufferingCoroutine;
    private Coroutine _releasedJumpBufferingCoroutine;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movements
        Vector2 currentDirection;
        if(!isGamepad)
        {
            var horizontal = Input.GetKey(KeyCode.A)?-1:Input.GetKey(KeyCode.D)?1:0;
            var vertical = Input.GetKey(KeyCode.W)?1:Input.GetKey(KeyCode.S)?-1:0;
            currentDirection = new Vector2(horizontal, vertical).normalized;
        }
        else
        {
            currentDirection = Gamepad.all[gamepadIndex].leftStick.value;
        }
        _playerInput.rawDirection = currentDirection;

        // Actions
        if (!isGamepad)
        {
            _playerInput.holdsJump = Input.GetKey(KeyCode.Space);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PressJumpBuffer();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                ReleaseJumpBuffer();
            }
            _playerInput.pressedAttack1 = Mouse.current.leftButton.wasPressedThisFrame;
            _playerInput.pressedAttack2 = Mouse.current.rightButton.wasPressedThisFrame;
            _playerInput.pressedBlock = Input.GetKeyDown(KeyCode.E);
        }
        else
        {
            _playerInput.holdsJump = Gamepad.all[gamepadIndex].buttonSouth.isPressed || Gamepad.all[gamepadIndex].leftTrigger.isPressed;
            if (Gamepad.all[gamepadIndex].buttonSouth.wasPressedThisFrame ||  Gamepad.all[gamepadIndex].leftTrigger.wasPressedThisFrame)
            {
                PressJumpBuffer();
            }
            else if (Gamepad.all[gamepadIndex].buttonSouth.wasReleasedThisFrame ||  Gamepad.all[gamepadIndex].leftTrigger.wasReleasedThisFrame)
            {
                ReleaseJumpBuffer();
            }
            _playerInput.pressedAttack1 = Gamepad.all[gamepadIndex].buttonWest.wasPressedThisFrame || Gamepad.all[gamepadIndex].rightShoulder.wasPressedThisFrame;
            _playerInput.pressedAttack2 = Gamepad.all[gamepadIndex].buttonNorth.wasPressedThisFrame || Gamepad.all[gamepadIndex].rightTrigger.wasPressedThisFrame;
            _playerInput.pressedBlock = Gamepad.all[gamepadIndex].buttonEast.wasPressedThisFrame || Gamepad.all[gamepadIndex].leftShoulder.wasPressedThisFrame;
        }
    }

    private void PressJumpBuffer()
    {
        if (_pressedJumpBufferingCoroutine != null)
        {
            StopCoroutine(_pressedJumpBufferingCoroutine);
        }
        _pressedJumpBufferingCoroutine = StartCoroutine(PressJumpBufferCoroutine());
    }
    
    private void ReleaseJumpBuffer()
    {
        if (_releasedJumpBufferingCoroutine != null)
        {
            StopCoroutine(_releasedJumpBufferingCoroutine);
        }
        _releasedJumpBufferingCoroutine = StartCoroutine(ReleaseJumpBufferCoroutine());
    }

    public void ConsumePressedJump()
    {
        if (_pressedJumpBufferingCoroutine != null)
        {
            StopCoroutine(_pressedJumpBufferingCoroutine);
        }
        _pressedJumpBufferingCoroutine = null;
        _playerInput.pressedJump = false;
    }
    
    public void ConsumeReleasedJump()
    {
        if (_releasedJumpBufferingCoroutine != null)
        {
            StopCoroutine(_releasedJumpBufferingCoroutine);
        }
        _releasedJumpBufferingCoroutine = null;
        _playerInput.releasedJump = false;
    }
    
    /// <summary>
    /// Allows to consider that the player pressed jump for a bit extra time
    /// Makes sure any script that needs to read this value has a chance to execute
    /// </summary>
    /// <returns>A Coroutine</returns>
    private IEnumerator PressJumpBufferCoroutine()
    {
        
        // Multiple Updates can happen between two FixedUpdate, thus resetting the value by error
        _playerInput.pressedJump = true;
        yield return new WaitForSeconds(bufferTime);
        _playerInput.pressedJump = false;
        _pressedJumpBufferingCoroutine = null;
    }
    
    /// <summary>
    /// Allows to consider that the player released jump for a bit extra time
    /// /// Makes sure any script that needs to read this value has a chance to execute
    /// </summary>
    /// <returns>A Coroutine</returns>
    private IEnumerator ReleaseJumpBufferCoroutine()
    {
        // Multiple Updates can happen between two FixedUpdate, thus resetting the value by error
        _playerInput.releasedJump = true;
        yield return new WaitForSeconds(bufferTime);
        _playerInput.releasedJump = false;
        _releasedJumpBufferingCoroutine = null;
    }
}
