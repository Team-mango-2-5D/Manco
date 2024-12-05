using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    [SerializeField] private AudioSource audioSource;
    internal void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = Instantiate(audioSource, transform) ;
            s.source.name = s.name ;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.mute = s.mute;
            s.source.panStereo = s.panStereo;
            s.source.spatialBlend = s.spatialBlend;
        }
    }
    internal void OnValidate()
    {
        foreach (Sound s in sounds)
        {
            s.source.name = s.name;
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.mute = s.mute;
            s.source.panStereo = s.panStereo;
            s.source.spatialBlend = s.spatialBlend;
        }
    }

    // Update is called once per frame
    public virtual void Play(string _name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + _name + "not found !");
            return;
        }
        if (!s.isPlaying)
        {
            StartCoroutine(DurationCoroutine(s));
        }
    }
    public void Stop(string _name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + _name + "not found !");
            return;
        }
        s.source.Stop();
    }
    internal IEnumerator DurationCoroutine(Sound _s)
    {
        _s.isPlaying = true;
        //1 frame to avoid conflict, should try without if desynchronized
        yield return null;
        _s.source.Play();
        yield return new WaitForSeconds(_s.duration);
        _s.isPlaying = false;
    }
}
