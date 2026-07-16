using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    private string gameplaySceneName = "Prototype Exploration";

    public void StartGame()
    {
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