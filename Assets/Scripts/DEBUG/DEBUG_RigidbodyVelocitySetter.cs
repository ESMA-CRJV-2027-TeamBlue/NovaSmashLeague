using UnityEngine;

public class DEBUG_RigidbodyVelocitySetter : MonoBehaviour
{
    [Tooltip("The rigidbody to debug")]
    public Rigidbody rb;
    [Tooltip("The speed to set velocity at")]
    public float speed = 2f;

    void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            rb.linearVelocity = Vector3.forward + Vector3.down + Vector3.left;
        }
        else if (Input.GetButtonDown("Debug2"))
        {
            rb.linearVelocity = Vector3.down;
        }
        else if (Input.GetButtonDown("Debug3"))
        {
            rb.linearVelocity = Vector3.forward + Vector3.down + Vector3.right;
        }
        else if (Input.GetButtonDown("Debug4"))
        {
            rb.linearVelocity = Vector3.left;
        }
        else if (Input.GetButtonDown("Debug5"))
        {
            rb.linearVelocity = Vector3.back;
        }
        else if (Input.GetButtonDown("Debug6"))
        {
            rb.linearVelocity = Vector3.right;
        }
        else if (Input.GetButtonDown("Debug7"))
        {
            rb.linearVelocity = Vector3.forward + Vector3.up + Vector3.left;
        }
        else if (Input.GetButtonDown("Debug8"))
        {
            rb.linearVelocity = Vector3.up;
        }
        else if (Input.GetButtonDown("Debug9"))
        {
            rb.linearVelocity = Vector3.forward + Vector3.up + Vector3.right;
        }
        
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }
}
