using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //Members
    #region SERIALIZED
    #region MOVEMENT
    [Header("Movement")]
    [Tooltip("Hollow Knight = 9.5\nSuper Meat Boy = 25\nCeleste = 1"), SerializeField, Min(0f)]
    private float maxSpeed = 10f;
    [SerializeField, Tooltip("Hollow Knight does not")]
    private bool conserveMomentum = true;
    [Tooltip("Hollow Knight = 9.5\nSuper Meat Boy = 2 \nCeleste = 2.5"), SerializeField, Range(0, 20)]
    private float runAcceleration = 9.5f;
    [Tooltip("Hollow Knight = 9.5\nSuper Meat Boy = 10\nCeleste = 5"), SerializeField, Range(0, 20)]
    private float runDeceleration = 9.5f;
    [Header("\t Read Only")]
    [Tooltip("Hollow Knight = 50 \nSuper Meat Boy = 10\nCeleste = 11.36"), SerializeField]
    private float m_RunAccelerationAmount = 50f;
    [Tooltip("Hollow Knight = 50 \nSuper Meat Boy = 20\nCeleste = 22.72"), SerializeField]
    private float m_RunDecelerationAmount = 50f;
    [Header("\t Aerial")]
    [Tooltip("Hollow Knight = 1  \nSuper Meat Boy = 1 \nCeleste = 0.65"), SerializeField, Range(0, 1)]
    private float accelerationInAir = 0.65f;
    [Tooltip("Hollow Knight = 1  \nSuper Meat Boy = 0 \nCeleste = 0.65"), SerializeField, Range(0, 1)]
    private float decelerationInAir = 0.65f;
    #endregion // Move

    #region JUMP
    [Header("Jump")]
    [Tooltip("Hollow Knight = 0.2\nSuper Meat Boy = 0.2\nCeleste = 0.1"), SerializeField, Min(0f)]
    private float coyoteTime = 0.20f;
    [Tooltip("Hollow Knight = 0.2\nSuper Meat Boy = 0.2\nCeleste = 0.1"), SerializeField, Min(0f)]
    private float jumpBufferTime = 0.20f;
    [Tooltip("Angle in Deg"), SerializeField, Range(1, 180)]
    private float validJumpAngle = 10f;
    [SerializeField, Min(0f)]
    private float initialJumpStrength = 10f;
    [SerializeField, Min(0f)]
    private float continuousJumpStrength = 10f;
    [SerializeField, Tooltip("Per second"), Min(0f)]
    private float maxTimeJumping = 0.5f;

    [Header("\tFall")]
    [SerializeField, Range(1f, 5f)]
    private float gravityScale = 2f;
    [SerializeField, Range(0f, 10f)]
    private float minFallSpeed = 2f;
    [SerializeField, Range(0f, 1000f)]
    private float maxFallSpeed = 50f;

    [Header("Other Jump capacities")]
    [SerializeField, Min(0f)]
    private float rodeoJumpStrength = 10f;
    #endregion //Jump
    #region DASH
    [Header("Dash")]
    [SerializeField, Min(0f)]
    private byte maxDashes = 1;
    [Header("\t First part, no control")]
    [SerializeField, Tooltip("20 for Celeste"), Min(0f)]
    private float dashSpeed = 20f;
    [SerializeField, Tooltip("0.15 for Celeste"), Min(0f)]
    private float dashDuration = 0.15f;
    [Header("\t Second part, control back")]
    [SerializeField, Tooltip("15 for Celeste"), Min(0f)]
    private float dashEndSpeed = 15f;
    [SerializeField, Tooltip("0.15 for Celeste"), Min(0f)]
    private float dashEndDuration = 0.15f;
    [SerializeField, Tooltip("Time Before dash is actuated,\n 0.05 in Celeste \n Care! It freezes everything !"), Range(0f, 0.25f)]
    private float dashSleepTime = 0.05f;
    [SerializeField, Tooltip("Time Before dash is available"), Min(0f)]
    private float dashCooldown = 5f;
    #endregion//Dash

    #region SLIDE
    [Header("Slide")]
    [Tooltip("Hollow Knight = -12\nSuper Meat Boy = -15\nCeleste = 0"), SerializeField, Range(-30, 0)]
    private float m_SlideSpeed = -12f;
    [Tooltip("Hollow Knight = 12\nSuper Meat Boy = 3   \nCeleste = 0"), SerializeField, Range(0, 20)]
    private float m_SlideAcceleration = 12f;
    #endregion //Slide
    #region COMBAT
    [Header("Combat")]
    [SerializeField] private Damage attack1 = new Damage(1, 0.418f);
    [SerializeField] private Damage attack2 = new Damage(1, 0.418f);
    [SerializeField] private Damage attack3 = new Damage(2, 0.626f);

    [SerializeField, Tooltip("In degrees"), Range(0f, 90f)] private float angleUpAttack = 45f;

    [SerializeField] SoundList soundList;
    public bool OwnSpear
    {
        get { return m_OwnSpear; }
        set
        {
            if (m_OwnSpear == value)
                return;
            m_OwnSpear = value;
            if (m_OwnSpear)
            {
                m_SpearObject.SetActive(true);
                GetBackSpear();
            }
            else
            {
                m_Spear.ResetRotation();
                m_SpearObject.SetActive(false);
            }
        }
    }

    #endregion
    #endregion //Serialized

    private Vector3 m_Input = Vector3.zero;
    #region SPEED
    private float m_InitialSpeed;
    private float m_TargetSpeed = 0f;
    public bool isMoving { get; private set; } = false;

    #endregion//Speed
    #region LOOK
    public Vector2 lookAt { get; private set; } = Vector2.right;
    #endregion //Look
    #region JUMP
    private bool m_IsJumping = false;
    private bool m_JumpBuffered = false;
    private float m_LastJumpInputTime = 0f;
    private float m_ExtraGravityScale;
    public bool isGrounded { get; private set; } = true;
    private bool m_IsSliding = false;
    private bool m_IsRodeo = false;
    private bool m_IsRodeoAttacking = false;
    private float m_JumpTimer = 0f;
    private bool m_JumpButtonPressed = false;
    private bool isFalling = false;
    #endregion //jump
    #region DASH
    private float m_DashTimer = 0f;
    private byte m_DashCount = 0;
    private bool m_IsDashing = false;
    private bool m_IsEndDashing = false;
    #endregion //Dash
    #region THROW
    private bool m_CanThrow = true;
    #endregion //Throw
    #region COMPONENTS 
    private Rigidbody m_Rigidbody = null;
    private Animator m_Animator;
    private SpearBehaviour m_Spear;
    [SerializeField] private Footsteps footsteps;
    private GameObject m_SpearObject;
    public bool m_OwnSpear = true;

    private bool m_IsAnimated = false;
    #endregion //Components

    //Methods
    #region UNITYCALLED

    void Awake()
    {
        if (!soundList)
            soundList = GetComponent<SoundList>();
        m_InitialSpeed = maxSpeed;
        if (!TryGetComponent(out m_Rigidbody))
            Debug.LogError(name + ": Rigidbody missing !");
        m_IsAnimated = TryGetComponent(out m_Animator);
        if (!m_IsAnimated)
            Debug.Log(name + ": Animator missing !");
        m_Spear = GetComponentInChildren<SpearBehaviour>();
        m_SpearObject = m_Spear.gameObject;
    }
    private void Start()
    {
        if (!m_OwnSpear)
            m_SpearObject.SetActive(false);
    }
    public void OnRespawn()
    {
        GetBackSpear();
        StopVelocity();
        if (CheckGround())
            Land();
        m_Animator.SetTrigger("Respawn");
    }

    private void OnValidate()
    {
        m_ExtraGravityScale = gravityScale - 1f;
        m_RunAccelerationAmount = (50 * runAcceleration) / maxSpeed;
        m_RunDecelerationAmount = (50 * runDeceleration) / maxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxSpeed);
        runDeceleration = Mathf.Clamp(runDeceleration, 0.01f, maxSpeed);
        m_OwnSpear = OwnSpear;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //JumpBuffering
        if (m_JumpBuffered && isGrounded && !m_IsJumping)
            DelayedJump();
        //Stop Continuous Jump
        else if (m_JumpTimer > maxTimeJumping)
            m_JumpButtonPressed = false;
        //Continuous Jump
        else if (m_JumpButtonPressed)
            ContinuousJump();
        //Fall
        if (m_Rigidbody.velocity.y < -minFallSpeed)
            ControlledFall();
        //Dash Timer
        if (m_DashTimer >= 0f)
            m_DashTimer -= Time.fixedDeltaTime;
        else
            m_DashCount = 0;
        //Move
        if (m_IsSliding)
            Slide();
        else if (!m_IsDashing)
            DoMove();
    }
    private void LateUpdate()
    {
        if (m_IsAnimated)
        {
            float absoluteVelocity = MathF.Abs(m_Rigidbody.velocity.x);
            m_Animator.SetFloat("SpeedX", absoluteVelocity);
            footsteps.currentVelocity = absoluteVelocity;
            if (m_Spear.state == SpearBehaviour.State.held)
                m_Animator.ResetTrigger("Back Spear");
        }

    }
    #region COLLISIONS
    public void OnCollisionEnter(Collision _other)
    {
        foreach (ContactPoint contact in _other.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < validJumpAngle)
            {
                Land();
                return;
            }
        }
    }
    public void OnCollisionStay(Collision _other)
    {
        //Layer 7 is Sticky 8 is Enemy
        if (_other.gameObject.layer == 7 || _other.gameObject.layer == 8)
        {
            return;
        }
        //tolerance could be modulable by an angle (cos MinValidAngle in rad) here 30deg
        float tolerance = 0.86602540378f;
        foreach (ContactPoint contact in _other.contacts)
        {
            float horDot = Vector3.Dot(contact.normal, Vector3.right);
            if (!isGrounded && !m_JumpButtonPressed && (horDot == 1 || horDot == -1))
            {
                //Check input direction compared to contact
                if (m_Input.x * horDot < -tolerance)
                    m_IsSliding = true;
                else
                    m_IsSliding = false;
                return;
            }
        }
    }

    public void OnCollisionExit(Collision _other)
    {
        //Grounded
        if (CheckGround())
        {
            return;
        }
        if (m_IsSliding)
        {
            m_IsSliding = false;
        }
        else if (!m_IsJumping! && !m_IsDashing && !CheckGround())
        {
            Debug.Log("Coyote");
            StartCoroutine(CoyoteCoroutine());
            return;
        }
        LeaveGround();
    }

    #endregion //Collisions
    #endregion //Unity Called

    #region ACTIONS
    #region MOVE
    private void StopVelocity()
    {
        m_IsRodeo = false;
        m_IsRodeoAttacking = false;
        m_Rigidbody.velocity = Vector3.zero;
    }
    public void OnMove(InputAction.CallbackContext _context)
    {
        //Play Move Sound
        //Play Move Animation
        m_Input.x = _context.ReadValue<float>();
        if (m_Input.x == 0)
        {
            isMoving = false;
        }
        else
            isMoving = true;
        m_TargetSpeed = m_Input.x * maxSpeed;
    }
    private void DoMove()
    {
        float tolerance = 0.01f;
        float accelRate;
        if (conserveMomentum
            && Mathf.Abs(m_Rigidbody.velocity.x) > Mathf.Abs(m_TargetSpeed)
            && Mathf.Sign(m_Rigidbody.velocity.x) == Mathf.Sign(m_TargetSpeed)
            && Mathf.Abs(m_TargetSpeed) > 0.01f
            && isGrounded)
            accelRate = 0;
        else if (isGrounded || m_JumpButtonPressed)
            accelRate = (Mathf.Abs(m_TargetSpeed) > tolerance) ? m_RunAccelerationAmount : m_RunDecelerationAmount;
        else
            accelRate = (Mathf.Abs(m_TargetSpeed) > tolerance) ? m_RunAccelerationAmount * accelerationInAir : m_RunDecelerationAmount * decelerationInAir;

        float speedDif = m_TargetSpeed - m_Rigidbody.velocity.x;

        float movement = speedDif * accelRate;

        m_Rigidbody.AddForce(movement * Vector2.right, ForceMode.Force);
    }
    #region FALL
    public void StopFall()
    {
        if (m_Rigidbody.velocity.y > -minFallSpeed)
            return;
        m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0f, m_Rigidbody.velocity.z);
        m_IsRodeo = false;
        m_IsRodeoAttacking = false;
    }
    #endregion //Fall
    #endregion //move
    #region JUMP
    public void OnJump(InputAction.CallbackContext _context)
    {
        if (m_JumpTimer > maxTimeJumping)
        {
            m_JumpButtonPressed = false;
            return;
        }
        if (_context.phase == InputActionPhase.Performed)
        {
            m_LastJumpInputTime = Time.time;

            if (isGrounded)
            {
                m_JumpButtonPressed = true;
                DoJump();
                if (m_IsAnimated)
                    //Play Jump Animation
                    m_Animator.SetTrigger("Jump");
            }
            else
                m_JumpBuffered = true;
        }
        else
            m_JumpButtonPressed = false;
    }
    private void ContinuousJump()
    {
        m_JumpTimer += Time.fixedDeltaTime;
        m_Rigidbody.AddForce(Vector3.up * continuousJumpStrength * Time.fixedDeltaTime, ForceMode.Impulse);
    }
    private void DelayedJump()
    {
        float timeSinceJump = Time.time - m_LastJumpInputTime;
        if (timeSinceJump < jumpBufferTime && !m_IsJumping)
        {
            DoJump();
        }
        m_JumpBuffered = false;
    }
    private void DoJump()
    {
        if (!m_IsJumping)
            m_Rigidbody.AddForce(transform.up * initialJumpStrength, ForceMode.Impulse);
        m_IsJumping = true;
    }
    public void DoBump(Vector3 _direction, float _force)
    {

        m_IsJumping = true;
        if (m_DashCount > 0)
        {
            m_DashCount--;
            m_DashTimer = 0f;
        }
        LeaveGround();
        m_DashTimer = 0f;
        if (m_IsRodeo)
            m_IsRodeo = false;
        if (m_IsRodeoAttacking)
        {
            if (m_IsRodeoAttacking)
            {
                m_Spear.StopRodeoAttack();
                m_IsRodeoAttacking = false;
            }
        }
        StopVelocity();
        m_Rigidbody.AddForce(_direction * _force, ForceMode.Impulse);
    }
    public void OnRodeoJump(InputAction.CallbackContext _context)
    {

        if (!CheckGround() && !m_IsRodeo)
            if (_context.phase == InputActionPhase.Started)
            {
                m_Rigidbody.AddForce(Vector3.down * rodeoJumpStrength, ForceMode.Impulse);
                m_IsRodeo = true;
            }
    }
    #endregion //Jump
    #region DASH
    public void OnDash(InputAction.CallbackContext _context)
    {

        if (m_DashCount < maxDashes)
            if (_context.phase == InputActionPhase.Performed)
            {
                //Play Dash Animation
                DoDash();
                //Play Dash Sound
                soundList.Play("Player_dash");
            }

    }
    private void DoDash()
    {
        Sleep(dashSleepTime);
        Vector3 direction;
        if (m_Input == Vector3.zero)
        {
            direction = new Vector3(lookAt.x, 0f, 0f);
        }
        else
            direction = m_Input;
        StartCoroutine(DashCoroutine(direction));
        m_DashTimer = dashCooldown;
    }
    #endregion //Dash
    #region SPEAR
    #region THROW
    public void OnThrow(InputAction.CallbackContext _ctxt)
    {
        if (!m_OwnSpear || _ctxt.phase != InputActionPhase.Performed)
            return;
        if (m_CanThrow && m_Spear.state == SpearBehaviour.State.held)
        {
            if (m_IsAnimated)
            {
                m_Animator.SetTrigger("Throw Spear");
            }
            m_Spear.DoThrow();
            if (!isGrounded)
            {
                m_CanThrow = false;
                StopFall();
            }
        }
        else if (m_Spear.canCallBack && m_Spear.state != SpearBehaviour.State.held)
        {
            //For now same animation reversed
            if (m_IsAnimated && !m_Animator.GetBool("Back Spear"))
            {
                m_Animator.SetTrigger("Back Spear");
                m_Animator.ResetTrigger("Throw Spear");
            }
            m_Spear.CallBack();
        }
    }
    private void GetBackSpear()
    {
        if (m_OwnSpear)
            m_Spear.GetBackSpear();
    }
    #endregion //Throw
    #region ATTACK
    public void OnAttack(InputAction.CallbackContext _context)
    {
        if (!m_OwnSpear || m_Spear == null || m_Spear.state != SpearBehaviour.State.held)
            return;
        if (m_IsRodeo)
        {
            m_Spear.DoRodeoAttack();
            m_IsRodeoAttacking = true;
            return;
        }
        //Could be Serialized but now, it is for over 45deg (sin 45deg)
        float tolerance = 0.71f;
        m_Spear.ChooseAttack(m_Input.y > tolerance ? angleUpAttack : 0f);
        if (m_Spear.attackCount == 1)
        {
            m_Animator.SetTrigger("Attack 1");
            m_Spear.DoAttack(attack1);
        }
        else if (m_Spear.attackCount == m_Spear.ComboNumber)
        {
            m_Animator.SetTrigger("Attack 3");
            m_Spear.DoAttack(attack3);
        }
        else if (m_Spear.attackCount > 1)
        {
            m_Animator.SetTrigger("Attack 2");
            m_Spear.DoAttack(attack2);
        }
    }
    #endregion //Attack
    #endregion //Spear
    public void OnLookAt(InputAction.CallbackContext _context)
    {
        Vector2 dir = _context.ReadValue<Vector2>();
        m_Input = new Vector3(dir.x, dir.y, 0f);
        if (dir.x != 0f)
        {
            lookAt = dir.normalized;
            Vector3 lookAtPosition = transform.position + new Vector3(0f, 0f, dir.x);
            transform.LookAt(lookAtPosition);
        }
    }
    private void Slide()
    {
        float rate = 1 / Time.fixedDeltaTime;
        float speedDif = m_SlideSpeed - m_Rigidbody.velocity.y;
        float movement = speedDif * m_SlideAcceleration;
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * rate, Mathf.Abs(speedDif) * rate);
        m_Rigidbody.AddForce(movement * Vector3.up);
    }

    #region COROUTINES
    private void Sleep(float _duration)
    {
        StartCoroutine(nameof(PerformSleep), _duration);
    }

    private IEnumerator PerformSleep(float _duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(_duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1;
    }
    //Dash Coroutine
    IEnumerator DashCoroutine(Vector3 _dir)
    {
        //StopVelocity();
        float startTime = Time.time;

        m_DashCount++;
        m_IsDashing = true;

        while (Time.time - startTime <= dashDuration)
        {
            m_Rigidbody.velocity = _dir.normalized * dashSpeed;
            yield return null;
        }

        startTime = Time.time;

        m_IsEndDashing = true;

        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration
        m_Rigidbody.velocity = _dir.normalized * dashEndSpeed;

        while (Time.time - startTime <= dashEndDuration)
        {
            //Stop on opposite input
            if (Vector3.Dot(_dir, m_Input) < 0)
            {
                break;
            }
            yield return null;
        }
        m_IsEndDashing = false;
        m_IsDashing = false;
    }
    IEnumerator CoyoteCoroutine()
    {
        if (!m_IsJumping && !m_IsDashing)
        {
            yield return new WaitForSeconds(coyoteTime);
            if (!CheckGround())
            {
                LeaveGround();
            }
        }
    }
    #endregion //Coroutines
    #endregion //Actions
    #region STATES
    private bool CheckGround()
    {
        float tolerance = 0.3f;
        Vector3 rayDirection = Vector3.down;
        if (Physics.Raycast(transform.position, rayDirection, transform.localScale.y + tolerance))
        {
            return true;
        }
        return false;
    }
    private void LeaveGround()
    {
        isGrounded = false;
        if (m_IsAnimated)
            m_Animator.SetBool("In Air", true);
    }
    private void ControlledFall()
    {
        //Increased Gravity only once
        if (!m_IsSliding)
        {
            Vector3 gravity = Physics.gravity * m_ExtraGravityScale;
            m_Rigidbody.AddForce(gravity, ForceMode.Acceleration);
        }
        isFalling = true;
        //Max Speed Here
        m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, Mathf.Max(m_Rigidbody.velocity.y, -maxFallSpeed), m_Rigidbody.velocity.z);
    }
    private void Land()
    {
        //Play Land Sound
        //Play Land Animation
        if (m_IsAnimated)
            m_Animator.SetBool("In Air", false);
        isGrounded = true;
        isFalling = false;
        m_IsSliding = false;
        m_IsJumping = false;
        if (m_IsRodeo)
        {
            m_IsRodeo = false;
            if (m_IsRodeoAttacking)
            {
                m_Spear.StopRodeoAttack();
                m_IsRodeoAttacking = false;
                soundList.Play("Player_groundslam");
            }
        }
        m_CanThrow = true;
        m_JumpTimer = 0f;
        m_DashTimer = 0f;
    }
    #endregion //states
}
