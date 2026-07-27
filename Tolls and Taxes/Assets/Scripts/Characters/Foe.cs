using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Foe : Character
{
    public Collider2D bodyCollider;
    public List<String> Foes = new List<string>();
    
    
    public static event Action PreBattleProcessing;
    
    
    protected override void Start()
    {
        bodyCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (string item in Foes)
        {
           TransferCenter.Instance.foeQueue.Add(item); 
        }
        PreBattleProcessing?.Invoke();
        CombatTransitionManager.Instance.StartCombat();
        Destroy(this.gameObject);
    }
}
