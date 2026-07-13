using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    public State CurrentState { get; set; }
    public Rigidbody2D body;
    
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
