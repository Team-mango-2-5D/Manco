using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlatformBehaviour : MonoBehaviour, IActivable
{
    [SerializeField][Range(1f, 180f)] private float validStickAngle = 10f;
    [SerializeField] private bool activated;

    private List<GameObject> m_ObjectsOnTop = new List<GameObject>();
    private bool m_IsFalling;
    private FallingObject m_Fall;

    private float m_CosStickAngle;
    // Start is called before the first frame update
    void Start()
    {
        m_CosStickAngle = (Mathf.Cos(Mathf.Deg2Rad * validStickAngle));
        m_IsFalling = TryGetComponent(out m_Fall);
    }

    private void OnValidate()
    {
        m_CosStickAngle = (Mathf.Cos(Mathf.Deg2Rad * validStickAngle));
        m_IsFalling = TryGetComponent(out m_Fall);
    }
    private void OnCollisionEnter(Collision _other)
    {
        if (m_IsFalling && m_Fall.isFalling)
            return;
        foreach (ContactPoint contact in _other.contacts)
        {
            if (Vector3.Dot(contact.normal, transform.up) < 1 - m_CosStickAngle)
            {
                if (m_IsFalling && _other.gameObject.layer == 3)
                    m_Fall.FallAfter();
                m_ObjectsOnTop.Add(_other.gameObject);
                _other.transform.SetParent(transform);
                return;
            }
        }
    }
    private void OnCollisionExit(Collision _other)
    {
        if (!m_ObjectsOnTop.Contains(_other.gameObject))
            return;
        m_ObjectsOnTop.Remove(_other.gameObject);
        _other.transform.SetParent(null);
    }
    public void Activate()
    {
        if (activated)
            return;
        activated = true;
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        if (!activated)
            return;
        activated = false;
        m_Fall.ResetObject();
        m_ObjectsOnTop.Clear();
        foreach (Transform child in transform)
            child.SetParent(null);
        gameObject.SetActive(false);
    }
}
