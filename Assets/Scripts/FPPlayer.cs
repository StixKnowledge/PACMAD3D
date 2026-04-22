using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class FPPlayer : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] FPController controller;
    [SerializeField] Enemy enemy;
    //[SerializeField] GameUI gameUI;
    [SerializeField] PauseManager pauseManager;

    #region Input Handling

    void OnMove(InputValue value)
    {
        controller.MoveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        controller.LookInput = value.Get<Vector2>();
    }
    #endregion


    #region Unity Methods

    private void OnValidate()
    {
        if (controller == null)
            controller = GetComponent<FPController>();
    }

    private void Start()
    {
        //if player is alive, hide and lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        enemy.PlayerAttacked += () =>
        {
            //if player is dead, show and unlock cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        };
        pauseManager.OnPauseIsPressed+= () =>
        {
            //if player is dead, show and unlock cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        };
        pauseManager.OnPauseIsReleased+= () =>
        {
            //if player is dead, show and unlock cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        };

        //else, show and unlock cursor
    }
    #endregion
}


