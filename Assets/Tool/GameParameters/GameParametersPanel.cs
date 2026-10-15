using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameParametersPanel : MonoBehaviour
{
    [Header("Dropdown Items :")]
    [SerializeField] private TMP_Dropdown _dropdownSelectDevice;
    [SerializeField] private TMP_Dropdown _dropdownSelectTeam;
    [SerializeField] private TMP_Dropdown _dropdownSelectCharacter;

    [Header("Items :")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private List<SO_Character> _characters;
    private Dictionary<string, SO_Character> _characterNameToSO = new Dictionary<string, SO_Character>();
    private Dictionary<string, int> _devicesNameToIndex = new Dictionary<string, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        InitDropDowns();
    }

    public void InitDropDowns()
    {
        InitDropdownSelectDevicesOptions();
        InitDropdownSelectCharacterOptions();
        InitDropdownSelectTeamOptions();
    }

    #region Dropdown Select Device

    private void InitDropdownSelectDevicesOptions()
    {
        List<string> textsOptions = new List<string>();
        int gamepadIndex = 1;
        int keyboardIndex = 1;
        _devicesNameToIndex.Clear();

        for (int i = 0; i < InputSystem.devices.Count; i++)
        {
            if (InputSystem.devices[i].device is Gamepad)
            {
                textsOptions.Add("Gamepad " + gamepadIndex);
                _devicesNameToIndex.Add(("Gamepad " + gamepadIndex), i);
                gamepadIndex++;
            }
            else if (InputSystem.devices[i].device is Keyboard)
            {
                textsOptions.Add("Keyboard " + keyboardIndex);
                _devicesNameToIndex.Add(("Keyboard " + keyboardIndex), i);
                keyboardIndex++;
            }
        }

        _dropdownSelectDevice.ClearOptions();
        _dropdownSelectDevice.AddOptions(textsOptions);
    }


    public void AssignedNewDeviceFromName()
    {
        _devicesNameToIndex.TryGetValue(_dropdownSelectDevice.captionText.text, out var indexCurretnDevices);

        Gamepad gamepad = InputSystem.devices[indexCurretnDevices] as Gamepad;
        if (gamepad != null)
        {
            StartCoroutine(VibrateGamepad(gamepad));
        }

        // TODO assigned gamepad to player
        //...
    }

    private System.Collections.IEnumerator VibrateGamepad(Gamepad gamepad)
    {
        gamepad.SetMotorSpeeds(0.5f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        gamepad.ResetHaptics();
    }

    #endregion

    #region Dropdown Select Character

    private void InitDropdownSelectCharacterOptions()
    {
        List<string> textsOptions = new List<string>();
        _characterNameToSO.Clear();
        foreach(SO_Character character in _characters)
        {
            textsOptions.Add(character.characterName);
            _characterNameToSO.Add(character.characterName, character);
        }
        _dropdownSelectCharacter.ClearOptions();
        _dropdownSelectCharacter.AddOptions(textsOptions);
    }

    public void AssignedNewCharacter()
    {
        _characterNameToSO.TryGetValue(_dropdownSelectCharacter.captionText.text, out var character);

        // TODO assigned character to player
        //...
    }

    #endregion

    #region Dropdown Select Team

    private void InitDropdownSelectTeamOptions()
    {
        List<string> textsOptions = new List<string>(); //Enum.GetValues(typeof(Team)).ToList<string>();
        
        foreach (Team team in Enum.GetValues(typeof(Team)))
        {
            textsOptions.Add(team.ToString());
        }
        _dropdownSelectTeam.ClearOptions();
        _dropdownSelectTeam.AddOptions(textsOptions);
    }

    public void AssignedNewTeam()
    {
        foreach (Team team in Enum.GetValues(typeof(Team)))
        {
            if (_dropdownSelectTeam.captionText.text == team.ToString())
            {
                // TODO Assigned team
                // ...
            }
        }
    }

    #endregion
}
