using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    public Animator animator;

    [Header("Movement Parameters")]
    public float maxSpeed = 3.5f;
    public float acceleration = 15f;

    public Vector3 CurrentVelocity { get; private set; }
    public float CurrentSpeed { get; private set; }

    [Header("Look Parameters")]
    public Vector2 LookSensitivity = new Vector2(.1f, .1f);
    public float PitchLimit = 85f;

    [SerializeField] float currentPitch = 0f;
    public float CurrentPitch
    {
        get => currentPitch;
        set => currentPitch = Mathf.Clamp(value, -PitchLimit, PitchLimit);
    }

    [Header("Input")]
    public Vector2 MoveInput;
    public Vector2 LookInput;

    [Header("Components")]
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerObject;
    [SerializeField] PlayerInteraction playerInteraction;
    [SerializeField] GameManager gameManager;
    [SerializeField] PauseManager pauseManager;

    Transform playerObjectTransform;

    [Space(10)]
    public Enemy enemy;
    bool isPlayerDead;

    // Jump & Run
    [Header("Jump & Run Parameters")]
    public GameObject runBoost;
    public GameObject runBoostCD;
    public TextMeshProUGUI runBoostCooldownText;
    public float jumpHeight = .5f;          // jump height
    public float runMultiplier = 1.3f;       // run speed multiplier
    public float runDuration = .1f;         // run lasts 1 second
    public float runCooldown = 5f;         // cooldown between runs

    private bool isGrounded;
    private float verticalVelocity;
    private bool isRunning;
    private float runTimer;
    private float runCooldownTimer;

    bool onPause = false;

    private void Start()
    {
        maxSpeed = GameManager.Instance.basePlayerSpeed; // initial speed
        pauseManager.OnPauseIsPressed += OnPausePressed;
        pauseManager.OnPauseIsReleased += OnPauseReleased;

        playerInteraction = GetComponent<PlayerInteraction>();
        playerObjectTransform = playerObject.transform;

        enemy.PlayerAttacked += () =>
        {
            isPlayerDead = true;
        };

        playerInteraction.PlayerLooking += () =>
        {
            isPlayerDead = true;
        };
    }

    #region Unity Methods
    private void OnValidate()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (gameManager.gameStarted)
        {
            if (!onPause)
            {
                ReadInputs();
                LookUpdate();

                if (playerInteraction.isCuring)
                    return;


                float speed = 0f;
                if (Keyboard.current.eKey.isPressed)
                {
                    speed = 0;
                }
                else
                {
                    speed = MoveInput.magnitude > 0 ? 1f : 0f;
                }
                animator.SetFloat("Speed", speed);

                if (Keyboard.current.shiftKey.wasPressedThisFrame)
                {
                    RunBoost();
                }
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    Jump();
                }

                MoveUpdate();


                // Cooldown countdown
                if (runCooldownTimer > 0f)
                {
                    runBoost.SetActive(false);
                    runBoostCD.SetActive(true);
                    runCooldownTimer -= Time.deltaTime;
                }
                else
                {
                    runBoostCD.SetActive(false);
                    runBoost.SetActive(true);
                }
            }


        }
    }

    void OnPausePressed()
    {
        onPause = true;
    }

    void OnPauseReleased()
    {
        onPause = false;
    }
    void ReadInputs()
    {
        if (isPlayerDead)
        {
            MoveInput = Vector2.zero;
            LookInput = Vector2.zero;
            return;
        }


    }
    #endregion

    #region Controller Methods

    void MoveUpdate()
    {
        // Ground check
        isGrounded = controller.isGrounded;

        Vector3 motion = transform.forward * MoveInput.y + transform.right * MoveInput.x;
        motion.y = 0;
        motion.Normalize();

        float targetSpeed = maxSpeed;


        // Run boost
        if (isRunning)
        {
            targetSpeed *= runMultiplier;
            runTimer -= Time.deltaTime;
            if (runTimer <= 0f)
                isRunning = false;
        }

        if (motion.sqrMagnitude >= 0.01f)
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, motion * targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, acceleration * Time.deltaTime);
        }

        // Gravity & Jump
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -1f; // small downward force to keep grounded
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        Vector3 fullVelocity = new Vector3(CurrentVelocity.x, verticalVelocity, CurrentVelocity.z);
        controller.Move(fullVelocity * Time.deltaTime);

        CurrentSpeed = CurrentVelocity.magnitude;


        // Update cooldown text
        if (runCooldownTimer > 0f)
            runBoostCooldownText.text = runCooldownTimer.ToString("F1");
        else
            runBoostCooldownText.text = " ";
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(LookInput.x * LookSensitivity.x, LookInput.y * LookSensitivity.y);

        // Looking left and right
        transform.Rotate(Vector3.up * input.x);
        playerObjectTransform.Rotate(Vector3.up * input.x);

        // Looking up and down
        CurrentPitch -= input.y;
        fpCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0, 0);
    }

    #endregion

    #region Actions

    // Jump function
    public void Jump()
    {
        if (isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            Debug.Log("Jump Height: " + jumpHeight + " | Velocity: " + verticalVelocity);

        }
    }

    // Run boost function
    public void RunBoost()
    {
        if (runCooldownTimer <= 0f) // only allow if cooldown finished
        {
            isRunning = true;
            runTimer = runDuration;
            runCooldownTimer = runCooldown; // reset cooldown
        }
        else
        {
            Debug.Log("Run boost on cooldown! " + runCooldownTimer.ToString("F1") + "s left.");
        }

    }

    #endregion
}