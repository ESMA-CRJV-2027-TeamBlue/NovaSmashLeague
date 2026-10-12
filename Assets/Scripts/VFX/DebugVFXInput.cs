using UnityEngine;

public class DebugVFXInput : MonoBehaviour
{
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
