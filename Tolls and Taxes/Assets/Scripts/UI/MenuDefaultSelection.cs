using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuDefaultSelection : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private void OnEnable()
    {
        StartCoroutine(SelectNextFrame());
    }

    private IEnumerator SelectNextFrame()
    {
        yield return null;

        if (EventSystem.current == null ||
            firstSelected == null)
        {
            yield break;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}