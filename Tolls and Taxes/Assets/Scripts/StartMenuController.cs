using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "SampleScene";

    public void StartGame()
    {
        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogError("Gameplay scene name has not been assigned.");
            return;
        }

        // Make sure gameplay is not accidentally left paused.
        Time.timeScale = 1f;

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        // Stops Play Mode when testing inside Unity.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Closes the finished game build.
        Application.Quit();
#endif
    }
}