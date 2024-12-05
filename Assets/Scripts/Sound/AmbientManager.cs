public class AmbientManager : OneAtATimeSoundManager
{
    public static AmbientManager s_instance;

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
