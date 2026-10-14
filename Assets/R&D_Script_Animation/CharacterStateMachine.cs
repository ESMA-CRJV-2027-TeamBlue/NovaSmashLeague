using System;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
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

    public bool TryStartAction()
    {
        if (!CanAct || animationSummary == null) return false;
        return animationSummary.Begin();
    }

    public void ApplyHit(int stunFrames)
    {
        if (IsDead || Current == CharacterState.Freeze) return;

        animationSummary?.Interrupt();
        SetState(CharacterState.Hit);
        timedFramesLeft = stunFrames;
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
        animationSummary?.Interrupt();
        timedFramesLeft = -1;
        SetState(CharacterState.Dead);
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
        if (showDebug) Debug.Log($"{name} : {old} -> {newState}");
        StateChanged?.Invoke(old, newState);
    }

    void OnGUI()
    {
        if (!showDebug) return;
        GUI.Label(new Rect(debugPosition.x, debugPosition.y, 400f, 24f), $"{name} state : {Current}");
        
    }
}
