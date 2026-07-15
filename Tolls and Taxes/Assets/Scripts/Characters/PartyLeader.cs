using System;
using Unity.VisualScripting;
using UnityEngine;

public class PartyLeader : Character
{
    public string LeaderName = "Samantha Pel";
    public int HP;
    public int MP;
    public static event Action<Vector2> Interaction;
    public Deck Deck = new Deck();
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        CurrentState = this.AddComponent<PlayerMovement>();
        body = this.GetComponent<Rigidbody2D>();
        CurrentState.EnterState();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    public void InteractionActivation()
    {
        Interaction?.Invoke(new Vector2(this.transform.position.x, this.transform.position.y));
    }
}
