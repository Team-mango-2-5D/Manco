using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSave : MonoBehaviour
{
    [SerializeField]
    private Slider volumeSlider;
    [SerializeField]
    private TextMeshProUGUI volumeText;
    private float volume = 0.8f;
    // Start is called before the first frame update
    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener(
             delegate { SaveVolume(); });
    }
    void OnEnable()
    {
        LoadValue();
    }

    // Update is called once per frame
    void VolumeSlider(float _value)
    {
        volumeText.text = _value.ToString();
    }

    public void SaveVolume()
    {
        volume = volumeSlider.value;
        VolumeSlider(volume * 100);
        PlayerPrefs.SetFloat(volumeText.text, volume);
        AudioListener.volume = volume;
        LoadValue();
    }
    void LoadValue()
    {
        volume = PlayerPrefs.GetFloat("VolumeValue");
        VolumeSlider(volume);
    }
}
