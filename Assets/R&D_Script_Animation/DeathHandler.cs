using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Réagit à la mort du personnage en lançant l'animation de mort.
/// Cherche un <see cref="IDeathSource"/> sur le personnage ou ses parents.
/// </summary>
public class DeathHandler : MonoBehaviour
{
    #region Paramètres
    [Header("Animation de mort")]
    [Tooltip("Animator du personnage. Peut rester vide tant qu'il n'y a pas d'Animator.")]
    [FormerlySerializedAs("animator")]
    [SerializeField] private Animator _animator;

    [Tooltip("Nom du trigger de l'Animator déclenché à la mort.")]
    [FormerlySerializedAs("deathTrigger")]
    [SerializeField] private string _deathTrigger = "Die";
    #endregion

    private IDeathSource _source;
    private int _deathParameter;

    /// <summary>Vrai si la source de mort indique que le personnage est mort.</summary>
    public bool IsDead => _source != null && _source.IsDead;

    /// <summary>Récupère la source de mort et prépare le paramètre de l'Animator.</summary>
    private void Awake()
    {
        _source = GetComponentInParent<IDeathSource>();
        _deathParameter = Animator.StringToHash(_deathTrigger);

        if (_source == null)
            Debug.LogWarning("DeathHandler sur " + name + " : aucun IDeathSource trouvé, la mort ne sera jamais déclenchée");
    }

    /// <summary>S'abonne à l'évènement de mort.</summary>
    private void OnEnable()
    {
        if (_source != null) _source.Died += OnDied;
    }

    /// <summary>Se désabonne de l'évènement de mort.</summary>
    private void OnDisable()
    {
        if (_source != null) _source.Died -= OnDied;
    }

    /// <summary>Déclenche l'animation de mort si un Animator est assigné.</summary>
    private void OnDied()
    {
        if (_animator != null) _animator.SetTrigger(_deathParameter);
    }
}