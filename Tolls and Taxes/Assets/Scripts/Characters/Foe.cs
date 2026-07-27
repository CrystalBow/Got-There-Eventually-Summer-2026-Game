using System;
using UnityEngine;

public class Foe : Character
{
    public Collider2D bodyCollider;
    
    protected override void Start()
    {
        bodyCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CombatTransitionManager.Instance.StartCombat();
        Destroy(this.gameObject);
    }
}
