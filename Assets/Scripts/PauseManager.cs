using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public bool IsPaused;

    //[SerializeField] public GameObject settingsPanel;

    public event Action OnPauseIsPressed;
    public event Action OnPauseIsReleased;

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        OnPauseIsPressed?.Invoke(); 
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        OnPauseIsReleased?.Invoke();
        Time.timeScale = 1f;
        IsPaused = false;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }
}