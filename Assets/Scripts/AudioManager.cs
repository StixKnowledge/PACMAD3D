using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    //[SerializeField] AudioSource pacmanSource;
    //[SerializeField] Transform pacmanPosition;

    [Header("Audio Clips")]
    public AudioClip gameThemeSound;
    public AudioClip MMenuThemeSound;
    public AudioClip pacmanWaka;
    public AudioClip clickSound;
    public AudioClip decodingSound;
    public AudioClip caughtSound;
    public AudioClip pantingSound;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        
    }
    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlayMusic(AudioClip audioClip)
    {
        musicSource.clip = audioClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip audioClip)
    {
        if (sfxSource != null && audioClip != null)
        {
            sfxSource.PlayOneShot(audioClip);
        }
    }

    public void StopSound(string sourceName)
    {
        if(sourceName == "music")
        {
            musicSource.Stop();
        }
        else if(sourceName == "sfx")
        {
            sfxSource.Stop();
        }
        else
        {
            Debug.LogWarning("Audio source not found: " + sourceName);
        }
    }

}
