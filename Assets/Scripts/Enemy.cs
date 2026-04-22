using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Debugger")]
    public bool DisablePacman = false;
    private AudioSource audioSource;

    public enum State { Idle, Chasing, Attacking }
    State currentState;

    [SerializeField] PauseManager pauseManager;
    [SerializeField] GameManager gameManager;
    PlayerInteraction player;
    public NavMeshAgent pathfinder;

    Transform target;

    public float attackDistanceThreshold = 3.2f;
    public float timeBetweenAttacks = 3f;

    public bool attacking = false;

    public event Action PlayerAttacked;
    public event Action<Transform> BeforeAttack; // notify player before attack

    bool hasTarget;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    private bool wasPathfinderEnabled = false;




    private void Awake()
    {
        
        pathfinder = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = false;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true; // Set hasTarget to true since the player is present in the scene.

            target = GameObject.FindGameObjectWithTag("Player").transform;
            Transform child = target.Find("Capsule");
            player = target.GetComponent<PlayerInteraction>();

            myCollisionRadius = GetComponent<SphereCollider>().radius; // Get the radius of the enemy's collider.
            targetCollisionRadius = child.GetComponent<CapsuleCollider>().radius; // Get the radius of the target's collider.
        }
    }

    void Start()
    {
        pauseManager.OnPauseIsPressed += Pause;
        pauseManager.OnPauseIsReleased += Resume;
        gameManager.gameHasStarted += GameStarted;

        if (audioSource != null && AudioManager.Instance != null)
        {
            audioSource.clip = AudioManager.Instance.pacmanWaka;
            audioSource.loop = true;
            //audioSource.Play();

        }
        if (hasTarget)
        {
            //AudioManager.Instance.PlayPacmanSound(hasTarget);

            Debug.Log("Player found, starting chase.");
            currentState = State.Chasing;
            //player.OnDeath += OnPlayerDeath;
            pathfinder.speed = GameManager.Instance.baseEnemySpeed; // initial speed

            StartCoroutine(UpdatePath());
        }

    }

    void GameStarted()
    {
        audioSource.Play();
    }

    void Pause()
    {
        // Stop movement
        wasPathfinderEnabled = pathfinder.enabled;
        if (pathfinder.enabled)
        {
            pathfinder.isStopped = true;
            pathfinder.ResetPath();
        }

        // Pause audio globally
        AudioListener.pause = true;

        // Stop audio
        //if (audioSource != null && audioSource.isPlaying)
        //    audioSource.Pause();

        // Stop all coroutines
        StopAllCoroutines();

    }
    void Resume()
    {

        // Resume movement
        if (wasPathfinderEnabled)
        {
            pathfinder.enabled = true;
            pathfinder.isStopped = false;
            StartCoroutine(UpdatePath());
        }

        // Resume audio globally
        AudioListener.pause = false;

        // Resume audio
        //if (audioSource != null)
        //    audioSource.UnPause();    

    }

    void Update()
    {
        if (DisablePacman)
        {
            wasPathfinderEnabled = pathfinder.enabled;
            if (pathfinder.enabled)
            {
                pathfinder.isStopped = true;
                pathfinder.ResetPath();
            }
            AudioListener.pause = true;
            // Stop all coroutines
            StopAllCoroutines();
            return;
        }
        if (attacking)
        {
            StopCoroutine(UpdatePath());
        }

        CheckDistanceForAttack();
        CheckIfPlayerWin();
    }

    void CheckIfPlayerWin()
    {
        if (player.areAllCured)
        {
            hasTarget = false;
            currentState = State.Idle;
        }
    }

    void CheckDistanceForAttack()
    {
        bool oneTime = true;
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2) && oneTime) // && !attacking && !player.isDead && !player.areAllCured
                {
                    audioSource.Stop();
                    attacking = true;
                    nextAttackTime = Time.time + timeBetweenAttacks; // Set the next attack time to the current time plus the time between attacks.
                    //AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.caughtSound);
                    AudioManager.Instance.StopSound("music");
                    BeforeAttack?.Invoke(transform); // Notify subscribers that an attack is about to happen.  
                    StartCoroutine(Attack()); // Start the attack coroutine.

                    Debug.Log("Attack!");
                    oneTime = false;
                }
            }
        }
    }

    IEnumerator Attack()
    {
        // Clean stop so the agent doesn't keep pushing forward
        pathfinder.isStopped = true;
        pathfinder.ResetPath();
        pathfinder.velocity = Vector3.zero;

        yield return new WaitForSeconds(1f);

        currentState = State.Attacking; // Change the state to Attacking.
        pathfinder.enabled = false; // Disable the NavMeshAgent to stop pathfinding while attacking.

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;

        // Use sum of radii + buffer to avoid overlap
        float buffer = 0.15f;
        float standOff = myCollisionRadius + targetCollisionRadius + buffer;
        Vector3 targetPosition = target.position - dirToTarget * standOff;

        //Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0; // Percentage of the attack animation completed.

        //skinMaterial.color = Color.red; // Change the enemy's material color to red to indicate an attack is in progress.
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage) // Check if the attack animation is halfway through and damage has not been applied yet.
            {
                hasAppliedDamage = true;
                PlayerAttacked?.Invoke();
                hasTarget = false;

                //AudioManager.Instance.StopSound("pacman");
                currentState = State.Idle;
                //player.TakeDamage(damage); // Apply damage to the target entity (the player) when the attack animation reaches halfway.
            }
            percent += Time.deltaTime * attackSpeed; // Increment the percentage based on the time elapsed and attack speed.
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; // Calculate the interpolation value for the attack animation.
            transform.position = Vector3.Lerp(originalPosition, targetPosition, interpolation); // Move the enemy towards the target position using linear interpolation.

            yield return null; // Wait for the next frame.
        }

        //skinMaterial.color = originalColor; // Reset the enemy's material color to its original color after the attack animation is complete.
        //pathfinder.enabled = true; // Re-enable the NavMeshAgent after the attack animation is complete.
        //currentState = State.Idle; // Change the state back to Chasing after the attack is done.
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                if (pathfinder.enabled)
                {
                    pathfinder.SetDestination(target.position);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

}
