using Unity.VisualScripting;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    protected CircleCollider2D InteractableCollider;
    protected bool IsInteractable = false;
    protected Collider2D BodyCollider;
    protected SpriteRenderer SpriteRenderer;
    [SerializeField] public float radius = 0.5f;
    [SerializeField] public Vector2 center = Vector2.zero;

    public virtual void Start()
    {
        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (col is CircleCollider2D)
            {
                InteractableCollider = col as CircleCollider2D;
            }
            else
            {
                BodyCollider = col;
            }
        }
        SpriteRenderer = GetComponent<SpriteRenderer>();
        InteractableCollider.isTrigger = true;
        InteractableCollider.radius = this.radius;
        InteractableCollider.offset = this.center;
        PartyLeader.Interaction += PartyLeaderOnInteraction;
        
    }

    public virtual void Update()
    {
        
    }

    protected void PartyLeaderOnInteraction(Vector2 obj)
    {
        if (InteractableCollider.OverlapPoint(obj))
        {
            Interact();
        }
    }
    
    protected virtual void Interact()
    {
        Debug.Log("The object was interacted with.");
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position + new Vector3(center.x, center.y, 0), this.radius);
    }
}