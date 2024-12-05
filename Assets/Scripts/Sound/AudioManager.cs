using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager s_instance;

    [Range(0f, 1f)] public float volume = 1f;

    public SFXManager sfxManager { get; private set; }
    private MusicManager m_Music;
    private AmbientManager m_Ambient;

    void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(s_instance);

        m_Music = GetComponentInChildren<MusicManager>();
        m_Ambient = GetComponentInChildren<AmbientManager>();
        sfxManager = GetComponentInChildren<SFXManager>();
    }

    private void Start()
    {
    }
    // Update is called once per frame
    public void PlayFX(string _name)
    {
        sfxManager.Play(_name);
    }
    public void PlayFX(string _name, Vector3 _position)
    {
        sfxManager.PlayAtPosition(_name, _position);
    }
    //Current
    public void PlayCurrentMusic()
    {
        m_Music.PlayCurrent();
    }
    public void PlayMusic(string _name)
    {
        m_Music.Play(_name);
    }
    public void PlayMusic(int _index)
    {
        m_Music.Play(_index);
    }
    //Current
    public void StopMusic()
    {
        m_Music.Stop();
    }
    public void PlayAmbient(string _name)
    {
        m_Ambient.Play(_name);
    }
    public void PlayAmbient(int _index)
    {
        m_Ambient.Play(_index);
    }
    //Current
    public void StopAmbient()
    {
        m_Ambient.Stop();
    }
}
