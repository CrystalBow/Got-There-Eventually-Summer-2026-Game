using UnityEngine;

public class Door : InteractableObject
{
    protected override void Interact()
    {
        if (BodyCollider.isActiveAndEnabled == true)
        {
            BodyCollider.enabled = false;
            SpriteRenderer.enabled = false;
        }
        else
        {
            BodyCollider.enabled = true;
            SpriteRenderer.enabled = true;
        }
    }
}
