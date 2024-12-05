using System.Collections;
using UnityEngine;

    public class GolemBehaviour : MonoBehaviour, IDetectable
{
    [SerializeField] Patrol patrol;
    [SerializeField] Animator animator;

    private Transform m_Player = null;

    // Update is called once per frame
    void Update()
    {
        if (m_Player != null)
            transform.LookAt(new Vector3 (m_Player.position.x, transform.position.y, transform.position.z));
    }

    public void OnDetected(Transform _player) 
    { 
        m_Player = _player;
        patrol.enabled = false;
        animator.SetBool("isWalking", false);
    }
    public void OnLostDetection()
    {
        m_Player = null;
        patrol.enabled = true;
        animator.SetBool("isWalking", true);
    }
    
}
