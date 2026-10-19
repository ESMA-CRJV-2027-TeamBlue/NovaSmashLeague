using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "NovaSmashLeague/Attack Data")]
public class AttackData : ScriptableObject
{
    public string attackName = "Attack";

    [Header("Frames (60 par seconde")]
    public ActionPhaseFrames frames = new ActionPhaseFrames { startup = 6, active = 3, recovery = 12 };

    [Header("Effets sur la cible")]
    public int damage = 10;
    public int hitstun = 15;
    public int hitstop = 6;
}
