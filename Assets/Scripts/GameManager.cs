using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance;
    public AudioManager audioManager { get; private set; }
    public Transform HUD { get; private set; }
    private Transform m_Healthbar;
    private Transform m_WinMessage;
    private Transform m_Pause;
    private Transform m_PauseMenu;

    public enum SceneIdx : int
    {
        MAIN_MENU = 0,
        TUTO = 1,
        HUB = 2,
        LEVEL1 = 3,
        LEVEL2 = 4,
        LEVEL3 = 5,
        LEVEL4 = 6,
        LEVEL5 = 7
    }
    public SceneIdx activeScene { set { ChangeScene(value); } }


    void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(s_instance);
        audioManager = AudioManager.s_instance;
        HUD = transform.GetChild(0);
        m_WinMessage = HUD.GetChild(1).GetChild(0);
        m_Healthbar = HUD.GetChild(0).GetChild(0);
        m_Pause = transform.GetChild(1);
        m_PauseMenu = m_Pause.GetChild(0);
        m_PauseMenu.gameObject.SetActive(false);
        m_Pause.gameObject.SetActive(false);
    }
    public void Restart()
    {
        ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Close()
    {
        Application.Quit();
    }
    private void LoadSound(int _sceneIdx)
    {
        if (_sceneIdx == 2)
            audioManager.StopMusic();
        else
            audioManager.PlayMusic(_sceneIdx);

        if (_sceneIdx < 1)
            return;
        if (_sceneIdx == 2)
            audioManager.PlayAmbient("Amb_loop_hub");
        else if (_sceneIdx == 5)
            audioManager.PlayAmbient("Amb_loop_lava");
        else
            audioManager.PlayAmbient("Amb_loop_level");
    }
    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
        int index = SceneManager.GetActiveScene().buildIndex;
        LoadSound(index);
        ManageUIOnLoad(index);
    }
    public void ChangeScene(int _sceneIdx)
    {
        SceneManager.LoadScene(_sceneIdx);
        LoadSound(_sceneIdx);
        ManageUIOnLoad(_sceneIdx);
    }
    public void ChangeScene(SceneIdx _sceneIdx)
    {
        int index = (int)_sceneIdx;
        SceneManager.LoadScene(index);
        LoadSound(index);
        ManageUIOnLoad(index);
    }

    public void ManageUIOnLoad(int _sceneIdx)
    {
        m_WinMessage.gameObject.SetActive(false);
        if (_sceneIdx == (int)SceneIdx.MAIN_MENU)
        {
            DeactivatePause();
        }
        else
        {
            ActivatePause();
            DeactivatePauseMenu();
        }
        if (_sceneIdx == (int)SceneIdx.MAIN_MENU || _sceneIdx == (int)SceneIdx.HUB)
            DeactivateHealthBar();
        else
            ActivateHealthBar();
    }
    public void DeactivateHealthBar()
    {
        if(m_Healthbar.gameObject.activeSelf)
        m_Healthbar.gameObject.SetActive(false);
    }
    public void ActivateHealthBar()
    {
        if (!m_Healthbar.gameObject.activeSelf)
            m_Healthbar.gameObject.SetActive(true);
    }
    public void DeactivatePauseMenu()
    {
        m_PauseMenu.gameObject.SetActive(false);
    }
    public void ActivatePause()
    {
        m_Pause.gameObject.SetActive(true);
    }
    public void DeactivatePause()
    {
        m_Pause.gameObject.SetActive(false);
    }
}
