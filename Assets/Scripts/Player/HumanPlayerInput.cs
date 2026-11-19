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
                _playerInput.pressedJump.Invoke();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _playerInput.releasedJump.Invoke();
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
                _playerInput.pressedJump.Invoke();
            }
            else if (Gamepad.all[gamepadIndex].buttonSouth.wasReleasedThisFrame ||  Gamepad.all[gamepadIndex].leftTrigger.wasReleasedThisFrame)
            {
                _playerInput.releasedJump.Invoke();
            }
            _playerInput.pressedAttack1 = Gamepad.all[gamepadIndex].buttonWest.wasPressedThisFrame || Gamepad.all[gamepadIndex].rightShoulder.wasPressedThisFrame;
            _playerInput.pressedAttack2 = Gamepad.all[gamepadIndex].buttonNorth.wasPressedThisFrame || Gamepad.all[gamepadIndex].rightTrigger.wasPressedThisFrame;
            _playerInput.pressedBlock = Gamepad.all[gamepadIndex].buttonEast.wasPressedThisFrame || Gamepad.all[gamepadIndex].leftShoulder.wasPressedThisFrame;
        }
    }
}
