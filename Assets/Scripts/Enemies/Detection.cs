using UnityEngine;

public interface IDetectable
{
    public void OnDetected(Transform _detected);
    public void OnLostDetection();
}


public class Detection : MonoBehaviour
{
    [SerializeField] private MonoBehaviour reactScript ;
    IDetectable m_Detectable ;
    private void Start()
    {
        m_Detectable = reactScript.GetComponent<IDetectable>();
    }
    private void OnTriggerEnter(Collider _other)
    {
        //Layer 3 is player
        if (_other.gameObject.layer == 3)
        {
            m_Detectable.OnDetected(_other.transform);

        }
    }
    private void OnTriggerExit(Collider _other)
    {
        //Layer 3 is player
        if (_other.gameObject.layer == 3)
        {
            m_Detectable.OnLostDetection();
        }
    }
}
