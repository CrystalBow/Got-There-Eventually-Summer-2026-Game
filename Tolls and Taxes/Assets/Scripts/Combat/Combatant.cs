using System;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : Character
{
    public string CombatantName;
    public string Location;
    public UnitData StaticData;
    public int currentHP { get; set; }
    
    public static event Action<Combatant> OnDeath;

    protected override void Start()
    {
        StaticData = DataCenter.Instance.Locations[Location][CombatantName];
        currentHP = StaticData.Hp;
        CardHandler.CallFoes += CallFoes;
    }

    private void CallFoes()
    {
        if (isDead())
        {
            return;
        }
        CardHandler.foes.Add(this);
    }

    public void damage(int damageNumber)
    {
        if (StaticData.Defense >= damageNumber)
        {
            return;
        }

        currentHP -= (damageNumber - StaticData.Defense);
        if (isDead())
        {
            OnDeath?.Invoke(this);
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public bool isDead()
    {
        if (currentHP <= 0)
        {
            return true;
        }
        return false;
    }
    
    
}
