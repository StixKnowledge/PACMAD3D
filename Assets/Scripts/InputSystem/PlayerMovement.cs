using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction m_moveAction;
    private InputAction m_lookAction;
    private InputAction m_jumpAction;

    private Vector2 m_lookAmt;
    private Vector2 m_moveAmt;
    private Animator m_animator;
    private Rigidbody m_rigidbody;

    public float MoveSpeed = 5f;
    public float RotateSpeed = 5f;
    public float JumpForce = 5f;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }
    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_lookAction = InputSystem.actions.FindAction("Look");
        m_jumpAction = InputSystem.actions.FindAction("Jump"); 

        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = m_lookAction.ReadValue<Vector2>();

        if(m_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }

    public void Jump()
    {
        m_rigidbody.AddForceAtPosition(new Vector3(0, 5f,0),Vector3.up, ForceMode.Impulse);
        m_animator.SetTrigger("Jump");
    }

    private void FixedUpdate()
    {
        Walking();
        Rotating();
    }

    

    private void Walking()
    {
        m_animator.SetFloat("Speed", m_moveAmt.y);
        m_rigidbody.MovePosition(transform.position + transform.forward * m_moveAmt.y * MoveSpeed * Time.fixedDeltaTime);
    }

    private void Rotating()
    {
        if(m_moveAmt.y != 0)
        {
            float rotationAmount = m_lookAmt.x * RotateSpeed * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0, rotationAmount, 0);
            m_rigidbody.MoveRotation(m_rigidbody.rotation * deltaRotation);
        }

    }
}
