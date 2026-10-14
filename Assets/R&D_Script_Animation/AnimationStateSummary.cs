using System;
using UnityEngine;

public class AnimationStateSummary : MonoBehaviour
{
    [SerializeField] ActionPhaseFrames defaultFrames = new ActionPhaseFrames();

    [Header("Debug")]
    [SerializeField] bool showDebug = true;
    [SerializeField] Vector2 debugPosition = new Vector2(10f, 10f);

    int startupEnd;
    int activeEnd;
    int totalFrames;
    int currentFrame;
    ActionPhase phase = ActionPhase.None;

    public ActionPhase Phase => phase;
    public int CurrentFrame => currentFrame;
    public int TotalFrames => totalFrames;
    public bool IsRunning => phase != ActionPhase.None;

    public event Action<ActionPhase> PhaseChanged;
    public event Action<ActionPhase> Interrupted;

    public bool Begin()
    {
        return Begin(defaultFrames);
    }

    public bool Begin(ActionPhaseFrames frames)
    {
        if (frames == null || IsRunning) return false;

        startupEnd = frames.startup;
        activeEnd = frames.startup + frames.active;
        totalFrames = frames.Total;
        currentFrame = 0;

        ChangePhase(ActionPhase.StartUp);
        return true;
    }

    void FixedUpdate()
    {
        if (!IsRunning) return;

        currentFrame++;

        if (currentFrame > totalFrames)
        {
            currentFrame = 0;
            ChangePhase(ActionPhase.None);
        }
        else if (currentFrame <= startupEnd)
        {
            ChangePhase(ActionPhase.StartUp);
        }
        else if (currentFrame <= activeEnd)
        {
            ChangePhase(ActionPhase.Action);
        }
        else
        {
            ChangePhase(ActionPhase.Recovery);
        }
    }
    public void Interrupt()
    {
        Debug.Log(name + " : Interrupt appele, IsRunning =" + IsRunning);
        if (!IsRunning) return;
        ActionPhase interruptedPhase = phase;
        currentFrame = 0;
        ChangePhase(ActionPhase.None);
        Interrupted?.Invoke(interruptedPhase);
        Debug.Log(name + " : après Interrupt, phase =" + phase);
    }

    void OnDisable()
    {
        currentFrame = 0;
        phase = ActionPhase.None;
    }

    void ChangePhase(ActionPhase newPhase)
    {
        if (phase == newPhase) return;
        phase = newPhase;
        PhaseChanged?.Invoke(newPhase);
    }

    [ContextMenu("Test : Begin")]
    void TestBegin()
    {
        if (Application.isPlaying) Begin();
    }

    [ContextMenu("Test : Interrupt")]
    void TestInterrupt()
    {
        if (Application.isPlaying) Interrupt();
    }

    void OnGUI()
    {
        if (!showDebug) return;
        GUI.Label(new Rect(debugPosition.x, debugPosition.y, 400f, 24f), name + ":" + phase + "frame" + currentFrame + "/" + totalFrames);
    }
}
