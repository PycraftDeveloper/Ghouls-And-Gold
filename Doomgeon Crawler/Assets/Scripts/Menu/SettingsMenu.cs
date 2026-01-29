using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;

    public void Start()
    {
        MusicVolumeSlider.value = Registry.Music_Volume;
        SFXVolumeSlider.value = Registry.SFX_Volume;
    }

    public void OnMusicVolumeSliderChanged()
    {
        Registry.Music_Volume = MusicVolumeSlider.value;
    }

    public void OnSFXVolumeSliderChanged()
    {
        Registry.SFX_Volume = SFXVolumeSlider.value;
    }
}