using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CheckPoint : MonoBehaviour
{
    [Header("Gizmo")]
    [SerializeField] private float size = 0.2f;
    [SerializeField] private Color colorWhenInactive = new Color(1f, 1f, 1f, 0.75F);
    [SerializeField] private Color colorWhenActive = new Color(1f, 0f, 1f, 0.75F);


    private bool m_IsActive = false;
    
    private void OnTriggerEnter(Collider _other)
    {
        //Layer 3 is Player
        if (_other.gameObject.layer != 3)
            return;
        _other.GetComponent<RespawnBehaviour>().NewCheckpoint(this);
        m_IsActive = true;
    }
    private void OnDrawGizmosSelected()
    {
        {  
            Gizmos.color = m_IsActive? colorWhenActive:colorWhenInactive;
            Gizmos.DrawSphere(transform.position, size);
        }
    }
    public void Deactivate()
    {
        m_IsActive = false;
    }
}
