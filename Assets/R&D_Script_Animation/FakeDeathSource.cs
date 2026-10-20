using System;
using UnityEngine;

/// <summary>
/// Fausse source de mort pour les tests : clic droit sur le composant > "Test : Mourir".
/// </summary>
public class FakeDeathSource : MonoBehaviour, IDeathSource
{
    /// <inheritdoc/>
    public bool IsDead { get; private set; }

    /// <inheritdoc/>
    public event Action Died;

    /// <summary>
    /// Fait mourir le personnage (une seule fois, uniquement en Play Mode).
    /// </summary>
    [ContextMenu("Test : Mourir")]
    private void Die()
    {
        if (!Application.isPlaying) { Debug.Log("Die : pas en Play Mode"); return; }
        if (IsDead) { Debug.Log("Die : déjà mort"); return; }

        IsDead = true;
        Debug.Log("Die : envoi de l'évènement Died");
        Died?.Invoke();
    }
}