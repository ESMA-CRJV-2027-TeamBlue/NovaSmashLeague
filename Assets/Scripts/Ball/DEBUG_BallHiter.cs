using System;
using UnityEngine;

public class DEBUG_BallHiter : MonoBehaviour
{
    [Tooltip("The BallBody to debug")]
    [SerializeField] BallBody _ballBody;
    [Tooltip("The BallController to debug")]
    [SerializeField] BallController _ballController;
    
    private void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            _ballBody.ChangeDirection(Vector3.forward + Vector3.down + Vector3.left);
        }
        else if (Input.GetButtonDown("Debug2"))
        {
            _ballBody.ChangeDirection(Vector3.down);
        }
        else if (Input.GetButtonDown("Debug3"))
        {
            _ballBody.ChangeDirection(Vector3.back + Vector3.down + Vector3.right);
        }
        else if (Input.GetButtonDown("Debug4"))
        {
            _ballBody.ChangeDirection(Vector3.left);
        }
        else if (Input.GetButtonDown("Debug5"))
        {
            _ballBody.ChangeDirection(Vector3.back);
        }
        else if (Input.GetButtonDown("Debug6"))
        {
            _ballBody.ChangeDirection(Vector3.right);
        }
        else if (Input.GetButtonDown("Debug7"))
        {
            _ballBody.ChangeDirection(Vector3.forward + Vector3.up + Vector3.left);
        }
        else if (Input.GetButtonDown("Debug8"))
        {
            _ballBody.ChangeDirection(Vector3.up);
        }
        else if (Input.GetButtonDown("Debug9"))
        {
            _ballBody.ChangeDirection(Vector3.back + Vector3.up + Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            _ballBody.ChangeSpeedLevel(1);
            _ballBody.Impact();
            _ballBody.Decay();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            _ballBody.ChangeSpeedLevel(-1);
            _ballBody.Impact();
            _ballBody.Decay();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            _ballBody.Impact();
            _ballBody.Decay();
        }
    }
}
