using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    [SerializeField] public bool isFalling { get; private set; } = false;
    [SerializeField, Min(0f)] private float fallAfterTime = 2f;

    [SerializeField] private bool resetable = true;
    [SerializeField, Min(0f)] private float resetAfterTime = 4f;

    private Patrol m_Patrol;
    private bool m_IsPatrolling;
    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;
    private Rigidbody m_Rigidbody;
    private bool m_AudioSourced = false;
    [SerializeField] SFXManager sfxManager;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSourced = TryGetComponent(out audioSource);
        sfxManager = SFXManager.s_instance;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = !isFalling;
        m_Rigidbody.freezeRotation = true;
        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;
        m_IsPatrolling = TryGetComponent(out m_Patrol);

        //Fall();
    }
    private void OnValidate()
    {
        m_IsPatrolling = TryGetComponent(out m_Patrol);
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void Fall()
    {
        if (isFalling)
            return;
        isFalling = true;
        if (m_AudioSourced)
            audioSource.Play();
        else
            sfxManager.Play("Platform_falling");
        m_Rigidbody.isKinematic = false;
        if (m_IsPatrolling)
            m_Patrol.enabled = false;
        StartCoroutine(Falling());
    }

    public void Fall(float _timeBefore)
    {
        if (isFalling)
            return;
        StartCoroutine(FallingAfter(_timeBefore));
    }

    public void FallAfter()
    {
        if (isFalling)
            return;
        StartCoroutine(FallingAfter(fallAfterTime));
    }

    public void ResetObject()
    {
        foreach (Transform child in transform)
            child.SetParent(null);
        transform.position = m_InitialPosition;
        transform.rotation = m_InitialRotation;
        m_Rigidbody.isKinematic = true;
        isFalling = false;
        if (m_IsPatrolling)
            m_Patrol.enabled = true;
    }

    IEnumerator Falling()
    {
        yield return new WaitForSeconds(resetAfterTime);
        m_Rigidbody.isKinematic = true;
        if (resetable)
        {
            ResetObject();
        }
        isFalling = false;
    }
    IEnumerator FallingAfter(float _timeBefore)
    {
        yield return new WaitForSeconds(_timeBefore);
        Fall();
    }
}
