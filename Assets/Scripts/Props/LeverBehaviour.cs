using System;
using System.Collections;
using UnityEngine;

public class LeverBehaviour : MonoBehaviour, IActivable
{
    [SerializeField] private GameObject[] targetObjects;
    [SerializeField] private bool activated = false;
    [SerializeField, Min(0f)] private float timeToActivate = 1f;
    [SerializeField] private bool deactivable = true;
    [SerializeField] private bool timed = false;
    [SerializeField, Min(0f)] private float timer = 5f;
    [SerializeField, Tooltip("Time Before lever can be reactivated"), Min(0f)] private float restingTime = 2f;

    private IActivable[] m_Targets;
    private Animator[] m_Animators;
    private bool m_IsTriggered = false;

    private void Awake()
    {
        m_Animators = GetComponentsInChildren<Animator>();
        if (m_Animators.Length == 0)
        {
            Debug.LogError("No Animator found on any child object of " + name);
        }
        m_Targets = new IActivable[targetObjects.Length];
        for (uint i = 0; i < targetObjects.Length; i++)
        {
            m_Targets[i] = targetObjects[i].GetComponent<IActivable>();
        }
    }
    // Start is called before the first frame update
    void Start()
    { 
        if (activated)
        {
            activated = false;
            Activate();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Activate()
    {
        if (activated)
            return;
        StartCoroutine(DelayedActivation(OnActivate));
        foreach (Animator animator in m_Animators)
            animator.SetTrigger("Activate");
    }

    public void Deactivate()
    {
        if (!activated || !deactivable)
            return;
        StartCoroutine(DelayedActivation(OnDeactivate));
        foreach (Animator animator in m_Animators)
            animator.SetTrigger("Deactivate");
    }

    private void OnActivate()
    {
        activated = true;
        foreach (var target in targetObjects)
        {
            Animator animator = null;
            IActivable activable;
            if (target.TryGetComponent(out activable))
                activable.Activate();
            else if (target.TryGetComponent(out animator))
                animator.SetTrigger("Activate");
            else target.SetActive(true);
        };
            if (timed)
            StartCoroutine(TimedDeactivation());
    }

    private void OnDeactivate()
    {
        activated = false;
        foreach (var target in targetObjects)
        {
            Animator animator = null;
            IActivable activable;
            if (target.TryGetComponent(out activable))
                activable.Deactivate();
            else if (target.TryGetComponent(out animator))
                animator.SetTrigger("Deactivate");
            else target.SetActive(false);
        };
    }

    private IEnumerator DelayedActivation(Action callback)
    {
        yield return new WaitForSeconds(timeToActivate);
        callback?.Invoke();
    }
    IEnumerator TimedDeactivation()
    {
        yield return new WaitForSeconds(timer);
        while (!activated)
            yield return null;
        Deactivate();
    }
    private void OnTriggerEnter(Collider _other)
    {
        // Layer 6 is Spear 
        if (!m_IsTriggered && _other.transform.gameObject.layer == 6)
        {
            SpearBehaviour.State state = _other.GetComponent<SpearBehaviour>().state;
            if (state == SpearBehaviour.State.held && state == SpearBehaviour.State.stuck)
                return;
                StartCoroutine(RestingTimer());
            if (activated)
                Deactivate();
            else
                Activate();
            return;
        }
    }
    IEnumerator RestingTimer()
    {
        m_IsTriggered = true;
        yield return new WaitForSeconds(restingTime);
        m_IsTriggered = false;
    }
}
