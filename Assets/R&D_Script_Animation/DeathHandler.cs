using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] AnimationStateSummary state;
    [SerializeField] Animator animator;
    [SerializeField] string deathTrigger = "Die";
    IDeathSource source;
    int deathParameter;

    public bool IsDead => source != null && source.IsDead;

    void Awake()
    {
        source = GetComponentInParent<IDeathSource>();
        deathParameter = Animator.StringToHash(deathTrigger);

        if (source == null)
            Debug.LogWarning("DeathHandler sur" + name + " : aucun IDeathSource trouvé, la mort ne sera pas détectée", this);
    }

    void OnEnable()
    {
        if (source != null) source.Died += OnDied;
    }

    void OnDisable()
    {
        if (source != null) source.Died -= OnDied;
    }

    void OnDied()
    {
        Debug.Log("DeathHandler : OnDied recu, state = " + (state != null ? state.name : "VIDE"));
        if (state != null) state.Interrupt();
        if (animator != null) animator.SetTrigger(deathParameter);
    }
}
