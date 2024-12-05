using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Patrol))]
[RequireComponent(typeof(Animator))]
public class Chase : MonoBehaviour, IDetectable
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float breakTime = 1f;

    private bool m_InChase = false;
    private Transform m_playerPos = null;
    private Patrol m_Patrol;
    private Animator m_Animator;


    // Start is called before the first frame update
    void Start()
    {
        m_Patrol = GetComponent<Patrol>();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_InChase)
            Seek();
    }

    private void Seek()
    {
        Vector3 direction = (m_playerPos.position - transform.position).normalized;
        transform.position += direction * speed * Time.fixedDeltaTime;
        transform.LookAt(new Vector3(m_playerPos.position.x, transform.position.y, m_playerPos.position.z));
    }

    public void OnDetected(Transform _player)
    {
        m_InChase = true;
        m_playerPos = _player;
        m_Animator.SetBool("FollowPlayer", m_InChase);
        Debug.Log("Chase");
            m_Patrol.enabled = false;
    }
    public void OnLostDetection()
    {
        m_InChase = false;
        m_playerPos = null;
        SleepFor(breakTime);
    }
    public void SleepFor(float _time)
    {
        StartCoroutine(BreaktimeCoroutine(_time));
    }
    private IEnumerator BreaktimeCoroutine(float _breaktime)
    {
        yield return new WaitForSeconds(_breaktime);
        if (m_playerPos == null)
        {
            m_Animator.SetBool("FollowPlayer", m_InChase);
            m_Patrol.enabled = true;
        }
    }
}
