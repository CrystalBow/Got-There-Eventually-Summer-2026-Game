using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject partyDetailsPanel;
    [SerializeField] private string startSceneName = "Prototype Start";
    public static Action PauseGameAction;
    public static Action ResumeGameAction;

    private bool isPaused;

    private void Awake()
    {
        Time.timeScale = 1f;

        SetActive(darkOverlay, false);
        SetActive(pausePanel, false);
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, false);
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (IsActive(controlsPanel) || IsActive(partyDetailsPanel))
        {
            ShowPauseMenu();
        }
        else if (isPaused)
        {
            ResumeGame();
            ResumeGameAction?.Invoke();
        }
        else
        {
            PauseGameAction?.Invoke();
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        SetActive(darkOverlay, true);
        SetActive(pausePanel, true);
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        SetActive(darkOverlay, false);
        SetActive(pausePanel, false);
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, false);
    }

    public void ShowControls()
    {
        SetActive(pausePanel, false);
        SetActive(controlsPanel, true);
        SetActive(partyDetailsPanel, false);
    }

    public void ShowPartyDetails()
    {
        SetActive(pausePanel, false);
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, true);
    }

    public void ShowPauseMenu()
    {
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, false);
        SetActive(pausePanel, true);
    }

    public void ReturnToStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneName);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private static bool IsActive(GameObject target)
    {
        return target != null && target.activeSelf;
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}