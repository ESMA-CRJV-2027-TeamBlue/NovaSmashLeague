using System;
using UnityEngine;

/// <summary>
/// Durée en frames de chaque phase d'une action. Une frame = 1/60 s si le Fixed Timestep est réglé sur 0.016667.
/// </summary>
[Serializable]
public class ActionPhaseFrames
{
    #region Durées
    [Header("Durées (en frames)")]
    [Tooltip("Frames de préparation avant que le coup soit actif.")]
    public int startup = 6;

    [Tooltip("Frames pendant lesquelles la hitbox est active.")]
    public int active = 3;

    [Tooltip("Frames de récupération après le coup, pendant lesquelles on ne peut pas agir.")]
    public int recovery = 12;
    #endregion

    /// <summary>Durée totale de l'action, en frames.</summary>
    public int Total => startup + active + recovery;
}