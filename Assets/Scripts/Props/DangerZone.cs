using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DangerZone : MonoBehaviour
{
    [SerializeField] private Damage damage = new Damage(1, 1f, 0f);
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.5f);
    private bool m_isEnemy;

    private void Start()
    {
        m_isEnemy = gameObject.layer == 8;
    }
    private void OnValidate()
    {
        m_isEnemy = gameObject.layer == 8;
    }
    private void OnTriggerStay(Collider _other)
    {
        if (m_isEnemy && _other.gameObject.layer == 8) // 8 for Enemy
            return;
        Health target;
        if (_other.gameObject.TryGetComponent(out target))
        {
            target.LoseHP(damage);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
