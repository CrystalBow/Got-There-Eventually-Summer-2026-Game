using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Foe : Character
{
    public BoxCollider2D bodyCollider;
    public List<String> Foes = new List<string>();
    private bool triggered = false;
    
    
    public static event Action PreBattleProcessing;
    
    
    protected override void Start()
    {
        bodyCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        CurrentState = this.AddComponent<Patrol>();
        CurrentState.EnterState();
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (string item in Foes)
            {
                TransferCenter.Instance.foeQueue.Add(item); 
            }
            triggered = true;
            PreBattleProcessing?.Invoke();
            CombatTransitionManager.Instance.StartCombat();
        }
    }


    public void OnEnable()
    {
        if (triggered)
        {
            Destroy(gameObject);
        }
    }
}
