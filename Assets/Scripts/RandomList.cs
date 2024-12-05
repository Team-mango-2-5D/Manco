using System;
using UnityEngine;

[Serializable]
class RandomList
{
    [SerializeField]
    internal string name;
    [SerializeField]
    internal string[] randomSoundList;
    public string ChooseRandom()
    {
        int randomIdx = UnityEngine.Random.Range(0, randomSoundList.Length);
        return randomSoundList[randomIdx];
    }
}