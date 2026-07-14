using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public static InteractableObject CurrentInteractable { get; private set; }

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PartyLeader player = collision.GetComponentInParent<PartyLeader>();

        if (player == null)
        {
            return;
        }

        // Stores the object currently within the player's interaction range.
        CurrentInteractable = this;
        Debug.Log("Player entered interaction range.");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PartyLeader player = collision.GetComponentInParent<PartyLeader>();

        if (player == null)
        {
            return;
        }

        // Only clear the reference if the player is leaving this object.
        if (CurrentInteractable == this)
        {
            CurrentInteractable = null;
        }

        Debug.Log("Player left interaction range.");
    }

    public void Interact()
    {
        Debug.Log("The object was interacted with.");
    }
}