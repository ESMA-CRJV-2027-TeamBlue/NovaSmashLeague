using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameParametersManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _playerTryToConect;
    public bool openGameParametersIsPressed;
    private bool _isActive;

    [SerializeField] private Canvas _window;
    [SerializeField] private List<TMP_Dropdown> _dropdownSelectControllers;
    private List<int> _listControllers;

    [SerializeField] private List<SO_PlayerInfos> _playersList;
    [SerializeField] private List<GameObject> _playersUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<string> textsOptions = new List<string>();
        //_playerController.performed +=
        int gamepadIndex = 1;
        int keyboardIndex = 1;

        for (int i = 0; i < InputSystem.devices.Count; i++)
        {
            _listControllers.Add(i);
            if (InputSystem.devices[i].device is Gamepad)
            {
                textsOptions.Add("Gamepad " + gamepadIndex);
                gamepadIndex++;
            }
            else if (InputSystem.devices[i].device is Keyboard)
            {
                textsOptions.Add("Keyboard " + keyboardIndex);
                keyboardIndex++;
            }
        }

        foreach (TMP_Dropdown dropdown in _dropdownSelectControllers)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(textsOptions);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
            */
            if (CheckDeviceIsUsed(FindIndexDevice(device)))
            {
                CreateNewPlayer(FindIndexDevice(device));
            }
        }

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

    private int FindIndexDevice(InputDevice device)
    {
        for (int i =0; i <= InputSystem.devices.Count - 1; i++) 
        {
                if (device.deviceId == InputSystem.devices[i].deviceId)
                {
                    return i;
                }
        }
        return -1;
    }

    private bool CheckDeviceIsUsed(int index)
    {
        if (_playersList.Count == 0)
        {
            return true;
        }

        else
        {
            foreach (SO_PlayerInfos playerInfos in _playersList)
            {
                if (playerInfos.deviceNumber == -1)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private void CreateNewPlayer(int index)
    {
        
    }
}
