using UnityEngine;

/// <summary>
/// Données d'une attaque : durée des phases et effets sur la cible.
/// Un asset par coup (Create > NovaSmashLeague > Attack Data).
/// </summary>
[CreateAssetMenu(fileName = "NewAttack", menuName = "NovaSmashLeague/Attack Data")]
public class AttackData : ScriptableObject
{
    #region Identité
    [Header("Identité")]
    [Tooltip("Nom de l'attaque, affiché dans les logs et le debug.")]
    public string attackName = "Attack";
    #endregion

    #region Durées
    [Header("Durées")]
    [Tooltip("Frames de startup, d'active et de recovery (60 frames = 1 seconde).")]
    public ActionPhaseFrames frames = new ActionPhaseFrames();
    #endregion

    #region Effets sur la cible
    [Header("Effets sur la cible")]
    [Tooltip("Dégâts infligés à la cible.")]
    public int damage = 10;

    [Tooltip("Frames pendant lesquelles la cible reste en état Hit (ne peut pas agir).")]
    public int hitstun = 15;

    [Tooltip("Frames de gel (hitstop) appliquées à l'attaquant ET à la cible. 0 = pas de gel.")]
    public int hitstop = 6;
    #endregion
}