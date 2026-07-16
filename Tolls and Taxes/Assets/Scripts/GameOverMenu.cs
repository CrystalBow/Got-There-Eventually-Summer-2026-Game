using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private string startSceneName = "StartMenu";

    public void ReturnToStart()
    {
        // Ensures the next scene is not left paused.
        Time.timeScale = 1f;

        SceneManager.LoadScene("Prototype Start");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        // Stops Play Mode while testing inside Unity.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Closes the finished game application.
        Application.Quit();
#endif
    }
}