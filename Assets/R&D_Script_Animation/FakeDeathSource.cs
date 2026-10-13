using System;
using UnityEngine;

public class FakeDeathSource : MonoBehaviour, IDeathSource
{
    public bool IsDead { get; private set; }
    public event Action Died;

    [ContextMenu("Test : Mourir")]

    void Die()
    {
        if (!Application.isPlaying) { Debug.Log("Die : pas en Play Mode"); return; }
        if (IsDead) { Debug.Log("Die : deja mort"); return; }

        IsDead = true;
        Debug.Log("Die : envoi de l'evenement Died");
        Died?.Invoke();
    }
}
