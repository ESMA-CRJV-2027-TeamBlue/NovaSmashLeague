using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DEBUG_VFXEventPositionSender : MonoBehaviour
{
    [Tooltip("The VFXEventSender to debug")]
    public VFXEventSender vfxEventSender;
    [Tooltip("The name of the event to send")]
    public string eventName;
    [Tooltip("The name of the attribute for the position")]
    public string attributeName;
    [Tooltip("The transform to send the position of")]
    public Transform trackedTransform;
    [Tooltip("The debug inputs to send the event")]
    public List<String> debugInputs = new List<String>(){"Debug1"};
    
    public void Update()
    {
        if (debugInputs.Any(Input.GetButtonDown))
        {
            vfxEventSender.SendVector3(eventName, attributeName, trackedTransform.position);
        }
    }
}
