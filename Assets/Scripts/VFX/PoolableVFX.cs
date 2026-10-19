using UnityEngine;

public class PoolableVFX : MonoBehaviour
{
    [Tooltip("The VXFActivator to call")]
    public VFXActivator vfxActivator;
    
    private void OnEnable()
    {
        vfxActivator.StartVFX();
    }
}
