using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Zone de dégâts d'un personnage. Son collider n'est actif que pendant l'état Action,
/// et chaque cible n'est touchée qu'une seule fois par attaque.
/// </summary>
public class Hitbox : MonoBehaviour
{
    #region Références
    [Header("Références")]
    [Tooltip("Machine à état du personnage qui attaque. Cherchée dans les parents si elle est vide.")]
    [FormerlySerializedAs("owner")]
    [SerializeField] private CharacterStateMachine _owner;

    [Tooltip("Collider de la hitbox. Cherché sur l'objet s'il est vide.")]
    [FormerlySerializedAs("hitCollider")]
    [SerializeField] private Collider _hitCollider;
    #endregion

    #region Debug
    [Header("Debug")]
    [Tooltip("Écrit un message dans la Console à chaque cible touchée.")]
    [FormerlySerializedAs("showDebug")]
    [SerializeField] private bool _showDebug = true;
    #endregion

    private readonly HashSet<CharacterStateMachine> _alreadyHit = new HashSet<CharacterStateMachine>();

    /// <summary>Déclenché quand une cible est touchée : (cette hitbox, la cible, l'attaque utilisée).</summary>
    public event Action<Hitbox, CharacterStateMachine, AttackData> Hit;

    #region Messages Unity
    /// <summary>Récupère les références et désactive le collider au départ.</summary>
    private void Awake()
    {
        if (_owner == null) _owner = GetComponentInParent<CharacterStateMachine>();
        if (_hitCollider == null) _hitCollider = GetComponent<Collider>();

        _hitCollider.isTrigger = true;
        _hitCollider.enabled = false;
    }

    /// <summary>S'abonne aux changements d'état du personnage.</summary>
    private void OnEnable()
    {
        if (_owner != null) _owner.StateChanged += OnStateChanged;
    }

    /// <summary>Se désabonne et coupe le collider.</summary>
    private void OnDisable()
    {
        if (_owner != null) _owner.StateChanged -= OnStateChanged;
        _hitCollider.enabled = false;
    }

    /// <summary>Détecte une cible entrant dans la hitbox et prévient les abonnés.</summary>
    /// <param name="other">Collider qui vient d'entrer dans la zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        CharacterStateMachine target = other.GetComponentInParent<CharacterStateMachine>();
        if (target == null || target == _owner || _alreadyHit.Contains(target)) return;

        _alreadyHit.Add(target);

        if (_showDebug) Debug.Log($"{_owner.name} touche {target.name} avec {_owner.CurrentAttack?.attackName}");
        Hit?.Invoke(this, target, _owner.CurrentAttack);
    }

    /// <summary>Dessine la hitbox dans la Scene view : rouge si active, blanc transparent sinon.</summary>
    private void OnDrawGizmos()
    {
        if (_hitCollider == null) return;

        Gizmos.color = _hitCollider.enabled ? Color.red : new Color(1f, 1f, 1f, 0.2f);
        Gizmos.DrawWireCube(_hitCollider.bounds.center, _hitCollider.bounds.size);
    }
    #endregion

    /// <summary>
    /// Active le collider pendant l'état Action uniquement. Ne fait rien pendant un gel.
    /// </summary>
    /// <param name="oldState">Ancien état du personnage.</param>
    /// <param name="newState">Nouvel état du personnage.</param>
    private void OnStateChanged(CharacterState oldState, CharacterState newState)
    {
        // Pendant un Freeze (hitstop), la hitbox reste comme elle était
        if (newState == CharacterState.Freeze) return;

        bool active = newState == CharacterState.Action;

        // Nouvelle attaque : on oublie les cibles déjà touchées
        // (mais pas quand on revient d'un Freeze, sinon on toucherait deux fois)
        if (active && oldState != CharacterState.Freeze) _alreadyHit.Clear();

        _hitCollider.enabled = active;
    }
}