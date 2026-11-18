using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Components
    [Header("Components needed :")]
    [Tooltip("Ball gameObject's transform")]
    [SerializeField] private Transform _ball;
    [Tooltip("Player gameObject's transform")]
    [SerializeField] private Transform _player;
    [Tooltip("Camera Pivot gameObject's transform")]
    [SerializeField] private Transform _pivot;
    #endregion

    #region Global Parameters
    [Header("Global Parameters :")]
    [Tooltip("Damping force of the camera follow")]
    [SerializeField] private float _dampingFollow;
    #endregion

    #region Offset Y
    [Header("Offset Position on Y, parameters :")]
    [Tooltip("Maximum hight on offset position on Y axis of the camera")]
    [SerializeField] private float _maxHightCamera;
    [Tooltip("Minimum hight on offset position on Y axis of the camera")]
    [SerializeField] private float _minHightCamera;
    #endregion

    #region RotationX
    [Header("Camera Rotation on X Parameters :")]
    [Tooltip("Maximum Hight (position Y) reachable in the arena.")]
    [SerializeField] private float _maxPositionYInArena;
    [Tooltip("Minimum Hight (position Y) reachable in the arena.")]
    [SerializeField] private float _minPositionYInArena;
    [Tooltip("Maximum angle value of the camera rotation on X.")]
    [SerializeField] private float _maxRotationX;
    [Tooltip("Minimu angle value of the camera rotaiton on X.")]
    [SerializeField] private float _minRotationX;
    #endregion

    #region DEBUG
    [Tooltip("DEBUG Variable simulating the ballSpeed value.")]
    public int _ballSpeedDebug;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotationY();
        OffsetY();
        RotationX();
        BallSpeedDEBUG();
    }

    private void RotationY()
    {
        Vector3 direction = _ball.position - _pivot.position;
        Quaternion TargetRotation = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.Lerp(_pivot.rotation, TargetRotation, Mathf.Lerp(_dampingFollow, _dampingFollow * 10, 1 - _ballSpeedDebug / 10f) * Time.deltaTime);
        _pivot.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
    }

    private void OffsetY()
    {
        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.position);
        float ecartHightBallPlyaer = _ball.position.y - _player.position.y;
        if (ecartHightBallPlyaer < -2f || ecartHightBallPlyaer > 2f)
        {
            if (ballScreenPosition.y > 0.8f && transform.localPosition.y < _maxHightCamera)
            {
                //float newHight = _maxHightCamera;
                float newHight = Mathf.Lerp(transform.localPosition.y, _maxHightCamera, Mathf.Lerp(_dampingFollow, _dampingFollow * 10f, 1f - _ballSpeedDebug / 10f) * Time.deltaTime);
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(newHight, _minHightCamera, _maxHightCamera), transform.localPosition.z);
            }
            else if (ballScreenPosition.y < 0.45f && transform.localPosition.y > _minHightCamera)
            {
                //float newHight = _minHightCamera;
                float newHight = Mathf.Lerp(transform.localPosition.y, _minHightCamera, Mathf.Lerp(_dampingFollow, _dampingFollow * 10f, 1f - _ballSpeedDebug / 10f) * Time.deltaTime);
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(newHight, _minHightCamera, _maxHightCamera), transform.localPosition.z);
            }
        }
    }

    private void RotationX()
    {
        float percent = _player.position.y / (_maxPositionYInArena - _minPositionYInArena);
        float rotationX = Mathf.Lerp(_minRotationX, _maxRotationX, percent);
        //Debug.Log("Percent position Y in Arena : " + percent);
        //Debug.Log("Camera Angle X value : " + rotationX);
        transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }


    private void BallSpeedDEBUG()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            _ballSpeedDebug++;
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            _ballSpeedDebug--;
        }
    }
}
