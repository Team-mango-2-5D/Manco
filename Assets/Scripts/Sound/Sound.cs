using UnityEngine;
[System.Serializable]
public class Sound
{

    public string name;
    public AudioClip clip;
    [Min(0f)] public float duration;
    [Header ("Read Only")]public bool isPlaying = false;
    [Header ("Read Only")]public float m_Time;
    [Range(0f,1f)]
    public float volume = 1f;
    [Range(0.1f,3f)]
    public float pitch = 1f;

    public bool mute;
    public bool loop;
    public bool playOnAwake;

    [Range(-1f, 1f)] public float panStereo;
    [Range(0f, 1f)] public float spatialBlend;

    [HideInInspector]
    public AudioSource source;
}
