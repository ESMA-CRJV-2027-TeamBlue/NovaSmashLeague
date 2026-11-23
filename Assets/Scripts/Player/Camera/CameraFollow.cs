using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Components
    [Header("Components needed :")]
    [Tooltip("Ball gameObject.")]
    [SerializeField] private GameObject _ball;
    private Rigidbody _ballRb;
    [Tooltip("Player gameObject's transform.")]
    [SerializeField] private Transform _player;
    [Tooltip("Camera Pivot gameObject's transform.")]
    [SerializeField] private Transform _pivot;
    #endregion

    #region Parameters
    [Tooltip("ScriptableObject CameraParameters.")]
    [SerializeField] private SO_CameraParameters _cameraParameters;
    #endregion

    private bool _switch = false;

    #region DEBUG
    [Header("DEBUG Parameters :")]
    [Tooltip("DEBUG Variable simulating the ballSpeed value.")]
    public int _ballSpeedDebug;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ballRb = _ball.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RotationY();
        if(ApexFollowCondition())
        {
            ApexFollow();
        }
        else
        {
            OffsetY();
            RotationX();
        }
        BallSpeedDEBUG();
    }

    /// <summary>
    /// Rotate the camera pivot on Y, to follow the ball horizontaly.
    /// </summary>
    private void RotationY()
    {
        Vector3 direction = _ball.transform.position - _pivot.position;
        Quaternion TargetRotation = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.Lerp(_pivot.rotation, TargetRotation, Mathf.Lerp(_cameraParameters._dampingRotationFollow, _cameraParameters._dampingRotationFollow * 10, 1 - _ballSpeedDebug / 10f) * Time.deltaTime);
        _pivot.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
    }

    /// <summary>
    /// Move the camera on Y, from the ball position in the viewport, to follow the ball verticaly.
    /// </summary>
    private void OffsetY()
    {
        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.transform.position);
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

    /// <summary>
    /// Rotate the camera on X.
    /// </summary>
    private void RotationX()
    {
        transform.localEulerAngles = new Vector3(CameraAngleXValue(), transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    /// <summary>
    /// Calculate the camera rotation x value, from the hight of the character in the arena.
    /// </summary>
    /// <returns>Camera rotation X value (float).</returns>
    private float CameraAngleXValue()
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
        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.transform.position);
        if (!_switch && transform.localPosition.y >= _cameraParameters._maxCameraHeight && ballScreenPosition.y >= 0.9f)
        {
            _switch = true;
        }
        else if(_switch && transform.localPosition.y >= _cameraParameters._maxCameraHeight && ballScreenPosition.y <= 0.9f)
        {
            _switch = false;
        }
        return _switch;
    }

    /// <summary>
    /// Apex Follow that permite to follow the ball when the normal follow get to his limit.
    /// </summary>
    private void ApexFollow()
    {
        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.transform.position);

        if ( (ballScreenPosition.y > _cameraParameters._BallPositionToViewportApexFollow && transform.localPosition.y > _cameraParameters._minCameraPositionYApexFollow) || (ballScreenPosition.y < _cameraParameters._BallPositionToViewportApexFollow && transform.localPosition.y < _cameraParameters._maxCameraHeight) )
        {
            float positionY = transform.localPosition.y - (_ballRb.linearVelocity.y * Mathf.Lerp(_cameraParameters._dampingApexFollow, _cameraParameters._dampingApexFollow * 10, 1 - _ballSpeedDebug / 10f));
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(positionY, _cameraParameters._minCameraPositionYApexFollow, _cameraParameters._maxCameraHeight), transform.localPosition.z);

            float rotationX = Mathf.Lerp(_cameraParameters._minCameraRotationXApexFollow, CameraAngleXValue(), Mathf.InverseLerp(_cameraParameters._minCameraPositionYApexFollow, _cameraParameters._maxCameraHeight, transform.localPosition.y));
            transform.localEulerAngles = new Vector3(Mathf.Clamp(rotationX, _cameraParameters._minCameraRotationXApexFollow, CameraAngleXValue()), transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
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
