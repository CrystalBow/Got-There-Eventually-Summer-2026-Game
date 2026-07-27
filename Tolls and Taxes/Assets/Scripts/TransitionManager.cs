using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatTransitionManager : MonoBehaviour
{
    public static CombatTransitionManager Instance { get; private set; }

    [Header("Scene Settings")]
    [SerializeField] private string combatSceneName = "Yimer's Workshop Combat";
    
    [Header("Positioning")]
    [Tooltip("Distance offset to place the combat scene root objects in world space.")]
    [SerializeField] private Vector3 combatSceneOffset = new Vector3(5000f, 5000f, 5000f);

    private Scene mainWorldScene;
    private Scene combatScene;
    private List<GameObject> disabledMainWorldRoots = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartCombat()
    {
        StartCoroutine(LoadCombatRoutine());
    }

    public void EndCombat()
    {
        StartCoroutine(UnloadCombatRoutine());
    }

    private IEnumerator LoadCombatRoutine()
    {
        // 1. Store main scene reference
        mainWorldScene = SceneManager.GetActiveScene();

        // 2. Load combat scene additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(combatSceneName, LoadSceneMode.Additive);
        yield return loadOp;

        combatScene = SceneManager.GetSceneByName(combatSceneName);
        SceneManager.SetActiveScene(combatScene);

        // 3. Offset the root objects of the loaded combat scene
        OffsetCombatSceneRoots(combatScene, combatSceneOffset);

        // 4. Temporarily disable main world roots
        DisableSceneRootObjects(mainWorldScene);
    }

    private IEnumerator UnloadCombatRoutine()
    {
        // 1. Re-enable main world roots BEFORE unloading combat
        EnableSceneRootObjects();

        // 2. Set main world back as active scene
        SceneManager.SetActiveScene(mainWorldScene);

        // 3. Unload combat scene
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(combatSceneName);
        yield return unloadOp;
    }

    private void OffsetCombatSceneRoots(Scene scene, Vector3 offset)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject go in rootObjects)
        {
            // Shift each root object by the offset
            go.transform.position += offset;
        }
    }

    private void DisableSceneRootObjects(Scene sceneToDisable)
    {
        disabledMainWorldRoots.Clear();
        GameObject[] rootObjects = sceneToDisable.GetRootGameObjects();

        foreach (GameObject go in rootObjects)
        {
            if (go.activeSelf)
            {
                disabledMainWorldRoots.Add(go);
                go.SetActive(false);
            }
        }
    }

    private void EnableSceneRootObjects()
    {
        foreach (GameObject go in disabledMainWorldRoots)
        {
            if (go != null)
            {
                go.SetActive(true);
            }
        }
        disabledMainWorldRoots.Clear();
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeScene()
    {
        GameObject g = new GameObject("Generated_Transition_Manager");
        g.AddComponent<CombatTransitionManager>();
    }
}