using UnityEngine;

public class DEBUG_PhysicsBodyMover : MonoBehaviour
{
    [Tooltip("The PhysicsBody to debug")]
    public DynamicBody dynamicBody;

    [Tooltip("The speed change applied (additive)")]
    public float speedIncrement = 1f;
    
    // Update is called once per frame
    public void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            dynamicBody.movementDirection = Vector3.forward + Vector3.down + Vector3.left;
        }
        else if (Input.GetButtonDown("Debug2"))
        {
            dynamicBody.movementDirection = Vector3.down;
        }
        else if (Input.GetButtonDown("Debug3"))
        {
            dynamicBody.movementDirection = Vector3.back + Vector3.down + Vector3.right;
        }
        else if (Input.GetButtonDown("Debug4"))
        {
            dynamicBody.movementDirection = Vector3.left;
        }
        else if (Input.GetButtonDown("Debug5"))
        {
            dynamicBody.movementDirection = Vector3.back;
        }
        else if (Input.GetButtonDown("Debug6"))
        {
            dynamicBody.movementDirection = Vector3.right;
        }
        else if (Input.GetButtonDown("Debug7"))
        {
            dynamicBody.movementDirection = Vector3.back + Vector3.up + Vector3.left;
        }
        else if (Input.GetButtonDown("Debug8"))
        {
            dynamicBody.movementDirection = Vector3.up;
        }
        else if (Input.GetButtonDown("Debug9"))
        {
            dynamicBody.movementDirection = Vector3.forward + Vector3.up + Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            dynamicBody.speed += speedIncrement;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            dynamicBody.speed -= speedIncrement;
        }
        dynamicBody.speed = Mathf.Max(dynamicBody.speed, 0);
    }
}
