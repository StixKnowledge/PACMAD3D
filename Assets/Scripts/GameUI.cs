using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public FoodPopulationSystem foodPopulationSystem;
    public PlayerInteraction playerInteraction;
    [SerializeField] PauseManager pauseManager;

    public GameObject[] virusImageRemaining;
    public GameObject SettingsUI;

    public int virusRemainingCount = 0;

    private void Start()
    {
        virusRemainingCount = foodPopulationSystem.normalInfectedFoodCount;
        playerInteraction.OnCuredInfection += UpdateRemainingVirusCount;
        pauseManager.OnPauseIsPressed += OnPauseClicked;
        pauseManager.OnPauseIsReleased += OnPauseReleased;
    }


    void UpdateRemainingVirusCount()
    {
        // Decrease count
        virusRemainingCount--;

        // Disable the correct image if still within bounds
        if (virusRemainingCount >= 0 && virusRemainingCount < virusImageRemaining.Length)
        {
            virusImageRemaining[virusRemainingCount].SetActive(false);
        }
    }

    public void OnQuitClicked()
    {
        Time.timeScale = 1f;
        pauseManager.IsPaused = false;
        //pauseManager.settingsPanel.SetActive(false);
        AudioListener.pause = false;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
        SceneManager.LoadScene("MainMenu");
    }

    void OnPauseClicked()
    {
        SettingsUI.SetActive(true);
    }

    void OnPauseReleased()
    {
        SettingsUI.SetActive(false);
    }
}

    //public void OnSettingsIfPressed()
    //{
    //    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    //    {
    //        OnSettingsPressed?.Invoke();
    //        Time.timeScale = 0;
    //        settingsPanel.SetActive(true);
    //    }
            
    //}
//    public void OnSettingsExitClicked()
//    {
//        OnSettingsReleased?.Invoke();
//        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
//        settingsPanel.SetActive(false);
//        Time.timeScale = 1;
//    }
//}

//using TMPro;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class GameUI : MonoBehaviour
//{
//    public FoodPopulationSystem foodPopulationSystem;
//    public PlayerInteraction playerInteraction;

//    public GameObject[] virusImageRemaining;
//    public TextMeshProUGUI virusRemainingText;


//    public int virusRemainingCount = 0;
//    bool OnCured;

//    private void Start()
//    {
//        virusRemainingCount = foodPopulationSystem.normalInfectedFoodCount;
//        playerInteraction.OnCuredInfection += () => UpdateRemainingVirusCount();
//    }

//    private void Update()
//    {
//        UpdateVirusStatus();
//    }

//    void UpdateVirusStatus()
//    {
//        virusRemainingText.text = ": " + virusRemainingCount.ToString();

//        if(OnCured)
//            virusImageRemaining[virusRemainingCount - 1].SetActive(false);
//        OnCured = false;
//    }


//    void UpdateRemainingVirusCount()
//    {
//        virusRemainingCount -= 1;
//        OnCured = true;
//    }

//    public void OnQuitClicked()
//    {
//        //AudioManager.Instance.StopSound("music");
//        AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSound);
//        SceneManager.LoadScene("MainMenu");
//    }
//}
