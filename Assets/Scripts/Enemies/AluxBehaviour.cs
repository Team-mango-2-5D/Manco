using UnityEngine;

[RequireComponent(typeof(Patrol))]
[RequireComponent(typeof(Chase))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class AluxBehaviour : MonoBehaviour
{
    [SerializeField] private Collider hitBox;
    [SerializeField] private Animator animator;
    private void Awake()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
        if (hitBox == null)
            hitBox = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.layer == 3)
            animator.SetTrigger("Attack");
    }
}
