using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private string startSceneName = "Prototype Start";

    private bool isPaused;

    private void Awake()
    {
        Time.timeScale = 1f;

        darkOverlay.SetActive(false);
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (controlsPanel.activeSelf)
        {
            ShowPauseMenu();
        }
        else if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        darkOverlay.SetActive(true);
        pausePanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        darkOverlay.SetActive(false);
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ShowControls()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        controlsPanel.SetActive(false);
        pausePanel.SetActive(true);
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

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}