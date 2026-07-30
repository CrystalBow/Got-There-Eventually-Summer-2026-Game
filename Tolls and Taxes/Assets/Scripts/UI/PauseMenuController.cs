using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject partyDetailsPanel;

    [Header("Keyboard Navigation")]
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject controlsBackButton;
    [SerializeField] private GameObject partyDetailsFirstButton;

    [SerializeField] private string startSceneName = "StartMenu";

    public static Action PauseGameAction;
    public static Action ResumeGameAction;

    private bool isPaused;
    private Coroutine selectionRoutine;

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

        ControlReminderUI.Instance?.Show(
            ControlReminderContext.Menu);

        SetActive(darkOverlay, true);
        SetActive(pausePanel, true);
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, false);

        SelectForKeyboard(resumeButton);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        ControlReminderUI.Instance?.Show(
            ControlReminderContext.Exploration);

        SetActive(darkOverlay, false);
        SetActive(pausePanel, false);
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, false);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ShowControls()
    {
        SetActive(pausePanel, false);
        SetActive(controlsPanel, true);
        SetActive(partyDetailsPanel, false);

        SelectForKeyboard(controlsBackButton);
    }

    public void ShowPartyDetails()
    {
        SetActive(pausePanel, false);
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, true);

        SelectForKeyboard(partyDetailsFirstButton);
    }

    public void ShowPauseMenu()
    {
        SetActive(controlsPanel, false);
        SetActive(partyDetailsPanel, false);
        SetActive(pausePanel, true);

        SelectForKeyboard(resumeButton);
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

    private void SelectForKeyboard(GameObject target)
    {
        if (target == null || EventSystem.current == null)
        {
            return;
        }

        if (selectionRoutine != null)
        {
            StopCoroutine(selectionRoutine);
        }

        selectionRoutine = StartCoroutine(
            SelectOnNextFrame(target));
    }

    private IEnumerator SelectOnNextFrame(GameObject target)
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);

        selectionRoutine = null;
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