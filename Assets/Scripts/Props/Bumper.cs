using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bumper : MonoBehaviour
{
    [SerializeField] private float bumpStrength = 15f;
    [SerializeField] private bool Sounded = false;
    private bool m_AudioSourced = false;
    [SerializeField] SFXManager sfxManager;
    AudioSource audioSource;


    void Start()
    {
        m_AudioSourced = TryGetComponent(out audioSource);
        if (sfxManager == null)
        {
            sfxManager = SFXManager.s_instance;
        }
    }
    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.layer != 3)
            return;
        else
        {
            _other.gameObject.GetComponent<PlayerController>().DoBump(transform.TransformDirection(Vector3.up), bumpStrength);
            //Play animation bounce
            //Play sound bounce
            if (!Sounded)
                return;
            if (m_AudioSourced)
                audioSource.Play();
            else
                sfxManager.Play("Platform_bump");
        }
    }

}
