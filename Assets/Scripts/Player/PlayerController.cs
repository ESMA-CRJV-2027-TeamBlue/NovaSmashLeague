using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerBody))]
public class PlayerController : MonoBehaviour
{
    private PlayerBody _body;
    private PlayerInput _input;

    private void Awake()
    {
        _body = GetComponent<PlayerBody>();
        _input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        _body.shouldJump = _input.holdsJump || _input.pressedJump;
        _body.shouldExtraJump = _input.pressedJump;
        _body.shouldFloat = _input.holdsJump;
    }

    private void Reset()
    {
        if (GetComponent<HumanPlayerInput>() == null)
        {
            gameObject.AddComponent<HumanPlayerInput>();
        }
    }
}
