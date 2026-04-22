using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class AsyncLoader : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreenGame;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] PauseManager pauseManager;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;

    GameObject mainMenuCanvas;
    GameObject introStoryCanvas;

    double videoLength= 0;

    public void LoadLevelBtnMainMenu(string leveltoLoad)
    {
        AudioManager.Instance.StopSound("music");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
        Time.timeScale = 1f;
        pauseManager.IsPaused = false;
        AudioListener.pause = false;
        AudioManager.Instance.StopSound("music");
        
        mainMenu.SetActive(false);
        loadingScreenGame.SetActive(true);

        StartCoroutine(LoadLevelAsync(leveltoLoad));
    }

    public void LoadLevenBtnGame(string leveltoLoad)
    {
        AudioManager.Instance.StopSound("music");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
        Time.timeScale = 1f;
        pauseManager.IsPaused = false;
        AudioListener.pause = false;

        loadingScreenGame.SetActive(true);

        StartCoroutine(LoadLevelAsync(leveltoLoad));
    }

    public IEnumerator LoadLevelAsync(string leveltoLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(leveltoLoad);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }


    public void OnStoryMainMenuPlay()
    {
        AudioManager.Instance.StopSound("music");
        if (introStoryCanvas != null && mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
            introStoryCanvas.SetActive(true);
            StartCoroutine(WaitForVideoToEnd());
        }
        else
        {
            LoadLevelBtnMainMenu("Game");
        }
    }

    public void GetCanvases(GameObject IntroStoryCanvas, GameObject MainMenucanvas, double length)
    {
        introStoryCanvas = IntroStoryCanvas;
        mainMenuCanvas = MainMenucanvas;
        videoLength = length;
    }

    public void RemoveCanvases()
    {
        introStoryCanvas = null;
        mainMenuCanvas = null;
    }

    IEnumerator WaitForVideoToEnd()
    {
        float timeElapsed = 0f;

        Debug.Log("Waiting for " + videoLength + " seconds");

        while (timeElapsed < videoLength)
        {
            timeElapsed += Time.deltaTime;
            yield return null; // wait for next frame
        }

        StartCoroutine(LoadLevelAsync("Game"));
    }

}
