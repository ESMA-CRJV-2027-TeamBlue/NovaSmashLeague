using UnityEngine;

public class DEBUG_VFXActivatorInput : MonoBehaviour
{
    [Tooltip("The VFXActivator to debug")]
    public VFXActivator vfxActivator;
    
    void Update()
    {
        if (Input.GetButtonDown("Debug1"))
        {
            vfxActivator.StartVFX();
        }
        else if(Input.GetButtonDown("Debug2"))
        {
            vfxActivator.StopVFX();
        }
    }
}
