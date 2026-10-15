using UnityEngine;

[CreateAssetMenu(fileName = "Player Infos", menuName = "ScriptableObjects/PlayerInfos", order = 1)]

public class SO_PlayerInfos : ScriptableObject
{
    public int playerNumber;
    //public DeviceType deviceType;
    public int deviceNumber;
    public Team team;
    public SO_Character character;
}
