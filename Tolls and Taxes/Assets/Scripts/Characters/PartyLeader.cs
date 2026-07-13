using Unity.VisualScripting;
using UnityEngine;

public class PartyLeader : Character
{
    public string LeaderName = "Samantha Pel";
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
}
