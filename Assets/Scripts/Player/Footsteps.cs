using UnityEngine;

public class Footsteps : MonoBehaviour
{
    internal float currentVelocity { set; private get; } = 0f;
    [SerializeField] SoundList soundList;
    [SerializeField] float walkSpeed = 0.5f;
    [SerializeField] float runSpeed = 5f;
    public void PlayWalkSoundByIndex()
    {
        
        if(currentVelocity > walkSpeed && currentVelocity < runSpeed)
        soundList.PlayRandomListIndex(0);
    }
    public void PlayWalkSoundByName()
    {
        if (currentVelocity > walkSpeed && currentVelocity < runSpeed)
            soundList.PlayRandomListName("Footsteps");
    }
    public void PlayRunSoundByIndex()
    {
        if (currentVelocity >= runSpeed)
            soundList.PlayRandomListIndex(0);
    }

    public void PlayRunSoundByName()
    {
        if (currentVelocity >= runSpeed)
            soundList.PlayRandomListName("Footsteps");
    }
}
