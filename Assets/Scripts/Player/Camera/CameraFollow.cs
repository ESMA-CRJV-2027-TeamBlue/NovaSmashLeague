using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _ball;
    [SerializeField] private Transform _player; 
    [SerializeField] private Transform _pivot;
    [SerializeField] private float _dampingFollow;
    [SerializeField] private float _maxHightCamera;
    [SerializeField] private float _minHightCamera;

    #region DEBUG
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
        /*
        float ecartHight = _ball.position.y - transform.position.y;
        if ( ecartHight >= 5f)
        {
            float newhightCamera = _ball.position.y - 5f;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(newhightCamera, _minHightCamera, _maxHightCamera), transform.localPosition.z);
        }
        else if( ecartHight <= -5f)
        {
            float newhightCamera = _ball.position.y + 5f;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(newhightCamera, _minHightCamera, _maxHightCamera), transform.localPosition.z);
        }
        */

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
