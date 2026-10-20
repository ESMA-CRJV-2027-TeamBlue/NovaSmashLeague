using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Applique les effets d'un coup quand une hitbox touche une cible :
/// hitstun sur la cible, hitstop sur la cible et l'attaquant.
/// À placer sur le personnage attaquant.
/// </summary>
public class HitResolver : MonoBehaviour
{
    #region Références
    [Header("Références")]
    [Tooltip("Machine à état de l'attaquant. Cherchée sur l'objet si elle est vide.")]
    [FormerlySerializedAs("attacker")]
    [SerializeField] private CharacterStateMachine _attacker;

    [Tooltip("Hitboxes de l'attaquant. Cherchées dans les enfants si la liste est vide.")]
    [FormerlySerializedAs("hitboxes")]
    [SerializeField] private Hitbox[] _hitboxes;
    #endregion

    /// <summary>Déclenché après chaque coup résolu : (attaquant, cible, attaque). Sert à brancher dégâts, effets et sons.</summary>
    public event Action<CharacterStateMachine, CharacterStateMachine, AttackData> HitResolved;

    #region Messages Unity
    /// <summary>Récupère les références manquantes.</summary>
    private void Awake()
    {
        if (_attacker == null) _attacker = GetComponent<CharacterStateMachine>();
        if (_hitboxes == null || _hitboxes.Length == 0)
            _hitboxes = GetComponentsInChildren<Hitbox>(true);
    }

    /// <summary>S'abonne à toutes les hitboxes.</summary>
    private void OnEnable()
    {
        foreach (Hitbox hitbox in _hitboxes)
            if (hitbox != null) hitbox.Hit += OnHit;
    }

    /// <summary>Se désabonne de toutes les hitboxes.</summary>
    private void OnDisable()
    {
        foreach (Hitbox hitbox in _hitboxes)
            if (hitbox != null) hitbox.Hit -= OnHit;
    }
    #endregion

    /// <summary>
    /// Applique le hitstun puis le hitstop à la cible, et le hitstop à l'attaquant.
    /// L'ordre compte : le hitstun doit être posé avant le gel, qui le mémorise et le reprend ensuite.
    /// </summary>
    /// <param name="hitbox">Hitbox qui a touché.</param>
    /// <param name="target">Personnage touché.</param>
    /// <param name="attack">Attaque utilisée.</param>
    private void OnHit(Hitbox hitbox, CharacterStateMachine target, AttackData attack)
    {
        if (attack == null || target == null || target.IsDead) return;

        // La cible encaisse : hitstun d'abord, puis gel (hitstop 0 = pas de gel)
        target.ApplyHit(attack.hitstun);
        if (attack.hitstop > 0) target.Freeze(attack.hitstop);

        // L'attaquant se fige aussi, même durée
        if (attack.hitstop > 0) _attacker.Freeze(attack.hitstop);

        HitResolved?.Invoke(_attacker, target, attack);
    }
}