using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenetrigger : MonoBehaviour
{
    public Collider2D InteractableCollider;

    public void Start()
    {
        InteractableCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene("Prototype Combat");
    }
}
