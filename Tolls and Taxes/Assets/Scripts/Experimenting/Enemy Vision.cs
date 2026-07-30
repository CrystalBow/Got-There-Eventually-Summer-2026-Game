using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine;

public class EnemyVisionCone : InteractableObject
{
    public ContactFilter2D contactFilter = new ContactFilter2D();
    public Rigidbody2D rb;
    public Vector2 previousDirection;
    public float DegreeView;
    public Foe owner;
    public Rigidbody2D thisBody;

    public override void Start()
    {
        base.Start();
        contactFilter.useLayerMask = true;
        contactFilter.SetLayerMask(LayerMask.GetMask("Player", "Follower"));
        thisBody = GetComponent<Rigidbody2D>();
        
        
    }
    
    
    public override void Update()
    {
        thisBody.linearVelocity = rb.linearVelocity;
        List<Collider2D> results = new List<Collider2D>();
        InteractableCollider.Overlap(contactFilter, results);
        Vector2 direction = rb.linearVelocity;
        if (direction == Vector2.zero)
        {
            direction = previousDirection;
        }
        foreach (Collider2D col in results)
        {
            float angle = Vector2.Angle(direction, col.transform.position - transform.position);
            Debug.DrawRay(transform.position, col.transform.position, Color.yellow);
            Debug.DrawRay(transform.position, direction, Color.red);
            if (angle > DegreeView)
            {
                continue;
            }
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance: radius);
            if (hit.collider == null)
            {
                continue;
            }

            if (!hit.collider.CompareTag("Player"))
            {
                continue;
            }

            if (owner.CurrentState is Patrol)
            {
                owner.CurrentState.ChangeState(owner.AddComponent<Persue>());
            }
        }
        previousDirection = direction;
    }
}
