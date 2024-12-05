using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private float minValueZoom = 5f;
    [SerializeField] private float maxValueZoom = 15f;
    [SerializeField] private Quaternion rotationTarget;

    public float smoothSpeed = 0.5f;
    private Vector3 locationOffset;
    private Quaternion m_currentRotation;

    void Awake()
    {
        locationOffset = transform.position - target.position;
        m_currentRotation = transform.rotation * Quaternion.Inverse(target.rotation);
    }
    private void FixedUpdate()
    {
        //Vector3 desiredPosition = target.position + target.rotation * locationOffset;
        Vector3 desiredPosition = target.position + locationOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        Quaternion desiredrotation = rotationTarget * m_currentRotation;
        Quaternion smoothedrotation = Quaternion.Lerp(transform.rotation, desiredrotation, smoothSpeed);
        transform.rotation = smoothedrotation;
    }
    public void OnZoom(InputAction.CallbackContext _context)
    {
        float zoom = Mathf.Clamp(locationOffset.y - _context.ReadValue<float>(), minValueZoom, maxValueZoom);
        locationOffset = new Vector3(locationOffset.x, zoom, locationOffset.z);
    }
}
