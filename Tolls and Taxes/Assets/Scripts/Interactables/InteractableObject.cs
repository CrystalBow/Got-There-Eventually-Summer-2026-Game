using Unity.VisualScripting;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    protected CircleCollider2D InteractableCollider;
    protected bool IsInteractable = false;
    protected Collider BodyCollider;
    [SerializeField] public float radius = 0.5f;
    [SerializeField] public Vector2 center = Vector2.zero;

    private void Start()
    {
        this.InteractableCollider = this.GetComponent<CircleCollider2D>();
        InteractableCollider.isTrigger = true;
        InteractableCollider.radius = this.radius;
        InteractableCollider.offset = this.center;
        PartyLeader.Interaction += PartyLeaderOnInteraction;
        
    }

    private void PartyLeaderOnInteraction(Vector2 obj)
    {
        if (InteractableCollider.OverlapPoint(obj))
        {
            Interact();
        }
    }
    
    public void Interact()
    {
        Debug.Log("The object was interacted with.");
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position + new Vector3(center.x, center.y, 0), this.radius);
    }
}