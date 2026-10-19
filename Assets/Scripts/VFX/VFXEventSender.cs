using System;
using UnityEngine;
using UnityEngine.VFX;

public class VFXEventSender: MonoBehaviour
{
    [Tooltip("The VFX to send an event to")]
    public VisualEffect visualEffect;

    private VFXEventAttribute _vfxAttribute;

    /// <summary>
    /// Sends an event along withs a Vector3 attribute to the VFX
    /// </summary>
    /// <param name="eventName">The name of the even to send</param>
    /// <param name="attributeName">The name of the Vector3 attribute</param>
    /// <param name="data">The value for the Vector3 attribute</param>
    public void SendVector3(String eventName, String attributeName, Vector3 data)
    {
        _vfxAttribute = new VFXEventAttribute(visualEffect.CreateVFXEventAttribute());
        _vfxAttribute.SetVector3(attributeName, data);
        visualEffect.SendEvent(eventName, _vfxAttribute);
    }

    /// <summary>
    /// Sends an event to the VFX
    /// </summary>
    /// <param name="eventName">The name of the event to send</param>
    public void SendEvent(String eventName)
    {
        visualEffect.SendEvent(eventName);
    }
}
