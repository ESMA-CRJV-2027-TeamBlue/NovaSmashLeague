using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameParametersPanel : MonoBehaviour
{
    #region UI Items
    [Header("UI Items :")]
    [Tooltip("Text Player + Number Element.")]
    [SerializeField] private TextMeshProUGUI _text;
    [Tooltip("Dropdown Selection Device Element.")]
    [SerializeField] private TMP_Dropdown _dropdownSelectDevice;
    [Tooltip("Dropdown Selection Team Element.")]
    [SerializeField] private TMP_Dropdown _dropdownSelectTeam;
    [Tooltip("Dropdown Selection Character Element.")]
    [SerializeField] private TMP_Dropdown _dropdownSelectCharacter;
    [Tooltip("Toggle is Player playing Element.")]
    [SerializeField] private Toggle _toggleIsPlaying;
    #endregion

    #region Gameplay Items
    [Header("Gameplay Items :")]
    [Tooltip("Prefab Player.")]
    [SerializeField] private GameObject _playerPrefab;
    [Tooltip("List Scriptable Object Characters")]
    [SerializeField] private List<SO_Character> _characters;
    [Tooltip("Dictionnary with Key = Name of the Character and Value = ScriptableObject of the Character")]
    private Dictionary<string, SO_Character> _characterNameToSO = new Dictionary<string, SO_Character>();
    [Tooltip("Dictionnary with Key = Device's name and Value = Device's Index")]
    private Dictionary<string, int> _devicesNameToIndex = new Dictionary<string, int>();
    #endregion

    #region PlayerData
    [Tooltip("Enum team ment to be transfered to the player.")]
    private Team _team;
    [Tooltip("Device's Index ment to be transfered to the player.")]
    private int _deviceId;
    [Tooltip("Scriptable Object character ment to be transfered to the player.")]
    private SO_Character _character;
    #endregion

    #region Initialisation UI/player
    /// <summary>
    /// Called when object the panel is switching to enable 
    /// </summary>
    void OnEnable()
    {
        InitDropDowns();
    }

    /// <summary>
    /// Call all the initialisation functions
    /// </summary>
    public void InitDropDowns()
    {
        InitDropdownSelectDevicesOptions();
        InitDropdownSelectCharacterOptions();
        InitDropdownSelectTeamOptions();
    }

    /// <summary>
    /// Instanciate the new player, if he is playing, and setting all the infos needed
    /// </summary>
    public void TransferDataToPlayer()
    {
        if (_toggleIsPlaying.isOn)
        {
            GameObject player = Instantiate(_playerPrefab, new Vector3(1000f, 1000f, 1000f), new Quaternion(0f, 0f, 0f, 0f));
            player.GetComponent<PlayerInfos>().SetNewPlayerInfos(GetNewTeam(), GetNewDeviceId(), GetNewCharacter());
            player.tag = _text.text;
            DontDestroyOnLoad(player);
        }
    }
    #endregion

    #region Dropdown Select Device
    /// <summary>
    /// Initialise the UI Dropdown of selection Device.
    /// </summary>
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

    /// <summary>
    /// Get the Device Id of the selected device.
    /// </summary>
    /// <returns>Return -1 if null or the correct Index of the device.</returns>
    private int GetNewDeviceId()
    {
        if (_devicesNameToIndex.TryGetValue(_dropdownSelectDevice.captionText.text, out var indexCurretnDevices))
        {
            return indexCurretnDevices;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Active feedbacks when changes of device.
    /// </summary>
    public void FeedBackSelectionDevice()
    {
        if (_devicesNameToIndex.TryGetValue(_dropdownSelectDevice.captionText.text, out var indexCurretnDevices))
        {
            Gamepad gamepad = InputSystem.devices[indexCurretnDevices] as Gamepad;
            if (gamepad != null)
            {
                StartCoroutine(VibrateGamepad(gamepad));
            }
        }
    }

    /// <summary>
    /// Coroutine that makes vibrate the gamepad.
    /// </summary>
    /// <param name="gamepad">gamepad to make vibrate</param>
    /// <returns>None</returns>
    private System.Collections.IEnumerator VibrateGamepad(Gamepad gamepad)
    {
        gamepad.SetMotorSpeeds(0.5f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        gamepad.ResetHaptics();
    }

    #endregion

    #region Dropdown Select Character
    /// <summary>
    /// Initialise the UI Dropdown of selection characters.
    /// </summary>
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

    /// <summary>
    /// Get the character.
    /// </summary>
    /// <returns>Return null or the character selected.</returns>
    private SO_Character GetNewCharacter()
    {
        if (_characterNameToSO.TryGetValue(_dropdownSelectCharacter.captionText.text, out var character))
        {
            return character;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region Dropdown Select Team
    /// <summary>
    /// Initialise the UI Dropdown Selection teams.
    /// </summary>
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

    /// <summary>
    /// Get the team selected.
    /// </summary>
    /// <returns>TeamB by default if error or the team selected.</returns>
    private Team GetNewTeam()
    {
        foreach (Team team in Enum.GetValues(typeof(Team)))
        {
            if (_dropdownSelectTeam.captionText.text == team.ToString())
            {
                return team;
            }
        }
        return Team.TeamB;
    }

    #endregion
}
