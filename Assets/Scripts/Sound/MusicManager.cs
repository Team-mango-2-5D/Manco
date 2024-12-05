using System.Runtime.InteropServices.WindowsRuntime;

public class MusicManager : OneAtATimeSoundManager
{
    public static MusicManager s_instance;
    public bool MuteMusic
    {
        get { return m_mute; }
        set
        {
            if (m_mute == value)
                return;
            foreach (Sound s in sounds)
                s.mute = value;
        }
    }
    private bool m_mute = false;
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
}
