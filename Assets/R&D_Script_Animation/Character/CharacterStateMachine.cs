using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Machine à état du personnage. Écoute le compteur de frames et la source de mort,
/// et expose un état unique que les autres scripts (mouvement, animation, UI) peuvent lire.
/// Priorité des états : Dead > Freeze > Hit > le reste.
/// </summary>
public class CharacterStateMachine : MonoBehaviour
{
    #region Références
    [Header("Références")]
    [Tooltip("Compteur de frames qui fournit les phases StartUp, Action et Recovery. Cherché sur l'objet s'il est vide.")]
    [FormerlySerializedAs("animationSummary")]
    [SerializeField] private AnimationStateSummary _animationSummary;
    #endregion

    #region Debug et tests
    [Header("Debug et tests")]
    [Tooltip("Affiche l'état à l'écran et dans la Console.")]
    [FormerlySerializedAs("showDebug")]
    [SerializeField] private bool _showDebug = true;

    [Tooltip("Attaque lancée par le menu de test « Begin Action ».")]
    [FormerlySerializedAs("testAttack")]
    [SerializeField] private AttackData _testAttack;

    [Tooltip("Durée en frames des tests Hit, Freeze et FreezeMove (60 frames = 1 seconde).")]
    [FormerlySerializedAs("testFrames")]
    [SerializeField] private int _testFrames = 180;
    #endregion

    private IDeathSource _deathSource;
    private CharacterState _stateBeforeFreeze = CharacterState.Free;
    private int _framesBeforeFreeze = -1;
    private int _timedFramesLeft = -1; // -1 = pas de timer (durée indéfinie)

    #region Propriétés
    /// <summary>État actuel du personnage.</summary>
    public CharacterState Current { get; private set; } = CharacterState.Free;

    /// <summary>Attaque en cours (null si le personnage n'attaque pas).</summary>
    public AttackData CurrentAttack { get; private set; }

    /// <summary>Vrai si le personnage est mort.</summary>
    public bool IsDead => Current == CharacterState.Dead;

    /// <summary>Vrai si le personnage peut se déplacer.</summary>
    public bool CanMove => Current == CharacterState.Free;

    /// <summary>Vrai si le personnage peut lancer une action.</summary>
    public bool CanAct => Current == CharacterState.Free;

    /// <summary>Vrai si le personnage est en StartUp, Action ou Recovery.</summary>
    public bool IsInAction => Current == CharacterState.StartUp
                           || Current == CharacterState.Action
                           || Current == CharacterState.Recovery;
    #endregion

    #region Évènements
    /// <summary>Déclenché à chaque changement d'état, avec (ancien état, nouvel état).</summary>
    public event Action<CharacterState, CharacterState> StateChanged;
    #endregion

    #region Messages Unity
    /// <summary>Récupère la source de mort et le compteur de frames.</summary>
    private void Awake()
    {
        _deathSource = GetComponentInParent<IDeathSource>();
        if (_animationSummary == null) _animationSummary = GetComponent<AnimationStateSummary>();
    }

    /// <summary>S'abonne aux phases d'animation et à la mort.</summary>
    private void OnEnable()
    {
        if (_animationSummary != null) _animationSummary.PhaseChanged += OnPhaseChanged;
        if (_deathSource != null) _deathSource.Died += OnDied;
    }

    /// <summary>Se désabonne des phases d'animation et de la mort.</summary>
    private void OnDisable()
    {
        if (_animationSummary != null) _animationSummary.PhaseChanged -= OnPhaseChanged;
        if (_deathSource != null) _deathSource.Died -= OnDied;
    }

    /// <summary>Fait avancer le timer des états temporaires (Hit, Freeze, FreezeMove).</summary>
    private void FixedUpdate()
    {
        if (_timedFramesLeft < 0) return;

        _timedFramesLeft--;
        if (_timedFramesLeft > 0) return;

        _timedFramesLeft = -1;
        EndTimedState();
    }

    /// <summary>Affiche l'état à l'écran.</summary>
    private void OnGUI()
    {
        if (!_showDebug) return;

        string timer = _timedFramesLeft >= 0 ? $" (reste {_timedFramesLeft}f)" : "";
        string attack = CurrentAttack != null ? $" | {CurrentAttack.attackName}" : "";
        DebugHud.Label($"[ÉTAT] {name} : {Current}{timer}{attack}");
    }
    #endregion

    #region API publique
    /// <summary>
    /// Lance une attaque si le personnage est libre.
    /// </summary>
    /// <param name="attack">Données de l'attaque à lancer.</param>
    /// <returns>Vrai si l'attaque a démarré, faux sinon (personnage occupé, mort ou attaque vide).</returns>
    public bool TryStartAction(AttackData attack)
    {
        if (attack == null || !CanAct || _animationSummary == null) return false;
        if (!_animationSummary.Begin(attack.frames)) return false;

        CurrentAttack = attack;
        return true;
    }

    /// <summary>
    /// Raccourci pour les tests : lance l'attaque assignée dans « Test Attack ».
    /// </summary>
    /// <returns>Vrai si l'attaque a démarré.</returns>
    public bool TryStartAction()
    {
        if (_testAttack == null)
        {
            Debug.LogWarning($"{name} : aucun 'Test Attack' assigné dans CharacterStateMachine");
            return false;
        }
        return TryStartAction(_testAttack);
    }

    /// <summary>
    /// Inflige un hitstun : coupe l'action en cours et met le personnage en état Hit.
    /// Ignoré si le personnage est mort ou gelé.
    /// </summary>
    /// <param name="stunFrames">Durée du hitstun, en frames.</param>
    public void ApplyHit(int stunFrames)
    {
        if (IsDead || Current == CharacterState.Freeze) return;

        SetState(CharacterState.Hit);      // d'abord l'état prioritaire...
        _timedFramesLeft = stunFrames;
        _animationSummary?.Interrupt();    // ...puis on coupe l'animation (OnPhaseChanged l'ignore en Hit)
    }

    /// <summary>
    /// Gèle le personnage (hitstop, cinématique). L'état d'avant reprend à la fin du gel.
    /// </summary>
    /// <param name="frames">Durée du gel en frames. 0 ou moins : gel jusqu'à l'appel de <see cref="Unfreeze"/>.</param>
    public void Freeze(int frames = -1)
    {
        if (IsDead || Current == CharacterState.Freeze) return;

        _stateBeforeFreeze = Current;
        _framesBeforeFreeze = _timedFramesLeft;
        SetState(CharacterState.Freeze);
        _timedFramesLeft = frames > 0 ? frames : -1;
    }

    /// <summary>Met fin à un gel en cours et reprend l'état précédent.</summary>
    public void Unfreeze()
    {
        if (Current == CharacterState.Freeze) EndTimedState();
    }

    /// <summary>
    /// Bloque le déplacement. Ne démarre que si le personnage est libre.
    /// </summary>
    /// <param name="frames">Durée en frames. 0 ou moins : jusqu'à l'appel de <see cref="ReleaseMove"/>.</param>
    public void FreezeMove(int frames = -1)
    {
        if (Current != CharacterState.Free) return;

        SetState(CharacterState.FreezeMove);
        _timedFramesLeft = frames > 0 ? frames : -1;
    }

    /// <summary>Libère le déplacement après un <see cref="FreezeMove"/>.</summary>
    public void ReleaseMove()
    {
        if (Current == CharacterState.FreezeMove) SetState(CharacterState.Free);
    }
    #endregion

    #region Réactions aux autres scripts
    /// <summary>
    /// Traduit une phase d'animation en état du personnage.
    /// Ignorée si le personnage est mort, gelé ou touché.
    /// </summary>
    /// <param name="phase">Nouvelle phase du compteur de frames.</param>
    private void OnPhaseChanged(ActionPhase phase)
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

    /// <summary>Passe le personnage en état Dead et coupe l'action en cours.</summary>
    private void OnDied()
    {
        _timedFramesLeft = -1;
        SetState(CharacterState.Dead);
        _animationSummary?.Interrupt();
    }
    #endregion

    #region Interne
    /// <summary>Termine l'état temporaire en cours (Hit, FreezeMove ou Freeze).</summary>
    private void EndTimedState()
    {
        if (Current == CharacterState.Freeze)
        {
            SetState(_stateBeforeFreeze);             // on reprend où on en était
            _timedFramesLeft = _framesBeforeFreeze;   // et le timer là où il en était
        }
        else if (Current == CharacterState.Hit || Current == CharacterState.FreezeMove)
        {
            SetState(CharacterState.Free);
        }
    }

    /// <summary>
    /// Change d'état et prévient les abonnés. Dead est final, et un état identique est ignoré.
    /// </summary>
    /// <param name="newState">Nouvel état.</param>
    private void SetState(CharacterState newState)
    {
        if (Current == CharacterState.Dead || Current == newState) return;

        CharacterState oldState = Current;
        Current = newState;

        // On garde l'attaque pendant un Freeze (elle reprendra après), on l'oublie sinon
        if (newState == CharacterState.Free
            || newState == CharacterState.Hit
            || newState == CharacterState.FreezeMove
            || newState == CharacterState.Dead)
            CurrentAttack = null;

        if (_animationSummary != null)
            _animationSummary.Paused = newState == CharacterState.Freeze;

        if (_showDebug) Debug.Log($"{name} : {oldState} -> {newState}  [t = {Time.fixedTime:F3}s]");
        StateChanged?.Invoke(oldState, newState);
    }
    #endregion

    #region Tests (clic droit sur le composant en Play Mode)
    /// <summary>Test : lance l'attaque de test.</summary>
    [ContextMenu("Test : Begin Action")]
    private void TestBeginAction() { if (Application.isPlaying) TryStartAction(); }

    /// <summary>Test : inflige un hitstun.</summary>
    [ContextMenu("Test : Hit")]
    private void TestHit() { if (Application.isPlaying) ApplyHit(_testFrames); }

    /// <summary>Test : gèle le personnage.</summary>
    [ContextMenu("Test : Freeze")]
    private void TestFreeze() { if (Application.isPlaying) Freeze(_testFrames); }

    /// <summary>Test : bloque le déplacement.</summary>
    [ContextMenu("Test : FreezeMove")]
    private void TestFreezeMove() { if (Application.isPlaying) FreezeMove(_testFrames); }

    /// <summary>Test : lance une attaque puis gèle le personnage 0,5 s plus tard.</summary>
    [ContextMenu("Test : Action puis Freeze")]
    private void TestActionThenFreeze()
    {
        if (!Application.isPlaying) return;
        TryStartAction();
        Invoke(nameof(TestFreeze), 0.5f);
    }
    #endregion
}