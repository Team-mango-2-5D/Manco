using System;
using UnityEngine;

public class SFXManager : SoundManager
{
    public static SFXManager s_instance;
    [SerializeField,Range(0f,1f)] private float varVolume = 0.2f;
    [SerializeField,Range(0f, 1f)] private float varMinPitch = 0.2f;
    [SerializeField,Range(0f, 1f)] private float varMaxPitch = 0.2f;


   void Awake() //Use new if problems here
    {
        if (s_instance == null)
            s_instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(s_instance);
        base.Awake();
    }
    public void PlayAtPosition(string _name, Vector3 _position)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + _name + "not found !");
            return;
        }
        if (!s.isPlaying)
        {
            //Random part here:
            s.source.volume = UnityEngine.Random.Range(1f - varVolume, 1f);
            s.source.pitch = UnityEngine.Random.Range(1f-varMinPitch, 1f + varMaxPitch);
            s.source.transform.position = _position;
            StartCoroutine(DurationCoroutine(s));
        }
    }
}
