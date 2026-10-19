using System;
using UnityEngine;

public class HitResolver : MonoBehaviour
{
    [SerializeField] CharacterStateMachine attacker;
    [SerializeField] Hitbox[] hitboxes;

    public event Action<CharacterStateMachine, CharacterStateMachine, AttackData> HitResolved;

    void Awake()
    {
        if (attacker == null) attacker = GetComponent<CharacterStateMachine>();
        if (hitboxes == null || hitboxes.Length == 0)
            hitboxes = GetComponentsInChildren<Hitbox>(true);
    }

    void OnEnable()
    {
        foreach (var hb in hitboxes)
            if (hb != null) hb.Hit -= OnHit;
    }

    void OnHit(Hitbox hitbox, CharacterStateMachine target, AttackData attack)
    {
        if (attack == null || target == null || target.IsDead) return;

        target.ApplyHit(attack.hitstun);
        if (attack.hitstop > 0) attacker.Freeze(attack.hitstop);
        if (attack.hitstop > 0) attacker.Freeze(attack.hitstop);
        HitResolved?.Invoke(attacker, target, attack);
    }
}
