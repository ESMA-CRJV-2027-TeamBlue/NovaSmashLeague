using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameParametersManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdownSelectScene;
    [SerializeField] private InputActionReference _playerTryToConect;
    public bool openGameParametersIsPressed;
    private bool _isActive;

    [SerializeField] private Canvas _window;

    private void Start()
    {
        InitDropdownSelectScene();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (_playerTryToConect.action.WasPressedThisFrame())
        {
            //CheckInputDevicePressed(_playerController.controls.);
            
            //Debug.Log("Name : " + _playerTryToConect.action.activeControl.device);
            InputDevice device = _playerTryToConect.action.activeControl.device;
            /*
            Debug.Log("ID : " + device.deviceId);
            bool test = (device.deviceId == InputSystem.devices[3].deviceId);
            Debug.Log("Bool : "+  test );
            Debug.Log("index : " + FindIndexDevice(device));
            
            if (CheckDeviceIsUsed(FindIndexDevice(device)))
            {
                CreateNewPlayer(FindIndexDevice(device));
            }
        }
        */
        if (openGameParametersIsPressed)
        {
            OpenCloseMenu();
        }
    }

    private void OpenCloseMenu()
    {
        if (_window.isActiveAndEnabled)
        {
            _window.enabled = false;
        }
        else
        {
            _window.enabled = true;
        }
    }


    private void InitDropdownSelectScene()
    {
        List<string> scenes = new List<string>(); 
        int sceneIndex = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneIndex; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            scenes.Add(sceneName);
        }

        _dropdownSelectScene.ClearOptions();
        _dropdownSelectScene.AddOptions(scenes);
    }

    public void StartButton()
    {
        string sceneName = _dropdownSelectScene.captionText.text;
        SceneManager.LoadSceneAsync(sceneName);
    }
}
