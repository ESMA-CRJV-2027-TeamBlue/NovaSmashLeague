using UnityEngine;

public class PlayerInfos : MonoBehaviour
{
    private Team _team;
    private int _deviceId;
    private SO_Character _character;

    [SerializeField] private MeshFilter _meshFilter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNewPlayerInfos(Team newTeam, int newDeviceId, SO_Character _newCharacter)
    {
        _team = newTeam;
        _deviceId = newDeviceId;
        _character = _newCharacter;
        LoadCharacterInfos();
    }

    private void LoadCharacterInfos()
    {
        _meshFilter.mesh = _character._meshCharacter;
    }
}
