using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    [Header("ABOUT")]
    public GameObject aboutUI;
    public GameObject creatorContent;
    public GameObject mechanicsContent;
    [Space(5)]

    [Header("")]
    public GameObject soundPanelUI;
    public Toggle playOnAndOffUI;
    public GameObject MainMenuCanvas;
    public GameObject IntroStoryCanvas;
    public VideoPlayer videoPlayer;
    public AsyncLoader asyncLoader;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.MMenuThemeSound);
        playOnAndOffUI.isOn = false;
    }
    private void Update()
    {
        CheckIfPlayStory();
    }


    #region ABOUT
    public void OnAboutClicked()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
        aboutUI.SetActive(true);
    }

    public void OnCreatorClicked()
    {
        mechanicsContent.SetActive(false);
        creatorContent.SetActive(true);
    }
    public void OnMechanicsClicked()
    {
        creatorContent.SetActive(false);
        mechanicsContent.SetActive(true);
    }

    public void OnExitAboutClicked()
    {
        aboutUI.SetActive(false);
    }

    public void OnExitSettings()
    {
        soundPanelUI.SetActive(false);
    }
    #endregion

    

    #region SETTINGS
    public void OnSettingsClicked()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
        if (soundPanelUI.activeSelf)
            soundPanelUI.SetActive(false);
        else
            soundPanelUI.SetActive(true);
    }
    public void OnSoundExitClicked()
    {
        soundPanelUI.SetActive(false);
    }

    #endregion

    public void OnExitClicked()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
        Application.Quit();
    }


    void CheckIfPlayStory()
    {
        if(playOnAndOffUI.isOn)
        {

            //MainMenuCanvas.SetActive(false);
            asyncLoader.GetCanvases(IntroStoryCanvas,MainMenuCanvas, videoPlayer.length);
            //IntroStoryCanvas.SetActive(true);
        }
        else
        {
            asyncLoader.RemoveCanvases();
        } 
            
    }


}
