using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

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
    // EffectList maintains a list of effects
    public EffectList ourEffects;
    public int currentHP { get; set; }
    public SpriteAtlas spriteAtlas;

    /*
     * It remains important to manage a counter for enemy and player death.
     * We therefore use this event to do so.
     */
    public static event Action<Combatant> OnDeath;
    

    public virtual void Initialize()
    {
        StaticData = DataCenter.Instance.Locations[Location][CombatantName];
        currentHP = StaticData.Hp;
        ourEffects = new EffectList();
        CardHandler.CallFoes += CallFoes;
        spriteRenderer =  GetComponent<SpriteRenderer>();
        if (CombatantName == "Skeleton")
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite("Skeleton");
        } else if (CombatantName == "Zombie")
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite("Zombie");
        } else if (CombatantName == "Living Shrub")
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite("Fire_Card_3_5");
        } else if (CombatantName == "Bandit")
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite("Bandit");
        } else if (CombatantName == "Tutorial Bug")
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite("Bug");
        }  else if (CombatantName == "Traitor Knight")
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite("Knight");
        }

        spriteRenderer.flipX = true;
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
    public virtual void damage(int damageNumber)
    {
        if (StaticData.Defense + EffectCenter.GetDefenseModifier(ourEffects) >= damageNumber)
        {
            return;
        }

        // If the damage kills this enemy, we want to ensure we count it and take it off the screen.
        currentHP -= (damageNumber - (StaticData.Defense + EffectCenter.GetDefenseModifier(ourEffects)));
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

    protected void OnDeathInvoker(Combatant combatant)
    {
        OnDeath?.Invoke(combatant);
    }

    public virtual void OnDestroy()
    {
        CardHandler.CallFoes -= CallFoes;
    }
}
