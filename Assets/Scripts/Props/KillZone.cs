using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = new Color(0f, 0f, 0f, 0.5f);
    private Collider m_Collider;

    private void Start()
    {
        m_Collider = GetComponent<Collider>();
    }
    private void OnValidate()
    {
        m_Collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider _other)
    {
        Health target;
        if (_other.TryGetComponent(out target))
            target.InstaKill();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
