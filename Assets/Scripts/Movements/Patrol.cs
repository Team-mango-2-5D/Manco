using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [Tooltip("Empty object with points in children")]
    [SerializeField] private Vector3[] path;
    [SerializeField, Min(0f)] private float speed = 1f;
    [SerializeField, Min(0f)] private float distanceThreshold = 0.3f;
    [SerializeField] private bool halfLap = false;
    [SerializeField] private bool oriented = false;


    private List<Vector3> m_CheckpointList = new List<Vector3>();
    private Vector3 m_CurrentCheckpoint = Vector3.zero;
    private int m_CurrentCheckpointIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (path != null)
            foreach (Vector3 point in path)
            {
                m_CheckpointList.Add(point);
            }
        if (m_CheckpointList.Count > 0)
            m_CurrentCheckpoint = m_CheckpointList[0];
        else
        {
            m_CurrentCheckpoint = transform.position;
        }
        if (oriented)
        {
            transform.LookAt(new Vector3 (m_CurrentCheckpoint.x, transform.position.y, transform.position.z));
        }
    }
    private void OnEnable()
    {
        if (oriented)
            transform.LookAt(new Vector3 (m_CurrentCheckpoint.x, transform.position.y, transform.position.z));
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += (m_CurrentCheckpoint - transform.position).normalized * speed * Time.fixedDeltaTime;
        CPProgression();
    }
    private void CPProgression()
    {
        if (m_CheckpointList.Count == 0)
        {
            return;
        }
        else if (Vector3.SqrMagnitude(m_CurrentCheckpoint - transform.position) < distanceThreshold * distanceThreshold) //Squared
        {
            m_CurrentCheckpointIndex = (m_CurrentCheckpointIndex + 1);
            if (m_CurrentCheckpointIndex == m_CheckpointList.Count)
            {
                if (halfLap)
                    m_CheckpointList.Reverse();
                m_CurrentCheckpointIndex = 0;
            }
            m_CurrentCheckpoint = m_CheckpointList[m_CurrentCheckpointIndex];
            if (oriented)
                transform.LookAt(new Vector3 (m_CurrentCheckpoint.x, transform.position.y, m_CurrentCheckpoint.z));
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (path == null) return;
        float size = 0.1f;
        foreach (Vector3 point in path)
        {
            Gizmos.color = new Color(1, 1, 0, 0.75F);
            Gizmos.DrawSphere(point, size);
        }
    }
}
