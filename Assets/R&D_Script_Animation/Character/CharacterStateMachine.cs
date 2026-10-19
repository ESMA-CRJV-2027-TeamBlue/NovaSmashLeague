using System;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{

    [Header("Debug / Test)")]
    [SerializeField] int testFrames = 180;

    [SerializeField] AttackData testAttack;
    public AttackData CurrentAttack { get; private set; }

    [ContextMenu("Test : Begin Action")]
    void TestBeginAction() { if (Application.isPlaying) TryStartAction();  }
    [ContextMenu("Test : Hit (30frames)")]
    void TestHit() { if (Application.isPlaying) ApplyHit(testFrames); }
    [ContextMenu("Test : Freeze")]
    void TestFreeze() { if (Application.isPlaying) Freeze(testFrames); }
    [ContextMenu("Test : FreezeMove")]
    void TestFreezeMove() { if (Application.isPlaying) FreezeMove(testFrames); }

    [ContextMenu("Test : Action puis Freeze")]
    void TestActionThenFreeze()
    {
        if (!Application.isPlaying) return;
        TryStartAction();
        Invoke(nameof(TestFreeze), 0.5f);
    }



    [SerializeField] AnimationStateSummary animationSummary;
    [SerializeField] bool showDebug = true;
    [SerializeField] Vector2 debugPosition = new Vector2(10f, 40f);

    IDeathSource deathSource;
    CharacterState stateBeforeFreeze = CharacterState.Free;
    int timedFramesLeft = -1;

    public CharacterState Current { get; private set; } = CharacterState.Free;
    
    public event Action<CharacterState, CharacterState> StateChanged;

    public bool IsDead => Current == CharacterState.Dead;
    public bool CanMove => Current == CharacterState.Free;
    public bool CanAct => Current == CharacterState.Free;
    public bool IsInAction => Current == CharacterState.StartUp
                           || Current == CharacterState.Action  
                           || Current == CharacterState.Recovery;
    
    void Awake()
    {
        deathSource = GetComponentInParent<IDeathSource>();
        if (animationSummary == null) animationSummary = GetComponent<AnimationStateSummary>();
    }

    void OnEnable()
    {
        if (animationSummary != null)
            animationSummary.PhaseChanged += OnPhaseChanged;
        if (deathSource != null)
            deathSource.Died += OnDied;
    }

    void OnDisable()
    {
        if (animationSummary != null)
            animationSummary.PhaseChanged -= OnPhaseChanged;
        if (deathSource != null)
            deathSource.Died -= OnDied;
    }

    void FixedUpdate()
    {
        if (timedFramesLeft < 0) return;

        timedFramesLeft--;
        if (timedFramesLeft < 0) return;

        timedFramesLeft = -1;
        EndTimedState();
    }

    public bool TryStartAction(AttackData attack)

    {
        if (attack == null || !CanAct || animationSummary == null) return false;
        if (!animationSummary.Begin(attack.frames)) return false;

        CurrentAttack = attack;
        return true;
    }

    public bool TryStartAction()
    {
        if (testAttack == null)
        {
            Debug.LogWarning($"{name} : aucun 'Test Attack' assigné dans CharacterStaeMachine");
            return false;
        }
        return TryStartAction(testAttack);
    }

    public void ApplyHit(int stunFrames)
    {
        if (IsDead || Current == CharacterState.Freeze) return;

        SetState(CharacterState.Hit);
        timedFramesLeft = stunFrames;
        animationSummary?.Interrupt();
    }

    public void Freeze(int frames = -1)
    {
        if (IsDead || Current == CharacterState.Freeze) return;

        stateBeforeFreeze = Current;
        SetState(CharacterState.Freeze);
        timedFramesLeft = frames > 0 ? frames : -1;
    }

    public void Unfreeze()
    {
        if (Current == CharacterState.Freeze) EndTimedState();
    }

    public void FreezeMove(int frames = -1)
    {
        if (Current != CharacterState.Free) return;

        SetState(CharacterState.FreezeMove);
        timedFramesLeft = frames > 0 ? frames : -1;
    }

    public void ReleaseMove()
    {
        if (Current == CharacterState.FreezeMove) SetState(CharacterState.Free);

    }

    void OnPhaseChanged(ActionPhase phase)
    {
        if (Current == CharacterState.Dead
            || Current == CharacterState.Freeze
            || Current == CharacterState.Hit) return;
        switch (phase)
        {
            case ActionPhase.StartUp: SetState(CharacterState.StartUp); break;
            case ActionPhase.Action: SetState(CharacterState.Action); break;
            case ActionPhase.Recovery: SetState(CharacterState.Recovery); break;
            case ActionPhase.None:
                if (IsInAction) SetState(CharacterState.Free);
                break;
        }
    }

    void OnDied()
    {
        timedFramesLeft = -1;
        SetState(CharacterState.Dead);
        animationSummary?.Interrupt();
    }

    void EndTimedState()
    {
        if (Current == CharacterState.Freeze)
            SetState(stateBeforeFreeze);
        else if (Current == CharacterState.Hit || Current == CharacterState.FreezeMove)
            SetState(CharacterState.Free);
    }
    
    void SetState(CharacterState newState)
    {
        if (Current == CharacterState.Dead || Current == newState) return;

        var old = Current;
        Current = newState;
        if (newState == CharacterState.Free
            || newState == CharacterState.Hit
            || newState == CharacterState.FreezeMove
            || newState == CharacterState.Dead)
            CurrentAttack = null;

        if (showDebug) Debug.Log($"{name} : {old} -> {newState} [t = {Time.fixedTime:F3}s");
        StateChanged?.Invoke(old, newState);
    }

    void OnGUI()
    {
        if (!showDebug) return;
        string timer = timedFramesLeft >= 0 ? $" (reste {timedFramesLeft}f)" : "";
        string atk = CurrentAttack != null ? $" | {CurrentAttack.attackName}" : "";
        DebugHud.Label($"[ETAT] {name} : {Current}{timer}{atk}");
    }
}
