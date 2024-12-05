using Unity.VisualScripting;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public GameManager gameManagerInstance { get; private set; }
    public ScriptMachine HUD;
    public SFXManager sfxManager;
    public SoundList soundList;
    public PlayerController playerController { get; private set; }

    void Awake()
    {
        gameManagerInstance = GameManager.s_instance;   
    }
    private void Start()
    {
        HUD = gameManagerInstance.HUD.GetComponent<ScriptMachine>();
        this.sfxManager = gameManagerInstance.audioManager.sfxManager;
        soundList = GetComponent<SoundList>();
        soundList.SFxManager = sfxManager;
    }

    // Start is called before the first frame update
    public void HUDTrigger(string _eventName)
    {
        if (HUD == null)
        {
            Debug.LogError("No HUD");
        }
        HUD.TriggerUnityEvent(_eventName);
    }
}
