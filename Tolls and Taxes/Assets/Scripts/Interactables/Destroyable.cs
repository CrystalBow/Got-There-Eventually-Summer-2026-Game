using System.Collections.Generic;
using UnityEngine;

public class Destroyable : InteractableObject
{
    public static List<Destroyable> destroyables = new List<Destroyable>();
    
    public int Hp = 10;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        PartyLeader.Interaction -= PartyLeaderOnInteraction;
        PartyLeader.AOEAttack += PartyLeaderOnAOEAttack;
        PartyLeader.Attack += PartyLeaderOnAttack;
    }

    private void PartyLeaderOnAttack(Vector2 obj)
    {
        if (InteractableCollider.OverlapPoint(obj))
        {
            destroyables.Add(this);
        }
    }

    private void PartyLeaderOnAOEAttack(Vector2 arg1, int arg2)
    {
        if (InteractableCollider.OverlapPoint(arg1))
        {
            Hp = Hp - arg2;
        }
    }

    protected override void Interact()
    {
        Debug.Log("Destroyable Down!");
        Destroy(gameObject);
    }

    // Update is called once per frame
    public override void Update()
    {
        if (Hp <= 0)
        {
            Interact();
        }
    }
}
