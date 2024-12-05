using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Animator))]
public class SpearBehaviour : MonoBehaviour
{
    internal enum State : byte
    {
        held,
        attack,
        combo,
        thrown,
        stuck,
        fix,
        back
    }
    [Header("Throw")]
    [SerializeField, Min(0f)] private float throwSpeed = 10;
    [SerializeField, Min(0f)] private float timeBeforeCanGetBack = 0.5f;
    [SerializeField, Min(0f)] private float throwTime = 1f;
    [Tooltip("Time the spear stay static")]
    [SerializeField, Min(0f)] private float autoReturnTime = 3f;
    [SerializeField] private Damage throwDamage = new Damage(1, 1f);

    [Header("Attack")]
    [SerializeField, Min(0f)] private int comboNumber = 3;
    public int ComboNumber => comboNumber;
    [SerializeField, Min(0f), Tooltip("Max time between two inputs before combo breaks")] private float comboStepTime = 2f;

    [SerializeField] private Damage RodeoAttackDamage = new Damage(1, 1f);
    [SerializeField, Min(0f)]
    private float rodeoBounceStrength = 10f;
    [SerializeField, Min(0f)]
    private float spearJumpForce = 20f;

    public bool canCallBack { get; private set; } = false;
    public bool hitSuccess { get; private set; } = false;
    public int attackCount { get; private set; } = 0;
    private float m_LastAttackInputTime = 0f;
    private bool m_InRodeo = false;
    private float m_BodyDiameter;
    private float m_BoxColliderYOffset;

    private Rigidbody m_Rigidbody;
    // Start is called before the first frame update
    private GameObject m_Parent;

    private Vector3 m_LocalPositionOffset;
    internal State state { get; private set; } = State.held;
    private Damage m_CurrentDamage;
    private Animator m_Animator;
    private BoxCollider m_BoxCollider;
    private PlayerController m_pController;
    private Transform m_Child;
    private Quaternion m_ChildRotationOffset;
    private bool isPlayerControlled;
    private bool m_AudioSourced = false;
    [SerializeField] SFXManager sfxManager;
    AudioSource audioSource;

    void Start()
    {
        m_AudioSourced = TryGetComponent(out audioSource);
        if (sfxManager == null)
            sfxManager = SFXManager.s_instance;
        m_Child = transform.GetChild(0);
        m_ChildRotationOffset = m_Child.localRotation;
        m_Parent = transform.parent.parent.gameObject;
        m_Rigidbody = transform.parent.GetComponent<Rigidbody>();
        m_BodyDiameter = transform.GetChild(0).GetChild(0).localScale.x;
        m_BoxCollider = GetComponent<BoxCollider>();
        m_BoxColliderYOffset = m_BoxCollider.size.y;
        m_BoxCollider.enabled = false;
        m_Animator = GetComponent<Animator>();
        isPlayerControlled = m_Parent.TryGetComponent(out m_pController);
        m_LocalPositionOffset = transform.localPosition;// Save the local position offset
    }
    void FixedUpdate()
    {
        if (hitSuccess)
            hitSuccess = false;
        if (state == State.back)
        {
            m_Rigidbody.velocity = (m_Parent.transform.position + m_LocalPositionOffset - transform.position).normalized * throwSpeed;
            transform.LookAt(m_Parent.transform.position);
        }
    }
    void LateUpdate()
    {
        if (state == State.combo)
            ResetCombo();
    }
    //Throw in front
    public void DoThrow()
    {
        Vector3 direction = m_Parent.transform.right;
        Vector3 lookAtPosition = transform.position + direction * 10.0f;
        ThrowAimTarget(lookAtPosition);
        ThrowDirection(direction);
    }
    public void DoThrow(Vector3 _target)
    {
        ThrowAimTarget(_target);
        Vector3 direction = (_target - m_Parent.transform.position).normalized;
        ThrowDirection(direction);
    }
    private void ThrowAimTarget(Vector3 _target)
    {
        m_CurrentDamage = throwDamage;
        m_Animator.SetBool("Spins", true);
        m_BoxCollider.enabled = true;
        transform.parent.SetParent(null, true);
        m_Rigidbody.isKinematic = false;
        transform.LookAt(_target);
    }
    private void ThrowDirection(Vector3 _direction)
    {
        m_Rigidbody.velocity += _direction * throwSpeed;
        state = State.thrown;
        StartCoroutine(OutOfRangeCoroutine());
        StartCoroutine(ThrowTimeCoroutine());
    }
    public void CallBack()
    {
        transform.parent.SetParent(null, true);
        m_Rigidbody.velocity = (m_Parent.transform.position + m_LocalPositionOffset - transform.position).normalized * throwSpeed;
        transform.LookAt(m_Parent.transform.position);
        m_Animator.SetBool("Spins", true);
        m_BoxCollider.size = new Vector3(m_BoxCollider.size.x, m_BoxColliderYOffset, m_BoxCollider.size.z); m_BoxCollider.isTrigger = true;
        m_Rigidbody.isKinematic = false;
        state = State.back;
    }

    IEnumerator OutOfRangeCoroutine()
    {
        yield return new WaitForSeconds(timeBeforeCanGetBack);
        canCallBack = true;
    }
    IEnumerator ThrowTimeCoroutine()
    {
        yield return new WaitForSeconds(throwTime);
        if (state == State.thrown)
            StartCoroutine(AutoReturnCoroutine());
    }
    IEnumerator AutoReturnCoroutine()
    {
        state = State.fix;
        m_Rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(autoReturnTime);
        if (state == State.fix)
            CallBack();
    }
    public void GetBackSpear()
    {
        m_BoxCollider.enabled = false;
        m_BoxCollider.size = new Vector3(m_BoxCollider.size.x, m_BoxColliderYOffset, m_BoxCollider.size.z);
        m_BoxCollider.isTrigger = true;
        transform.parent.SetParent(m_Parent.transform, true);
        transform.parent.localPosition = Vector3.zero;
        transform.parent.localRotation = Quaternion.identity;

        transform.localPosition = m_LocalPositionOffset; // Update the position using the saved offset
        Vector3 lookAtPosition = transform.position + transform.parent.parent.right;
        transform.LookAt(lookAtPosition);
        StopAllCoroutines();
        state = State.held;
        m_Animator.SetBool("Spins", false);
        m_Rigidbody.isKinematic = true;
        canCallBack = false;
    }
    public void OnTriggerStay(Collider _other)
    {
        if (state == State.back)
        {
            if (_other.gameObject == m_Parent)
            {
                GetBackSpear();
                return;
            }
        }
    }
    public void OnTriggerEnter(Collider _other)
    {
        if (state == State.thrown)
        {
            //Layer 7 is StickySurfaces
            if (_other.gameObject.layer == 7)
            {
                StickToSurface(_other.gameObject);
                return;
            }
            //Layer 9 is Platform 10 is Scenery
            if (_other.gameObject.layer == 9 || _other.gameObject.layer == 10)
            {
                StartCoroutine(AutoReturnCoroutine());
            }
        }

        if (isPlayerControlled && state == State.fix && _other.gameObject.layer == 3)
        {
            m_pController.DoBump(Vector3.up, spearJumpForce);
            sfxManager.PlayAtPosition("Player_spear_jump", transform.position);
        }
        //Layer 8 is Enemies, 3 is Player
        if ((_other.gameObject.layer == 8 && m_Parent.layer == 3) || (m_Parent.layer == 8 && _other.gameObject.layer == 3))
        {
            if (state == State.combo || state == State.attack)
            {
                Hitting();
            }
            else if (state != State.thrown && state != State.fix && state != State.back)
                return;
            //Do Damage attack
            DoDamage(_other.gameObject, m_CurrentDamage);
            Rigidbody rb;
            if (_other.TryGetComponent(out rb))
                rb.AddForce(transform.forward * m_CurrentDamage.knockBackForce);
        }
    }
    public void ChooseAttack(float _angleDirection = 0f)
    {
        if (state != State.held)
            return;
        if (_angleDirection != 0f)
            transform.parent.Rotate(Vector3.forward, _angleDirection);
        ComboCount();
        if (attackCount == comboNumber)
            state = State.combo;
        else
            state = State.attack;
    }
    public void DoAttack(Damage _damage)
    {
        m_CurrentDamage = _damage;
        if (state == State.combo)
            m_Animator.SetTrigger("Attack3");
        else
            m_Animator.SetTrigger("Attack1");

        StartCoroutine(AttackCoroutine(_damage.graceTime));
    }
    public void DoRodeoAttack()
    {
        if (state != State.held)
            return;
        m_BoxCollider.enabled = true;
        m_InRodeo = true;
        state = State.attack;
        m_CurrentDamage = RodeoAttackDamage;
        transform.parent.Rotate(Vector3.forward, -90f); ;
    }
    public void StopRodeoAttack()
    {
        if (state != State.attack)
            return;
        m_InRodeo = false;
        m_BoxCollider.enabled = false;
        transform.parent.localRotation = Quaternion.identity;
        state = State.held;
    }
    public void ResetCombo()
    {
        attackCount = 0;
    }

    private void Hitting()
    {
        if (isPlayerControlled)
        {
            m_pController.StopFall();
            if (m_InRodeo)
            {
                m_pController.DoBump(Vector3.up, rodeoBounceStrength);
                StopRodeoAttack();
            }
        }
        hitSuccess = true;
    }
    private void DoDamage(GameObject _target, Damage _damage)
    {
        Health health;

        //Do Damage throw (if DoT -> OnTriggerStay)
        if (_target.TryGetComponent(out health))
            health.LoseHP(_damage);
    }
    private void StickToSurface(GameObject _surface)
    {
        state = State.stuck;
        m_Animator.SetBool("Spins", false);
        m_Rigidbody.isKinematic = true;
        if (m_AudioSourced)
            audioSource.Play();
        else
            sfxManager.Play("Platform_spear");
        m_Rigidbody.velocity = Vector3.zero;
        m_BoxCollider.size = new Vector3(m_BoxCollider.size.x, m_BodyDiameter, m_BoxCollider.size.z);
        transform.parent.SetParent(_surface.transform, true);
        GetComponent<BoxCollider>().isTrigger = false;
        transform.localPosition = Vector3.zero;
    }

    private IEnumerator AttackCoroutine(float _animationTime = 1f)
    {
        //animation
        m_BoxCollider.enabled = true;
        yield return new WaitForSeconds(_animationTime);
        m_BoxCollider.enabled = false;
        transform.parent.localRotation = Quaternion.identity;
        state = State.held;
    }
    private void ComboCount()
    {
        float currentInputTime = Time.time;
        if (currentInputTime - m_LastAttackInputTime > comboStepTime)
            attackCount = 0;
        attackCount++;
        m_LastAttackInputTime = currentInputTime;
    }
    public void ResetRotation()
    {
        m_Child.localRotation = m_ChildRotationOffset;
    }
}
