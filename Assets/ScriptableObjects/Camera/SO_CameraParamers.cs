using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/CameraParameters", order = 0)]
public class SO_CameraParameters : ScriptableObject
{
    #region Global Parameters
    [Header("Global Parameters :")]
    [Tooltip("Damping force of the camera follow In Y rotation.")]
    public float _dampingRotationFollow;
    #endregion

    #region Offset Y
    [Header("Offset Position on Y, parameters :")]
    [Tooltip("Maximum hight on offset position on Y axis of the camera.")]
    public float _maxCameraHeight;
    [Tooltip("Minimum hight on offset position on Y axis of the camera.")]
    public float _minCameraHeight;
    [Tooltip("Damping force of the camera Offset Follow In Y position.")]
    public float _dampingOffsetFollow;
    #endregion

    #region RotationX
    [Header("Camera Rotation on X Parameters :")]
    [Tooltip("Maximum Hight (position Y) reachable in the arena.")]
    public float _maxPositionYInArena;
    [Tooltip("Minimum Hight (position Y) reachable in the arena.")]
    public float _minPositionYInArena;
    [Tooltip("Maximum angle value of the camera rotation on X.")]
    public float _maxCameraRotationX;
    [Tooltip("Minimu angle value of the camera rotaiton on X.")]
    public float _minCameraRotationX;
    #endregion

    #region Should
    [Header("Apex Follow Parameters :")]
    [Tooltip("Minimal X Rotation of the camera on the Apex Follow.")]
    public float _minCameraRotationXApexFollow;
    [Tooltip("Minimal Y position of the camera on the Apex Follow.")]
    public float _minCameraPositionYApexFollow;
    [Tooltip("Limit of the ball position on viewport to orient the Apex follow. (in percent)")]
    public float _BallPositionToViewportApexFollow;
    [Tooltip("Damping force of the camera follow in Apex Follow.")]
    public float _dampingApexFollow;
    #endregion
}
