using UnityEngine;
using UnityEngine.VFX;

public class VFXActivator : MonoBehaviour
{
    [Tooltip("The VFX to control")]
    public VisualEffect vfxToActivate;
    
    /// <summary>
    /// Activate the VFX
    /// </summary>
    public void StartVFX()
    {
        vfxToActivate.Play();
    }

    /// <summary>
    /// Deactivate the VFX
    /// </summary>
    public void StopVFX()
    {
        vfxToActivate.Stop();
    }
}
