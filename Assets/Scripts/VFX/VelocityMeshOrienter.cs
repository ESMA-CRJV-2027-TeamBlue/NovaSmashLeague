using System;
using UnityEngine;

public class VelocityMeshOrienter : MonoBehaviour
{
    [Tooltip("The rigidbody to read the velocity from")]
    public Rigidbody rb;
    [Tooltip("The mesh to orient")]
    public GameObject mesh;

    private void Update()
    {
        mesh.transform.up = -rb.linearVelocity;
    }
}
