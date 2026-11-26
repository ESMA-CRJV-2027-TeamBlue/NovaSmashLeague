using System;
using UnityEngine;

public class TestTrackingScreen : MonoBehaviour
{
    public GameObject trackedObject;
    public Camera trackingCamera;
    private Vector2 screenPosition;
    private Vector3 targetPosition;
    private Vector3 _dampVelocity;

    [Range(0f, 1f)]
    public float rotateSpeed;


    public bool useDamp = false;
    public float smoothTime = 1f;
    public float maxSpeed = 10f;

    private void Start()
    {
        targetPosition = trackingCamera.transform.position;
    }

    private void Update()
    {
       screenPosition = trackingCamera.WorldToViewportPoint(trackedObject.transform.position);
        var p1 = screenPosition.y;
        var p2 = 0.8f;
        var p3 = 0.2f;
        var unrotatedTrackedPosition =
            Quaternion.AngleAxis(-trackingCamera.transform.eulerAngles.x, trackingCamera.transform.right) *
            trackedObject.transform.position;
        var unrotatedCameraPosition =
            Quaternion.AngleAxis(-trackingCamera.transform.eulerAngles.x, trackingCamera.transform.right) *
            trackingCamera.transform.position;
        var offsetY = unrotatedCameraPosition.y;
        var offsetP = 0.5f;
        if (p1 > p2)
        {
            float Y = unrotatedTrackedPosition.y - offsetY;
            float delta = Y - ((p2-offsetP) * Y) / (p1-offsetP);
            targetPosition = trackingCamera.transform.position + Vector3.up * delta;
        }
        else if(p1<p3)
        {
            float Y = unrotatedTrackedPosition.y - offsetY;
            float delta = Y - ((p3-offsetP) * Y) / (p1-offsetP);
            targetPosition = trackingCamera.transform.position + Vector3.up * delta;
        }
        
        trackingCamera.transform.position = targetPosition;
        
        Vector3 relativePos = trackedObject.transform.position - trackingCamera.transform.position;
        relativePos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation = Quaternion.Lerp(trackingCamera.transform.rotation, rotation, Mathf.Lerp(0.5f, 5f, rotateSpeed) * Time.deltaTime);
        // pivotCamera.rotation = rotation;
        trackingCamera.transform.eulerAngles= new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10f, 10f + 0 * 20f, 800f, 22f), "screenPosition: "+screenPosition);
    }
}
