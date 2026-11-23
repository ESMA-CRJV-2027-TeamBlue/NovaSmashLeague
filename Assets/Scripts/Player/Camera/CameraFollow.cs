using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Components
    [Header("Components needed :")]
    [Tooltip("Ball gameObject")]
    [SerializeField] private GameObject _ball;
    private Rigidbody _ballRb;
    [Tooltip("Player gameObject's transform")]
    [SerializeField] private Transform _player;
    [Tooltip("Camera Pivot gameObject's transform")]
    [SerializeField] private Transform _pivot;
    #endregion

    #region Global Parameters
    [Header("Global Parameters :")]
    [Tooltip("Damping force of the camera follow In Y rotation")]
    [SerializeField] private float _dampingRotationFollow;
    #endregion

    #region Offset Y
    [Header("Offset Position on Y, parameters :")]
    [Tooltip("Maximum hight on offset position on Y axis of the camera")]
    [SerializeField] private float _maxCameraHight;
    [Tooltip("Minimum hight on offset position on Y axis of the camera")]
    [SerializeField] private float _minCameraHight;
    [Tooltip("")]
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
    [Header("Should Parameters :")]
    [Tooltip("")]
    [SerializeField] private float _minCameraRotationXShould;
    [Tooltip("")]
    [SerializeField] private float _minCameraPositionYShould;
    [Tooltip("")]
    [SerializeField] private float _limitBallPositionToScreenInPercentShould;
    [Tooltip("Damping force of the camera follow in Should Rotation")]
    [SerializeField] private float _dampingShouldFollow;
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
        if(ShouldCondition())
        {
            Debug.Log("Should");
            Should();
        }
        else
        {
            Debug.Log("Normal");
            OffsetY();
            RotationX();
        }
        BallSpeedDEBUG();
    }

    /// <summary>
    /// 
    /// </summary>
    private void RotationY()
    {
        Vector3 direction = _ball.transform.position - _pivot.position;
        Quaternion TargetRotation = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.Lerp(_pivot.rotation, TargetRotation, Mathf.Lerp(_dampingRotationFollow, _dampingRotationFollow * 10, 1 - _ballSpeedDebug / 10f) * Time.deltaTime);
        _pivot.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    private void RotationX()
    {
        transform.localEulerAngles = new Vector3(CameraAngleXValue(), transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float CameraAngleXValue()
    {
        float percent = _player.position.y / (_maxPositionYInArena - _minPositionYInArena);
        float rotationX = Mathf.Lerp(_minCameraRotationX, _maxCameraRotationX, percent);
        return rotationX;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>True or False.</returns>
    private bool ShouldCondition()
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
    /// 
    /// </summary>
    private void Should()
    {
        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.transform.position);

        if ( (ballScreenPosition.y > _limitBallPositionToScreenInPercentShould && transform.localPosition.y > _minCameraPositionYShould) || (ballScreenPosition.y < _limitBallPositionToScreenInPercentShould && transform.localPosition.y < _maxCameraHight) )
        {
            float positionY = transform.localPosition.y - (_ballRb.linearVelocity.y * Mathf.Lerp(_dampingShouldFollow, _dampingShouldFollow * 10, 1 - _ballSpeedDebug / 10f));
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(positionY, _minCameraPositionYShould, _maxCameraHight), transform.localPosition.z);

            float rotationX = Mathf.Lerp(_minCameraRotationXShould, CameraAngleXValue(), Mathf.InverseLerp(_minCameraPositionYShould, _maxCameraHight, transform.localPosition.y));
            transform.localEulerAngles = new Vector3(Mathf.Clamp(rotationX, _minCameraRotationXShould, CameraAngleXValue()), transform.localEulerAngles.y, transform.localEulerAngles.z);
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
