using UnityEngine;

public class PlayerInfos : MonoBehaviour
{
    private Team _team;
    private int _deviceId;
    private SO_Character _character;
    [Tooltip("GameObject visuel du personnage.")]
    [SerializeField] private GameObject _characterVisual;

    /// <summary>
    /// Actualize player's informations set in parameters.
    /// </summary>
    /// <param name="newTeam">New player's team apartenece.</param>
    /// <param name="newDeviceId">New player's controller index.</param>
    /// <param name="_newCharacter">New player's character.</param>
    public void SetNewPlayerInfos(Team newTeam, int newDeviceId, SO_Character _newCharacter)
    {
        _team = newTeam;
        _deviceId = newDeviceId;
        _character = _newCharacter;
        LoadCharacterInfos();
    }

    /// <summary>
    /// Set visual character's informations from the scriptable object character.
    /// </summary>
    private void LoadCharacterInfos()
    {
        _characterVisual = _character.visualCharacter;
    }
}
