using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerBody))]
[RequireComponent(typeof(HumanPlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerBody _body;
    private PlayerInput _playerInput;
    private HumanPlayerInput _humanPlayerInput;

    private void Awake()
    {
        _body = GetComponent<PlayerBody>();
        _playerInput = GetComponent<PlayerInput>();
        _humanPlayerInput =  GetComponent<HumanPlayerInput>();
    }

    private void Start()
    {
        _body.OnJumped.AddListener(_humanPlayerInput.ConsumePressedJump);
    }

    private void Update()
    {
        _body.shouldJump = _playerInput.holdsJump || _playerInput.pressedJump;
        _body.shouldExtraJump = _playerInput.pressedJump;
        _body.shouldFloat = _playerInput.holdsJump;
        _body.localGroundDirection = _playerInput.rawDirection;
    }

    private void Reset()
    {
        if (GetComponent<HumanPlayerInput>() == null)
        {
            gameObject.AddComponent<HumanPlayerInput>();
        }
    }
}
