/// <summary>
/// Phases d'une action (incantation, attaque et temps de récupération...), dans l'ordre où elles se déroulent.
/// </summary>
public enum ActionPhase
{
    /// <summary>Aucune action en cours.</summary>
    None,

    /// <summary>Préparation du coup : le personnage ne touche pas encore.</summary>
    StartUp,

    /// <summary>Le coup est actif : la hitbox peut toucher.</summary>
    Action,

    /// <summary>Fin du coup : le personnage est vulnérable et ne peut pas encore agir.</summary>
    Recovery
}