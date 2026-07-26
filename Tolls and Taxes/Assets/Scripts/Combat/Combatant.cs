using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Combatant is a class that is also inherited by PlayerCombatant.
 * It is used to manage fighters in combat, particularly enemies.
 * As you might expect, PlayerCombatant is for player allies.
 */
public class Combatant : Character
{
    public string CombatantName;
    public string Location;
    public UnitData StaticData;
    public int currentHP { get; set; }

    /*
     * It remains important to manage a counter for enemy and player death.
     * We therefore use this event to do so.
     */
    public static event Action<Combatant> OnDeath;

    /*
     * staticdata refers to data that is not regularly changed during combat (speed, attack etc.)
     * It is important to note that effects do exist, and we must account for them when making actual decisions on stats.
     */
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

    /*
     * This function deals damage to this enemy, accounting automatically for defense.
     */
    public void damage(int damageNumber)
    {
        if (StaticData.Defense >= damageNumber)
        {
            return;
        }

        // If the damage kills thsi enemy, we want to ensure we count it and take it off the screen.
        currentHP -= (damageNumber - StaticData.Defense);
        if (isDead())
        {
            OnDeath?.Invoke(this);
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    // Just checks if we're dead.
    public bool isDead()
    {
        if (currentHP <= 0)
        {
            return true;
        }
        return false;
    }
    
    
}
