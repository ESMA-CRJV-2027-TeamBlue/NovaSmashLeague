using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;

public class CameraFollow : MonoBehaviour
{
    #region Components
    [Header("Components needed :")]
    [Tooltip("Ball gameObject.")]
    [SerializeField] private GameObject _ball;
    private Rigidbody _ballRb;
    [Tooltip("Player gameObject's transform.")]
    [SerializeField] private Transform _player;
    [Tooltip("Camera Pivot On player gameObject's transform.")]
    [SerializeField] private Transform _pivotOnPlayer;
    [Tooltip("Camera Pivot Offset from player gameObject's transform.")]
    [SerializeField] private Transform _pivotOffset;
    private Camera _camera;
    #endregion

    #region Parameters
    [Tooltip("ScriptableObject CameraParameters.")]
    [SerializeField] private SO_CameraParameters _cameraParameters;
    #endregion

    private bool _isApex = false;
    private float _maxAngleX;

    #region DEBUG
    [Header("DEBUG Parameters :")]
    [Tooltip("DEBUG Variable simulating the ballSpeed value.")]
    public int _ballSpeedDebug;

    private float rotationX = 0f;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ballRb = _ball.GetComponent<Rigidbody>();
        _camera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        _maxAngleX = ComputeMaxXAngleForHeight();
        _isApex = ApexFollowCondition();
        if (_isApex)
        {
            ApexFollowPolish();
        }
        else
        {
            OffsetY();
            RotationX();
        }
        RotationY();
        PivotFollow();
        BallSpeedDEBUG();
    }

    private void PivotFollow()
    {
        _pivotOnPlayer.transform.position = Vector3.Lerp(_pivotOnPlayer.transform.position, _player.transform.position, 10f * Time.deltaTime);
    }

    /// <summary>
    /// Rotate the camera pivot on Y, to follow the ball horizontaly.
    /// </summary>
    private void RotationY()
    {
        Vector3 direction = _ball.transform.position - _pivotOnPlayer.transform.position;
        direction.y = 0f;
        Quaternion TargetRotation = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.Lerp(_pivotOnPlayer.rotation, TargetRotation, Mathf.Lerp(_cameraParameters._dampingRotationFollow, _cameraParameters._dampingRotationFollow * 10, 1- _ballSpeedDebug / 10f) * Time.deltaTime);
        _pivotOnPlayer.eulerAngles = new Vector3(_pivotOnPlayer.localEulerAngles.x, rotation.eulerAngles.y, 0f);
    }

    /// <summary>
    /// Move the camera on Y, from the ball position in the viewport, to follow the ball verticaly.
    /// </summary>
    private void OffsetYOLD()
    {
        Vector3 ballScreenPosition = _camera.WorldToViewportPoint(_ball.transform.position);
        float ecartHightBallPlyaer = _ball.transform.position.y - _player.position.y;

        if (ecartHightBallPlyaer < -2f || ecartHightBallPlyaer > 2f)
        {
            if ((ballScreenPosition.y > 0.8f && transform.localPosition.y < _cameraParameters._maxCameraHeight) || (ballScreenPosition.y < 0.8f && transform.localPosition.y > _cameraParameters._minCameraHeight) )
            {
                float newHight = transform.localPosition.y + (_ballRb.linearVelocity.y * Mathf.Lerp(_cameraParameters._dampingOffsetFollow, _cameraParameters._dampingOffsetFollow * 10, 1 - _ballSpeedDebug / 10f));
                transform.localPosition = new Vector3(transform.localPosition.x,Mathf.Clamp(newHight, _cameraParameters._minCameraHeight, _cameraParameters._maxCameraHeight) , transform.localPosition.z);
            }
        }
    }

    private void OffsetY()
    {
        Vector3 ballScreenPosition = _camera.WorldToViewportPoint(_ball.transform.position);

        Vector3 unrotatedBallPosition = Quaternion.AngleAxis(-_camera.transform.localEulerAngles.x, _camera.transform.right) * _ball.transform.position;
        Vector3 unrotatedCameraPosition = Quaternion.AngleAxis(-_camera.transform.localEulerAngles.x, _camera.transform.right) * _camera.transform.position;
        float offsetP = 0.5f;
        Vector3 targetPosition = new Vector3(0f, _pivotOffset.localPosition.y, 0f);

        if (ballScreenPosition.y > _cameraParameters._topLimitToOffset)
        {
            float Y = unrotatedBallPosition.y - unrotatedCameraPosition.y;
            float delta = Y - ((_cameraParameters._topLimitToOffset - offsetP) * Y) / (ballScreenPosition.y-offsetP);
            targetPosition = _pivotOffset.localPosition + Vector3.up * delta;
        }
        else if (ballScreenPosition.y < _cameraParameters._bottomLimitToOffset)
        {
            float Y = unrotatedBallPosition.y - unrotatedCameraPosition.y;
            float delta = Y - ((_cameraParameters._bottomLimitToOffset- offsetP) *Y) / (ballScreenPosition.y- offsetP);
            targetPosition = _pivotOffset.localPosition + Vector3.up * delta;
        }
        _pivotOffset.localPosition = new Vector3(_pivotOffset.localPosition.x, Mathf.Clamp(targetPosition.y, _cameraParameters._minCameraHeight, _cameraParameters._maxCameraHeight), _pivotOffset.localPosition.z);
    }

    private void OffsetYPolish()
    {
        Vector3 ballScreenPosition = _camera.WorldToViewportPoint(_ball.transform.position);

        Vector3 unrotatedBallPosition = Quaternion.AngleAxis(-_camera.transform.localEulerAngles.x, _camera.transform.right) * Quaternion.AngleAxis(-_pivotOffset.localEulerAngles.x, _pivotOffset.right) * _ball.transform.position;
        Vector3 unrotatedCameraPivotPosition = Quaternion.AngleAxis(-_camera.transform.localEulerAngles.x, _camera.transform.right) * Quaternion.AngleAxis(-_pivotOffset.localEulerAngles.x, _pivotOffset.right) * _pivotOffset.position;
        Vector3 unrotatedCameraPosition = Quaternion.AngleAxis(-_camera.transform.localEulerAngles.x, _camera.transform.right) * Quaternion.AngleAxis(-_pivotOffset.localEulerAngles.x, _pivotOffset.right) * transform.position;
        float offsetP = 0.5f;
        Vector3 targetPosition = new Vector3(0f, _pivotOffset.localPosition.y, 0f);

        if (ballScreenPosition.y > _cameraParameters._topLimitToOffset)
        {
            float Y = unrotatedBallPosition.y - unrotatedCameraPosition.y;
            float delta = Y - ((_cameraParameters._topLimitToOffset - offsetP) * Y) / (ballScreenPosition.y - offsetP);
            targetPosition = _pivotOffset.localPosition + Vector3.up * delta;
        }
        else if (ballScreenPosition.y < _cameraParameters._bottomLimitToOffset)
        {
            float Y = unrotatedBallPosition.y - unrotatedCameraPosition.y;
            float delta = Y - ((_cameraParameters._bottomLimitToOffset - offsetP) * Y) / (ballScreenPosition.y - offsetP);
            targetPosition = _pivotOffset.localPosition + Vector3.up * delta;
        }
        _pivotOffset.localPosition = new Vector3(_pivotOffset.localPosition.x, Mathf.Clamp(targetPosition.y, _cameraParameters._minCameraHeight, _cameraParameters._maxCameraHeight), _pivotOffset.localPosition.z);
    }

    /// <summary>
    /// Rotate the camera on X.
    /// </summary>
    private void RotationX()
    {
        _pivotOffset.localEulerAngles = new Vector3(_maxAngleX, _pivotOffset.localEulerAngles.y, _pivotOffset.localEulerAngles.z);
    }

    /// <summary>
    /// Calculate the camera rotation x value, from the hight of the character in the arena.
    /// </summary>
    /// <returns>Camera rotation X value (float).</returns>
    private float ComputeMaxXAngleForHeight()
    {
        float percent = _player.position.y / (_cameraParameters._maxPositionYInArena - _cameraParameters._minPositionYInArena);
        float rotationX = Mathf.Lerp(_cameraParameters._minCameraRotationX, _cameraParameters._maxCameraRotationX, percent);
        return rotationX;
    }

    /// <summary>
    /// Apex Follow Condition that switch between normal follow and apex follow.
    /// </summary>
    /// <returns>True or False.</returns>
    private bool ApexFollowCondition()
    {
        bool newApex = _isApex;
        Vector3 ballScreenPosition = _camera.WorldToViewportPoint(_ball.transform.position);
        if (!_isApex)
        {
            if(_pivotOffset.localPosition.y >= _cameraParameters._maxCameraHeight && ballScreenPosition.y >= _cameraParameters._BallPositionToViewportApexFollow)
            {
                newApex = true;
            }
        }
        else
        {
            var refAngle = _pivotOffset.eulerAngles.x;
            if (refAngle > 180f) refAngle -= 360f;
            //if(ballScreenPosition.y < _cameraParameters._BallPositionToViewportApexFollow-0.1f)
            if (refAngle >= _maxAngleX)
            {
                newApex = false;
            }
        }
        //if(_isApex && Mathf.DeltaAngle(_pivotOffset.localEulerAngles.x, _maxAngleX) <0.01f)
        //{
        //    _isApex = false;
        //}
        //else if (!_isApex && _pivotOffset.localPosition.y >= _cameraParameters._maxCameraHeight && ballScreenPosition.y >= _cameraParameters._BallPositionToViewportApexFollow)
        //{
        //    _isApex = true;
        //}
        return newApex;
    }

    /// <summary>
    /// Apex Follow that permite to follow the ball when the normal follow get to his limit.
    /// </summary>
    private void ApexFollow()
    {
        Vector3 ballScreenPosition = _camera.WorldToViewportPoint(_ball.transform.position);
        if (ballScreenPosition.y > 0.9f)
        {
            float rotationA = Mathf.Atan2(_cameraParameters._BallPositionToViewportApexFollow - 0.5f, -transform.localPosition.z) * Mathf.Rad2Deg - 10f;
            float rotationB = Mathf.Atan2(ballScreenPosition.y - 0.5f, -transform.localPosition.z) * Mathf.Rad2Deg - 10f;
            float targetRotationX = rotationB - rotationA;
            rotationX = _pivotOffset.localEulerAngles.x - targetRotationX;
            if (rotationX > 180f) rotationX -= 360f;
            _pivotOffset.localEulerAngles = new Vector3(Mathf.Clamp(rotationX, _cameraParameters._minCameraRotationXApexFollow, ComputeMaxXAngleForHeight()), _pivotOffset.transform.localEulerAngles.y, _pivotOffset.transform.localEulerAngles.z);
        }
        else if (ballScreenPosition.y < 0.8f)
        {
            float rotationA = Mathf.Atan2(_cameraParameters._BallPositionToViewportApexFollow - 0.5f, -transform.localPosition.z) * Mathf.Rad2Deg + 10f;
            float rotationB = Mathf.Atan2(ballScreenPosition.y - 0.5f, -transform.localPosition.z) * Mathf.Rad2Deg + 10f;
            float targetRotationX = rotationB - rotationA;
            rotationX = _pivotOffset.localEulerAngles.x - targetRotationX;
            if (rotationX > 180f) rotationX -= 360f;
            _pivotOffset.localEulerAngles = new Vector3(Mathf.Clamp(rotationX, _cameraParameters._minCameraRotationXApexFollow, ComputeMaxXAngleForHeight()), _pivotOffset.transform.localEulerAngles.y, _pivotOffset.transform.localEulerAngles.z);
        }
    }

    /// <summary>
    /// Apex Follow that permite to follow the ball when the normal follow get to his limit.
    /// </summary>
    private void ApexFollowPolish()
    {
        Vector3 unrotatedBallPosition = Quaternion.AngleAxis(-_camera.transform.localEulerAngles.x, _camera.transform.right) * Quaternion.AngleAxis(-_pivotOffset.localEulerAngles.x, _pivotOffset.right) * _ball.transform.position;
        Vector3 unrotatedCameraPivotPosition = Quaternion.AngleAxis(-_camera.transform.localEulerAngles.x, _camera.transform.right) * Quaternion.AngleAxis(-_pivotOffset.localEulerAngles.x, _pivotOffset.right) * _pivotOffset.position;
        Vector3 unrotatedCameraPosition = Quaternion.AngleAxis(-_camera.transform.localEulerAngles.x, _camera.transform.right) * Quaternion.AngleAxis(-_pivotOffset.localEulerAngles.x, _pivotOffset.right) * transform.position;
        float offsetP = 0.5f;
        Vector3 ballScreenPosition = _camera.WorldToViewportPoint(_ball.transform.position);

        if (ballScreenPosition.y > _cameraParameters._BallPositionToViewportApexFollow || ballScreenPosition.y < _cameraParameters._BallPositionToViewportApexFollow)
        {
            float Y = unrotatedBallPosition.y - unrotatedCameraPivotPosition.y;
            float delta = Y - ((_cameraParameters._BallPositionToViewportApexFollow - offsetP) * Y) / (ballScreenPosition.y - offsetP);
            float distance = Vector3.Distance(unrotatedCameraPosition, unrotatedBallPosition);

            float targetRotation = Mathf.Atan2(delta, distance) * Mathf.Rad2Deg;
            
            rotationX = Mathf.DeltaAngle(targetRotation,_pivotOffset.localEulerAngles.x);
            // Damping PlaceHolder
            //rotationX = Mathf.Lerp(rotationX, _pivotOffset.localEulerAngles.x, Mathf.Lerp(0f, _cameraParameters._dampingApexFollow,_ballSpeedDebug / 10f) * Time.deltaTime);
            //rotationX = Mathf.Lerp(_pivotOffset.localEulerAngles.x, rotationX, Mathf.Lerp(_cameraParameters._dampingApexFollow, _cameraParameters._dampingApexFollow * 10f, 1 - _ballSpeedDebug / 10f) * Time.deltaTime);
            _pivotOffset.transform.localEulerAngles = new Vector3(Mathf.Clamp(rotationX, _cameraParameters._minCameraRotationXApexFollow, _maxAngleX), _pivotOffset.transform.localEulerAngles.y, _pivotOffset.transform.localEulerAngles.z);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(500f, 10f, 800f, 100f), "Wanted Angle X Value : " + rotationX);
        GUI.Label(new Rect(500f, 30f, 800f, 100f), "Max Angles X : " + _maxAngleX);
        GUI.Label(new Rect(500f, 50f, 800f, 100f), "Local pivot euleur X Value : " + _pivotOffset.localEulerAngles.x);
        GUI.Label(new Rect(500f, 70f, 800f, 100f), "isApex : " + _isApex);
        GUI.Label(new Rect(500f, 110f, 800f, 100f), "Current camera height : " + _pivotOffset.localPosition.y);
        GUI.Label(new Rect(500f, 130f, 800f, 100f), "Max camera height : " + _cameraParameters._maxCameraHeight);
    }

    /// <summary>
    /// Modify the ballSpeedDEBUG value.
    /// </summary>
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
