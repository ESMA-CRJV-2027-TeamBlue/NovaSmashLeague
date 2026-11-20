using System;
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

    #region Should
    [Header("Should Parameters :")]
    // param => SpeedTransition / _minAngleXShould / _minOffsetYShould / _minPositionScreenPercent / _maxPositionScreenPercent
    private bool _switch = false;
    private float lerpPercentShould = 0f;
    [Tooltip("")]
    [SerializeField] private float _speedTransitionShould;
    [Tooltip("")]
    [SerializeField] private float _minRotationXShould;
    [Tooltip("")]
    [SerializeField] private float _minPositionYShould;
    [Tooltip("")]
    [SerializeField] private float _minBallPositionToScreenInPercentShould;
    [Tooltip("")]
    [SerializeField] private float _maxBallPositionToScreenInPercentShould;
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
        //Debug.Log(Camera.main.WorldToViewportPoint(_ball.transform.position).y);
    }

    /// <summary>
    /// 
    /// </summary>
    private void RotationY()
    {
        Vector3 direction = _ball.transform.position - _pivot.position;
        Quaternion TargetRotation = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.Lerp(_pivot.rotation, TargetRotation, Mathf.Lerp(_dampingFollow, _dampingFollow * 10, 1 - _ballSpeedDebug / 10f) * Time.deltaTime);
        _pivot.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
    }

    private void OffsetY()
    {
        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.transform.position);
        float ecartHightBallPlyaer = _ball.transform.position.y - _player.position.y;

        if (ecartHightBallPlyaer < -2f || ecartHightBallPlyaer > 2f)
        {
            if ((ballScreenPosition.y > 0.8f && transform.localPosition.y < _maxHightCamera) || (ballScreenPosition.y < 0.8f && transform.localPosition.y > _minHightCamera) )
            {
                float newHight = transform.localPosition.y + (_ballRb.linearVelocity.y * Time.deltaTime); // ajouter dans la multiplication un damping si besoin
                transform.localPosition = new Vector3(transform.localPosition.x,Mathf.Clamp(newHight,_minHightCamera,_maxHightCamera) , transform.localPosition.z);
            }
        }
    }

    private void RotationX()
    {
        transform.localEulerAngles = new Vector3(CameraAngleXValue(), transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    private float CameraAngleXValue()
    {
        float percent = _player.position.y / (_maxPositionYInArena - _minPositionYInArena);
        float rotationX = Mathf.Lerp(_minRotationX, _maxRotationX, percent);
        //Debug.Log("Percent position Y in Arena : " + percent);
        //Debug.Log("Camera Angle X value : " + rotationX);
        return rotationX;
    }

    private bool ShouldCondition()
    {
        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.transform.position);
        if (!_switch && transform.localPosition.y >= _maxHightCamera && ballScreenPosition.y >= 0.9f)
        {
            _switch = true;
            Debug.Log("Switch True");
        }
        else if(_switch && transform.localPosition.y >= _maxHightCamera && ballScreenPosition.y <= 0.9f)
        {
            _switch = false;
            Debug.Log("Switch False");
        }

        //return false;
        return _switch;
    }

    private void Should()
    {
        // rotationX => Lerp [_minAngleXShould ; CameraAngleXValue()]
        // positionY => Lerp [_minOffsetYShould ; _maxOffsetY]
        // percentLerp => [(_ball.poisition.y > screen 0.95)=> --] [(_ball.position.y < scrren 0.9)=> ++]
        // param => SpeedTransition / _minAngleXShould / _minOffsetYShould / _minPositionScreenPercent / _maxPositionScreenPercent

        /*
        if (Camera.main.WorldToViewportPoint(_ball.transform.position).y >0.9f)
        {
            lerpPercentShould += Mathf.Lerp(_speedTransitionShould, _speedTransitionShould*10f, 1f - _ballSpeedDebug / 10f) * Time.deltaTime;
        }
        else if(Camera.main.WorldToViewportPoint(_ball.transform.position).y  < 0.8f)
        {
            //lerpPercentShould -= _speedTransitionShould * Time.deltaTime;
            lerpPercentShould -= Mathf.Lerp(_speedTransitionShould, _speedTransitionShould*10f, 1f - _ballSpeedDebug / 10f) * Time.deltaTime;
        }
        */

        Vector3 ballScreenPosition = Camera.main.WorldToViewportPoint(_ball.transform.position);

        if ( (ballScreenPosition.y > 0.8f && transform.localPosition.y > _minPositionYShould) || (ballScreenPosition.y < 0.8f && transform.localPosition.y < _maxHightCamera) )
        {
            //float rotationX = transform.localEulerAngles.y - (_ballRb.linearVelocity.y * 10f * Time.deltaTime); // ajouter dans la multiplication un damping si besoin

            float positionY = transform.localPosition.y - (_ballRb.linearVelocity.y * 0.5f * Time.deltaTime); // Damping possible
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(positionY, _minPositionYShould, _maxHightCamera), transform.localPosition.z);

            //float rotationX = transform.localEulerAngles.y - (_ballRb.linearVelocity.y * Time.deltaTime);

            float rotationX = Mathf.Lerp(_minRotationXShould, CameraAngleXValue(), Mathf.InverseLerp(_minPositionYShould, _maxHightCamera, transform.localPosition.y));
            // (transform.localPosition.y-(-_minPositionYShould)) / (_maxHightCamera - (-_minPositionYShould))
            
            transform.localEulerAngles = new Vector3(Mathf.Clamp(rotationX, _minRotationXShould, CameraAngleXValue()), transform.localEulerAngles.y, transform.localEulerAngles.z);
            float resultDEBUG = (transform.localPosition.y - (-_minPositionYShould)) / (_maxHightCamera - (-_minPositionYShould));
            Debug.Log(resultDEBUG);
        }
        //
        //Mathf.Clamp01(lerpPercentShould);
        //float rotationX = Mathf.Lerp(CameraAngleXValue(), _minRotationXShould, lerpPercentShould);
        //float positionY = Mathf.Lerp(_maxHightCamera,_minPositionYShould,lerpPercentShould);
        //transform.localEulerAngles = new Vector3(rotationX,transform.localEulerAngles.y, transform.localEulerAngles.z);
        //transform.localPosition = new Vector3(transform.localPosition.x,positionY,transform.localPosition.z);
    }

    private float LerpTest(float valueA, float valueB, float t)
    {
        //valueB - valueB;

        return 0f;
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
