using System;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    public State CurrentState { get; set; }
    [NonSerialized]
    public Rigidbody2D body;
    
    // For testing
    [NonSerialized]
    public SpriteRenderer spriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        CurrentState = this.AddComponent<Idle>();
        body = this.GetComponent<Rigidbody2D>();
        CurrentState.EnterState();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
