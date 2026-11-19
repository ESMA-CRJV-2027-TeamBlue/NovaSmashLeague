using UnityEngine;

public class DEBUG_AlignToCamera : MonoBehaviour
{
//Place on Canvas Object
// Text -> Canvas (this) -> Model Object (-> =  is parented to...)

    public Camera cameraToFace;

    private void LateUpdate() {
        transform.LookAt(cameraToFace.transform);
        transform.RotateAround(transform.position, transform.up, 180f);
    }
}