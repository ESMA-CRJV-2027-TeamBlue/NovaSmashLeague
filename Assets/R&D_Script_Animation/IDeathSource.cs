using System;

/// <summary>
/// Source de mort d'un personnage (vie à zéro, chute hors de l'arène...).
/// Permet aux autres scripts de réagir à la mort sans connaître sa cause.
/// </summary>
public interface IDeathSource
{
    /// <summary>Vrai si le personnage est mort.</summary>
    bool IsDead { get; }

    /// <summary>Déclenché une seule fois, au moment de la mort.</summary>
    event Action Died;
}