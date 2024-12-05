using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Health : MonoBehaviour
{
    [SerializeField, Min(1f)] private int maxHealthPoints = 10;
    public int currentHealthPoints = 10;
    [Header("Animation triggers Death and Damage already linked")]
    public UnityEvent OnTakeDamage = null;
    //[HideInInspector]
    public UnityEvent OnDeath = null;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyAfterSeconds = 2f;
    [SerializeField] private GameObject objectToDestroy;


    public bool invulnerable { get; private set; } = false;
    public bool dead { get; private set; } = false;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    // Start is called before the first frame update
    private void Awake()
    {
        TryGetComponent(out m_Rigidbody);
        m_Animator = GetComponent<Animator>();
    }
    public void Start()
    {
        dead = false;
        m_Animator.SetBool("Death", false);
        currentHealthPoints = maxHealthPoints;
    }

    private void OnValidate()
    {
        currentHealthPoints = maxHealthPoints;
        m_Animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void LoseHP(Damage _damage)
    {
        if (dead || invulnerable)
            return;
        currentHealthPoints -= _damage.amount;
        m_Animator.SetTrigger("Damage");
        Debug.Log(currentHealthPoints);
        OnTakeDamage.Invoke();
        CheckDeath();
        SetInvulnerability(_damage.graceTime);
    }

    public void InstaKill()
    {
        Die();
    }

    private void CheckDeath()
    {
        if (currentHealthPoints > 0f)
            return;
        Die();
    }
    private void Die()
    {
        if (dead)
            return;
        currentHealthPoints = 0;
        dead = true;
        m_Animator.ResetTrigger("Damage");
        OnDeath.Invoke(); 
        m_Animator.SetBool("Death", true);
        if (destroyOnDeath)
        {
            Destroy(objectToDestroy? objectToDestroy:transform.gameObject, destroyAfterSeconds);
        }
        Debug.Log(name+"Dead");
    }

    public void SetInvulnerability(float _time)
    {
        if (dead || invulnerable) return;
        StartCoroutine(nameof(InvulnerabilityCoroutine), _time);
    }

    private IEnumerator InvulnerabilityCoroutine(float _time)
    {
        invulnerable = true;
        yield return new WaitForSeconds(_time);
        invulnerable = false;
    }
}
