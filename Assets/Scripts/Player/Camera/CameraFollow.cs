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

    #region Global Parameters
    [Header("Global Parameters :")]
    [Tooltip("Damping force of the camera follow In Y rotation.")]
    [SerializeField] private float _dampingRotationFollow;
    #endregion

    #region Offset Y
    [Header("Offset Position on Y, parameters :")]
    [Tooltip("Maximum hight on offset position on Y axis of the camera.")]
    [SerializeField] private float _maxCameraHight;
    [Tooltip("Minimum hight on offset position on Y axis of the camera.")]
    [SerializeField] private float _minCameraHight;
    [Tooltip("Damping force of the camera Offset Follow In Y position.")]
    [SerializeField] private float _dampingOffsetFollow;
    #endregion

    #region RotationX
    [Header("Camera Rotation on X Parameters :")]
    [Tooltip("Maximum Hight (position Y) reachable in the arena.")]
    [SerializeField] private float _maxPositionYInArena;
    [Tooltip("Minimum Hight (position Y) reachable in the arena.")]
    [SerializeField] private float _minPositionYInArena;
    [Tooltip("Maximum angle value of the camera rotation on X.")]
    [SerializeField] private float _maxCameraRotationX;
    [Tooltip("Minimu angle value of the camera rotaiton on X.")]
    [SerializeField] private float _minCameraRotationX;
    #endregion

    #region Should
    [Header("Apex Follow Parameters :")]
    [Tooltip("Minimal X Rotation of the camera on the Apex Follow.")]
    [SerializeField] private float _minCameraRotationXApexFollow;
    [Tooltip("Minimal Y position of the camera on the Apex Follow.")]
    [SerializeField] private float _minCameraPositionYApexFollow;
    [Tooltip("Limit of the ball position on viewport to orient the Apex follow. (in percent)")]
    [SerializeField] private float _BallPositionToViewportApexFollow;
    [Tooltip("Damping force of the camera follow in Apex Follow.")]
    [SerializeField] private float _dampingApexFollow;
    private bool _switch = false;
    #endregion

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
        Quaternion rotation = Quaternion.Lerp(_pivot.rotation, TargetRotation, Mathf.Lerp(_dampingRotationFollow, _dampingRotationFollow * 10, 1 - _ballSpeedDebug / 10f) * Time.deltaTime);
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
            if ((ballScreenPosition.y > 0.8f && transform.localPosition.y < _maxCameraHight) || (ballScreenPosition.y < 0.8f && transform.localPosition.y > _minCameraHight) )
            {
                float newHight = transform.localPosition.y + (_ballRb.linearVelocity.y * Mathf.Lerp(_dampingOffsetFollow, _dampingOffsetFollow * 10, 1 - _ballSpeedDebug / 10f));
                transform.localPosition = new Vector3(transform.localPosition.x,Mathf.Clamp(newHight,_minCameraHight,_maxCameraHight) , transform.localPosition.z);
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
        float percent = _player.position.y / (_maxPositionYInArena - _minPositionYInArena);
        float rotationX = Mathf.Lerp(_minCameraRotationX, _maxCameraRotationX, percent);
        return rotationX;
    }

    /// <summary>
    /// Apex Follow Condition that switch between normal follow and apex follow.
    /// </summary>
    /// <returns>True or False.</returns>
    private bool ApexFollowCondition()
    {
        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.transform.position);
        if (!_switch && transform.localPosition.y >= _maxCameraHight && ballScreenPosition.y >= 0.9f)
        {
            _switch = true;
        }
        else if(_switch && transform.localPosition.y >= _maxCameraHight && ballScreenPosition.y <= 0.9f)
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

        if ( (ballScreenPosition.y > _BallPositionToViewportApexFollow && transform.localPosition.y > _minCameraPositionYApexFollow) || (ballScreenPosition.y < _BallPositionToViewportApexFollow && transform.localPosition.y < _maxCameraHight) )
        {
            float positionY = transform.localPosition.y - (_ballRb.linearVelocity.y * Mathf.Lerp(_dampingApexFollow, _dampingApexFollow * 10, 1 - _ballSpeedDebug / 10f));
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(positionY, _minCameraPositionYApexFollow, _maxCameraHight), transform.localPosition.z);

            float rotationX = Mathf.Lerp(_minCameraRotationXApexFollow, CameraAngleXValue(), Mathf.InverseLerp(_minCameraPositionYApexFollow, _maxCameraHight, transform.localPosition.y));
            transform.localEulerAngles = new Vector3(Mathf.Clamp(rotationX, _minCameraRotationXApexFollow, CameraAngleXValue()), transform.localEulerAngles.y, transform.localEulerAngles.z);
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
