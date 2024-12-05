using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour, IDetectable
{
    [SerializeField] private Damage damage;
    [SerializeField] private Animator animator;
    [SerializeField] private Health health;
    [SerializeField] private float timeAttackPrepare = 1.5f;
    [SerializeField] private float timeDangerZone = 1.5f;

    [SerializeField] private float timeShieldProtect = 0.5f;

    public bool inRange = false;
    public bool m_Attacking = false;

    private void FixedUpdate()
    {
        if (health.dead == true)
        {StopAllCoroutines();
            animator.SetTrigger("Death");
        }
    }
    public void OnDetected(Transform _)
    {
        inRange = true;
        Attack();
    }
    public void OnLostDetection()
    {
        inRange = false;
    }
    public void Attack()
    {
        StartCoroutine(AttackCouroutine());
    }
    IEnumerator AttackCouroutine()
    {
        animator.SetTrigger("startAttack");
        yield return new WaitForSeconds(timeAttackPrepare);
        m_Attacking = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(timeDangerZone);
        m_Attacking = false;
        if (inRange)
        { Attack(); }
    }
    private void OnTriggerEnter(Collider _other)
    {
        //Layer 6 is Spear
        if (_other.gameObject.layer == 6)
        {
            Debug.Log("Invulnerable for: " + timeShieldProtect + "s.");
            animator.SetTrigger("Resist");
            health.SetInvulnerability(timeShieldProtect);
        }
        if (!m_Attacking || _other.gameObject.layer != 3) // 3 for player
            return;
        Health target;
        if (_other.gameObject.TryGetComponent(out target))
        {
            target.LoseHP(damage);
        }
    }
    public void InterruptAttack()
    {
        animator.ResetTrigger("PrepareAttack");
        animator.ResetTrigger("Attack");
        m_Attacking = false;
        StopCoroutine(AttackCouroutine());
    }
}
