using System;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] CharacterStateMachine owner;
    [SerializeField] Collider hitCollider;
    [SerializeField] bool showDebug = true;

    readonly HashSet<CharacterStateMachine> alreadyHit = new HashSet<CharacterStateMachine>();

    public event Action<Hitbox, CharacterStateMachine, AttackData> Hit;

    void Awake()
    {
        if (owner == null) owner = GetComponentInParent<CharacterStateMachine>();
        if (hitCollider == null) hitCollider = GetComponent<Collider>();

        hitCollider.isTrigger = true;
        hitCollider.enabled = false;
    }

    void OnEnable()
    {
        if (owner != null) owner.StateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        if (owner != null) owner.StateChanged -= OnStateChanged;
        hitCollider.enabled = false;
    }

    void OnStateChanged(CharacterState oldState, CharacterState newState)
    {
        if (newState == CharacterState.Freeze) return;

        bool active = newState == CharacterState.Action;

        if (active && oldState != CharacterState.Freeze) alreadyHit.Clear();

        hitCollider.enabled = active;
    }

    void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponentInParent<CharacterStateMachine>();
        if (target == null || target == owner || alreadyHit.Contains(target)) return;

        alreadyHit.Add(target);

        if (showDebug) Debug.Log($"{owner.name} touche{target.name} avec {owner.CurrentAttack?.attackName}");
        Hit?.Invoke(this, target, owner.CurrentAttack);
    }

    void OnDrawGizmos()
    {
        if (hitCollider == null) return;
        Gizmos.color = hitCollider.enabled ? Color.red : new Color(1f, 1f, 1f, 0.2f);
        Gizmos.DrawWireCube(hitCollider.bounds.center, hitCollider.bounds.size);
    }
}
