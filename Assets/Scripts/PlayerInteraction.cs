using System;
using System.Collections;
using System.Collections.Generic;
using TMPro; // for progress text
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Game UI")]
    [Space(5)]
    public GameObject gameLoseCanvas;
    public TextMeshProUGUI cureProgressText; // UI text for curing progress
    public GameObject particles;

    [Header("Cameras")]
    [Space(5)]
    public GameObject firstPOVCamera;
    public GameObject deathCamera;

    [Header("Enemy")]
    [Space(5)]
    public Enemy enemy;
    private CharacterController controller;

    [Header("Food System")]
    [Space(5)]
    public GameObject foodLocation;
    public List<GameObject> infectedFood = new List<GameObject>();
    Renderer infectedFoodColor; // current virus touched

    bool isInfected = false;
    public bool areAllCured = false;

    public event Action OnCuredAll;
    public event Action OnCuredInfection;

    [Header("Portal System")]
    [Space(5)]
    public GameObject portal1;
    public GameObject portal2;

    int curedInfectionCount = 0;
    public bool isPlayerDead;

    // Cure timing
    [Header("Cure Settings")]
    public bool isCuring = false;
    float requiredHoldTime = 1f; // seconds required
    Coroutine cureRoutine;
    public GameObject canBeDecodedNotif;

    public event Action PlayerLooking;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        enemy.PlayerAttacked += OnPlayerDeath;
        ScanAndAddInfectedFood();

        if (cureProgressText != null)
            cureProgressText.text = ""; // clear at start
    }

    void OnEnable()
    {
        if (enemy != null)
        {
            enemy.BeforeAttack += OnPlayerLookingAtEnemy; // subscribe correctly
        }
    }

    void OnDisable()
    {
        if (enemy != null)
        {
            enemy.BeforeAttack -= OnPlayerLookingAtEnemy; // unsubscribe
        }
    }


    private void OnPlayerLookingAtEnemy(Transform enemyTransform)
    {
        PlayerLooking?.Invoke();
        StartCoroutine(RotateTowardsEnemy(enemyTransform, .5f));
    }

    private void OnPlayerDeath()
    {
        //StartCoroutine(RotateTowardsEnemy(enemy.transform, 1f));
        isPlayerDead = true;

        //if (!deathCamera.activeInHierarchy)
        //{
        //    firstPOVCamera.SetActive(false);
        //    deathCamera.SetActive(true);
        //}
        //else if (!firstPOVCamera.activeInHierarchy)
        //{
        //    firstPOVCamera.SetActive(true);
        //    deathCamera.SetActive(false);
        //}

        gameLoseCanvas.SetActive(true);
        Debug.Log("Player has died!");
    }

    IEnumerator RotateTowardsEnemy(Transform enemyTransform, float duration)
    {
        //AudioManager.Instance.PlaySFX(AudioManager.Instance.caughtSound);

        Quaternion startRotation = transform.rotation;
        Vector3 direction = enemyTransform.position - transform.position;
        direction.y = 0f; // keep rotation horizontal
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Ensure final rotation is exact
        transform.rotation = targetRotation;
    }


    private void Update()
    {
        if (!areAllCured)
        {
            InjectingAntidote();
            AreAllAntidotesInjected();
        }
    }

    void ScanAndAddInfectedFood()
    {
        foreach (Transform child in foodLocation.transform)
        {
            if (child.CompareTag("Virus"))
            {
                infectedFood.Add(child.gameObject);
            }
        }
    }

    void InjectingAntidote()
    {
        if (isInfected && !areAllCured && Keyboard.current.eKey.isPressed)
        {
            if (infectedFoodColor != null && cureRoutine == null)
            {
                // Start fade animation with progress text
                cureRoutine = StartCoroutine(CureFade(infectedFoodColor));
                isCuring = true;
            }
        }

        // Cancel if player releases early
        if (Keyboard.current.eKey.wasReleasedThisFrame && cureRoutine != null)
        {
            AudioManager.Instance.StopSound("sfx");
            StopCoroutine(cureRoutine);
            cureRoutine = null;
            infectedFoodColor.material.color = Color.black; // reset to infected
            if (cureProgressText != null)
                cureProgressText.text = "Debugging interrupted ";
            Debug.Log("Cure interrupted — player released key too early.");

            isCuring = false;
        }
        cureProgressText.text = "";
    }

    IEnumerator CureFade(Renderer foodRenderer)
    {
        Color startColor = Color.black;
        Color endColor = Color.white;
        float duration = requiredHoldTime;
        float elapsed = 0f;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.decodingSound);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Update color
            foodRenderer.material.color = Color.Lerp(startColor, endColor, t);

            // Update text indicator (0–100%)
            if (cureProgressText != null)
            {
                int percent = Mathf.Clamp(Mathf.RoundToInt(t * 100f), 0, 100);
                cureProgressText.text = $"Debugging... {percent}%";
            }

            yield return null;
        }

        // Final cure state
        foodRenderer.material.color = endColor;
        foodRenderer.material.EnableKeyword("_EMISSION");

        // Get all colliders attached to the foodRenderer GameObject
        Collider[] colliders = foodRenderer.GetComponents<Collider>();

        // Find the one that is NOT a trigger and disable it
        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)   // only disable the solid collider
            {
                col.enabled = false;
                break; // stop after disabling the first non-trigger
            }
        }

        //foodRenderer.GetComponent<Collider>().enabled = false;
        Instantiate(particles, foodRenderer.transform.position, particles.transform.rotation);
        foodRenderer.gameObject.tag = "Cured";
        curedInfectionCount++;
            
        if (cureProgressText != null)
            cureProgressText.text = "Debugged! ";

        Debug.Log("Debug completed with smooth fade!");
        OnCuredInfection?.Invoke();
        isInfected = false;
        infectedFoodColor = null;
        cureRoutine = null;

        canBeDecodedNotif.SetActive(false);
        foodRenderer.transform.GetChild(0).gameObject.SetActive(false); // disable virus visual

        GameManager.Instance.PelletCured();

        isCuring = false;
    }

    void AreAllAntidotesInjected()
    {
        if (curedInfectionCount >= infectedFood.Count)
        {
            areAllCured = true;
            Debug.Log("Game Finished! All infected food items have been debugged!");
           OnCuredAll ?.Invoke();

            curedInfectionCount = 0; // reset if needed for replay
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            canBeDecodedNotif.SetActive(true);
            isInfected = true;
            infectedFoodColor = other.GetComponent<Renderer>();

            Debug.Log($"Player touched infected food: {other.name}");
        }

        if (other.CompareTag("Portal1"))
        {
            Debug.Log("Player has entered portal 1!");
            controller.enabled = false;
            transform.position = portal2.transform.position + new Vector3(2, 0, 0);
            controller.enabled = true;
        }

        if (other.CompareTag("Portal2"))
        {
            Debug.Log("Player has entered portal 2!");
            controller.enabled = false;
            transform.position = portal1.transform.position + new Vector3(-2, 0, 0);
            controller.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            canBeDecodedNotif.SetActive(false);
            isInfected = false;
            Debug.Log("Player exited infected food area.");
        }
    }
}
