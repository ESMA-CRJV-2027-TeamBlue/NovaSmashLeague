using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DEBUG_VFXActivatorInput : MonoBehaviour
{
    [Tooltip("The VFXActivator to debug")]
    public VFXActivator vfxActivator;
    [Tooltip("The debug inputs to start the VFX")]
    public List<String> startDebugInputs = new List<String>(){"Debug1"};
    [Tooltip("The debug inputs to stop the VFX")]
    public List<String> stopDebugInputs = new List<String>(){"Debug2"};
    
    void Update()
    {
        if (startDebugInputs.Any(Input.GetButtonDown))
        {
            vfxActivator.StartVFX();
        }
        else if(stopDebugInputs.Any(Input.GetButtonDown))
        {
            vfxActivator.StopVFX();
        }
    }
}
