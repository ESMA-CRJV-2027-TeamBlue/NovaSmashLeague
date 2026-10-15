using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameParametersPanel : MonoBehaviour
{
    [Header("Dropdown Items :")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TMP_Dropdown _dropdownSelectDevice;
    [SerializeField] private TMP_Dropdown _dropdownSelectTeam;
    [SerializeField] private TMP_Dropdown _dropdownSelectCharacter;

    [Header("Items :")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private List<SO_Character> _characters;
    private Dictionary<string, SO_Character> _characterNameToSO = new Dictionary<string, SO_Character>();
    private Dictionary<string, int> _devicesNameToIndex = new Dictionary<string, int>();

    #region PlayerData
    private Team _team;
    private int _deviceId;
    private SO_Character _character;
    #endregion

    // Called when object the panel is switching to enable
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

    public void TransferDataToPlayer()
    {
        GameObject player = Instantiate(_playerPrefab, new Vector3(1000f,1000f,1000f), new Quaternion(0f,0f,0f,0f));
        player.GetComponent<PlayerInfos>().SetNewPlayerInfos(_team, _deviceId, _character);
        player.tag = _text.text;
        DontDestroyOnLoad(player);
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
        _deviceId = indexCurretnDevices;
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
        _character = character;
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
                _team = team;
            }
        }
    }

    #endregion
}
