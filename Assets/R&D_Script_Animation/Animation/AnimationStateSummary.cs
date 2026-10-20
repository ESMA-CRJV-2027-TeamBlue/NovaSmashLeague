using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Compteur de frames d'une action. Fait avancer les phases StartUp, Action et Recovery
/// et prévient les autres scripts à chaque changement de phase.
/// </summary>
public class AnimationStateSummary : MonoBehaviour
{
    #region Debug
    [Header("Debug")]
    [Tooltip("Affiche la phase et le compteur de frames à l'écran.")]
    [FormerlySerializedAs("showDebug")]
    [SerializeField] private bool _showDebug = true;
    #endregion

    private int _startupEnd;
    private int _activeEnd;
    private int _totalFrames;
    private int _currentFrame;
    private ActionPhase _phase = ActionPhase.None;

    #region Propriétés
    /// <summary>Phase en cours (None si aucune action).</summary>
    public ActionPhase Phase => _phase;

    /// <summary>Frame en cours dans l'action.</summary>
    public int CurrentFrame => _currentFrame;

    /// <summary>Durée totale de l'action en cours, en frames.</summary>
    public int TotalFrames => _totalFrames;

    /// <summary>Vrai si une action est en cours.</summary>
    public bool IsRunning => _phase != ActionPhase.None;

    /// <summary>Mis à vrai par la machine à état pendant un Freeze : fige le compteur.</summary>
    public bool Paused { get; set; }
    #endregion

    #region Évènements
    /// <summary>Déclenché à chaque changement de phase, avec la nouvelle phase.</summary>
    public event Action<ActionPhase> PhaseChanged;

    /// <summary>Déclenché quand l'action est coupée, avec la phase qui a été interrompue.</summary>
    public event Action<ActionPhase> Interrupted;
    #endregion

    #region API publique
    /// <summary>
    /// Démarre une action avec les durées données.
    /// </summary>
    /// <param name="frames">Durée de chaque phase, en frames.</param>
    /// <returns>Vrai si l'action a démarré, faux si les durées sont nulles ou si une action est déjà en cours.</returns>
    public bool Begin(ActionPhaseFrames frames)
    {
        if (frames == null || IsRunning) return false;

        _startupEnd = frames.startup;
        _activeEnd = frames.startup + frames.active;
        _totalFrames = frames.Total;
        _currentFrame = 0;

        ChangePhase(ActionPhase.StartUp);
        return true;
    }

    /// <summary>
    /// Coupe l'action en cours (par exemple quand le personnage est touché). Sans effet si aucune action n'est en cours.
    /// </summary>
    public void Interrupt()
    {
        if (!IsRunning) return;

        ActionPhase interruptedPhase = _phase;
        _currentFrame = 0;
        ChangePhase(ActionPhase.None);
        Interrupted?.Invoke(interruptedPhase);
    }
    #endregion

    #region Messages Unity
    /// <summary>Fait avancer le compteur d'une frame à chaque pas de physique.</summary>
    private void FixedUpdate()
    {
        if (!IsRunning || Paused) return;

        _currentFrame++;

        if (_currentFrame > _totalFrames)
        {
            _currentFrame = 0;
            ChangePhase(ActionPhase.None);
        }
        else if (_currentFrame <= _startupEnd)
        {
            ChangePhase(ActionPhase.StartUp);
        }
        else if (_currentFrame <= _activeEnd)
        {
            ChangePhase(ActionPhase.Action);
        }
        else
        {
            ChangePhase(ActionPhase.Recovery);
        }
    }

    /// <summary>Remet le compteur à zéro quand le composant est désactivé.</summary>
    private void OnDisable()
    {
        _currentFrame = 0;
        _phase = ActionPhase.None;
        Paused = false;
    }

    /// <summary>Affiche l'état du compteur à l'écran.</summary>
    private void OnGUI()
    {
        if (!_showDebug) return;
        DebugHud.Label($"[FRAMES] {name} : {_phase}  {_currentFrame}/{_totalFrames}");
    }
    #endregion

    /// <summary>
    /// Change de phase et prévient les abonnés. Ne fait rien si la phase est identique.
    /// </summary>
    /// <param name="newPhase">Nouvelle phase.</param>
    private void ChangePhase(ActionPhase newPhase)
    {
        if (_phase == newPhase) return;

        _phase = newPhase;
        PhaseChanged?.Invoke(newPhase);
    }
}