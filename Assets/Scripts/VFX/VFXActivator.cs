using UnityEngine;
using UnityEngine.VFX;

public class VFXActivator : MonoBehaviour
{
    public VisualEffect vfxToActivate;
    
    public void StartVFX()
    {
        vfxToActivate.Play();
    }

    public void StopVFX()
    {
        vfxToActivate.Stop();
    }
}
