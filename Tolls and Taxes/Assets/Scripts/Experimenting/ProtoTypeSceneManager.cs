using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProtoTypeSceneManager : MonoBehaviour
{
    public static ProtoTypeSceneManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameObject explorationRoot; // Drag your _ExplorationRoot here
    [SerializeField] private CanvasGroup fadeScreen;     // Simple UI Panel with a CanvasGroup for fading

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Call this to smoothly transition from Exploration to Combat
    /// </summary>
    public void EnterCombat(string combatSceneName)
    {
        StartCoroutine(TransitionToCombat(combatSceneName));
    }

    /// <summary>
    /// Call this when combat is over to return to Exploration
    /// </summary>
    public void ExitCombat(string combatSceneName)
    {
        StartCoroutine(TransitionToExploration(combatSceneName));
    }

    private IEnumerator TransitionToCombat(string combatSceneName)
    {
        // 1. Fade screen to black
        yield return StartCoroutine(Fade(1f));

        // 2. Load Combat Scene ADDITIVELY
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(combatSceneName, LoadSceneMode.Additive);
        while (!loadScene.isDone)
        {
            yield return null; // Wait until loading is fully complete
        }

        // 3. Pause/Deactivate Exploration logic to save RAM and CPU
        explorationRoot.SetActive(false);

        // 4. Fade back to clear (revealing the combat scene)
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator TransitionToExploration(string combatSceneName)
    {
        // 1. Fade screen to black
        yield return StartCoroutine(Fade(1f));

        // 2. Reactivate Exploration scene (restores player position, states, and tiles instantly)
        explorationRoot.SetActive(true);

        // 3. Unload the Combat Scene to free up memory
        AsyncOperation unloadScene = SceneManager.UnloadSceneAsync(combatSceneName);
        while (!unloadScene.isDone)
        {
            yield return null;
        }

        // 4. Fade back to clear
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeScreen == null) yield break;

        float speed = 2f; // Adjust to speed up/slow down fade
        while (!Mathf.Approximately(fadeScreen.alpha, targetAlpha))
        {
            fadeScreen.alpha = Mathf.MoveTowards(fadeScreen.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }
    }
}