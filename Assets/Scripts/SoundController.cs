using UnityEngine;
using UnityEngine.UI;
public class SoundController : MonoBehaviour
{
    public Slider musicSlider, sfxSlider;

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(musicSlider.value);
    }
    public void sfxVolume()
    {
        AudioManager.Instance.SFXVolume(sfxSlider.value);
    }
}
