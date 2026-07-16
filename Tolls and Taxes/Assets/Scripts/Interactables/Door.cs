using UnityEngine;

public class Door : InteractableObject
{
    protected override void Interact()
    {
        if (BodyCollider.isActiveAndEnabled == true)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //BodyCollider.enabled = true;
            //SpriteRenderer.enabled = true;
        }
    }
}
