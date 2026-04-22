using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public FPController player;
    public PlayerInteraction playerInteraction;
    public Enemy enemy;
    public FoodPopulationSystem foodPopulationSystem;
    public GameObject instructionUI;
    public GameObject gameStartedFlag;
    //public GameObject gameWinCanvas;
    public AsyncLoader loader;

    public Renderer wallGlowRend;

    public static GameManager Instance;

    public int totalPellets = 0;
    public int curedPellets = 0;

    public float basePlayerSpeed;
    public float baseEnemySpeed;

    public bool gameStarted = false;
    public event Action gameHasStarted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Time.timeScale = 0f;
    }

    private void Start()
    {
        totalPellets = foodPopulationSystem.normalInfectedFoodCount;
        basePlayerSpeed = player.maxSpeed;
        baseEnemySpeed = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().speed;
        playerInteraction.OnCuredAll += ShowGameFinalStory;

    }

    void ShowGameFinalStory()
    {
        loader.LoadLevenBtnGame("GameWinStory");
    }

    void Update()
    {
        PressedAnyKeyToStart();
    }

    void PressedAnyKeyToStart()
    {
        if (Time.timeScale == 0f && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            gameHasStarted?.Invoke();
            //AudioManager.Instance.PlayMusic(AudioManager.Instance.gameThemeSound);
            gameStarted = true;
            StartCoroutine(showGameStarted());
            Time.timeScale = 1f;
            instructionUI.SetActive(false);
        }
    }

    IEnumerator showGameStarted()
    {
        gameStartedFlag.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameStartedFlag.SetActive(false);
    }

    public void PelletCured()
    {
        curedPellets++;
        UpdateDifficulty();
    }

    

    void UpdateDifficulty()
    {
        float progress = (float)curedPellets / totalPellets;

        if (progress < 0.3f)
        {
            // Phase 1
            player.maxSpeed = 3.5f;
            enemy.pathfinder.speed = 3.0f;

        }
        else if (progress < 0.6f)
        {
            // Phase 2
            player.maxSpeed = 3.2f;
            enemy.pathfinder.speed = 3.5f;
            wallGlowRend.material.SetColor("_EmissionColor", new Color(0.93f, 0.51f, 0.93f));
            AudioManager.Instance.PlayMusic(AudioManager.Instance.pantingSound);

        }
        else if (progress < 0.9f)
        {
            // Phase 3
            player.maxSpeed = 2.8f;
            enemy.pathfinder.speed = 4.0f;
            wallGlowRend.material.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f));

        }
        else
        {
            // Phase 4
            player.maxSpeed = 2.5f;
            enemy.pathfinder.speed = 4.5f;
            wallGlowRend.material.SetColor("_EmissionColor", new Color(1f, 0f, 0f));
        }

        Debug.Log($"Difficulty Updated: PlayerSpeed={player.maxSpeed}, EnemySpeed={enemy.pathfinder.speed}");
    }

    //void UpdateDifficulty()
    //{
    //    float progress = (float)curedPellets / totalPellets;

    //    // Player slows slightly as progress increases
    //    float newPlayerSpeed = basePlayerSpeed * (1 - 0.3f * progress);

    //    // Enemy speeds up as progress increases
    //    float newEnemySpeed = baseEnemySpeed * (1 + 0.5f * progress);

    //    if (player != null) player.maxSpeed = newPlayerSpeed;
    //    if (enemy != null) enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = newEnemySpeed;

    //    Debug.Log($"Difficulty Updated: PlayerSpeed={newPlayerSpeed}, EnemySpeed={newEnemySpeed}");
    //}
}