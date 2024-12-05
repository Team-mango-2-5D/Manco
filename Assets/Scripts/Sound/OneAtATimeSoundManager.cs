using System;
using UnityEngine;

public class OneAtATimeSoundManager : SoundManager
{
    private Sound m_CurrentlyPlaying;

    private void Start()
    {
        m_CurrentlyPlaying = sounds[0];
        foreach (Sound s in sounds)
            Debug.Log(s.name);
        Play(0);
    }
    public override void Play(string _name)
    {
        if (m_CurrentlyPlaying != null)
        {
            Stop();
        }
        Sound s = Array.Find(sounds, sound => sound.name == _name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + _name + " not found !");
            return;
        }
        s.source.Play();
        m_CurrentlyPlaying = s;
        m_CurrentlyPlaying.isPlaying = true;
    }
    public void Play(int _index)
    {
        if (m_CurrentlyPlaying != null)
        {
            Stop();
        }
        Sound s = sounds[_index];
        s.source.Play();
        m_CurrentlyPlaying = s;
        m_CurrentlyPlaying.isPlaying = true;
    }
    public void PlayCurrent()
    {
        if (m_CurrentlyPlaying == null || m_CurrentlyPlaying.isPlaying)
            return;
        m_CurrentlyPlaying.source.Play();
        m_CurrentlyPlaying.isPlaying = true;
    }
    public void Stop()
    {
        if (m_CurrentlyPlaying != null)
        m_CurrentlyPlaying.source.Stop();
        m_CurrentlyPlaying.isPlaying = false;
    }
}
