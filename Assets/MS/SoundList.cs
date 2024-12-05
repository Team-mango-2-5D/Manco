using System;
using System.Reflection;
using UnityEngine;

public class SoundList : MonoBehaviour
{
    [SerializeField]
    private string[] soundList;
    [SerializeField]
    internal RandomList[] randomSoundList;

    //Set only
    [SerializeField] public SFXManager SFxManager { set; private get;}

    
    // Start is called before the first frame update
    void Awake()
    {
        if (!SFxManager)
        SFxManager = SFXManager.s_instance;
    }

    public void Play(string _name)
    {
        SFxManager.PlayAtPosition(_name, transform.position);   
    }
    public void Play(int _index)
    {
        Debug.Log("Playing "+soundList[_index]);
        SFxManager.PlayAtPosition(soundList[_index], transform.position);
    }
    public void Stop(string _name)
    {
        SFxManager.Stop(_name);
    }
    public void Stop(int _index)
    {
        SFxManager.Stop(soundList[_index]);
    }
    public void PlayRandomListIndex(int _index)
    {
        string randomItem = randomSoundList[_index].ChooseRandom();
        Debug.Log("Playing " + randomItem);
        SFxManager.PlayAtPosition(randomItem, transform.position);
    }
    public void PlayRandomListName(string _name)
    {
        foreach(RandomList item in randomSoundList)
        {
            if (item.name == _name)
            {
                string randomItem = item.ChooseRandom();
                Debug.Log("Playing " + randomItem);
                SFxManager.PlayAtPosition(randomItem, transform.position);
            //Could return here for performances
            //I keep it in case several lists with same name
            }
        }
    }
}