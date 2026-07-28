using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using JetBrains.Annotations;
using Unity.Hierarchy;

/*
 * This class is designed to be instanced on each and every combatant.
 * The basic idea is that it tracks the effects current active on a combatant for our reference.
 * the int effectName is the code used to determine the effect consequence.
 */
public class EffectList 
{
    List<EffectMaintenanceToken> currentEffects;
    public EffectList()
    {
        this.currentEffects = new List<EffectMaintenanceToken>();
    }

    // just returns list length.
    public int Length()
    {
        return currentEffects.Count;
    }

    /*
     * This method allows us to add an effect to the list with the given duration.
     */
    public void instateEffect(string effectName, int duration)
    {
        EffectMaintenanceToken addToList = new EffectMaintenanceToken(duration, effectName);

        for (int i = 0; i < currentEffects.Count; i++)
        {
            if (effectName.Equals(currentEffects[i].effectName))
            {
                currentEffects[i].timeTilEnd = duration;
                return;
            }
        }

        currentEffects.Add(addToList);
    }

    /*
     * This simple method is used to return a value depending on if a combatant is currently under a given effect.
     * This is valuable because it makes effect-application easier.
     */
    public bool effectIsPresent(string nameToFind)
    {
        for (int i = 0; i < currentEffects.Count; i++)
        {
            if (currentEffects[i].effectName.Equals(nameToFind))
            {
                return true;
            }
        }

        return false;
    } 

    /*
     * This decrements effect timers by 1 and removes any now stale effects.
     * In order to get the new list without old effects, we make a new list and just don't add an element with too low a time.
     * This allows us to manage effect timers with only a single function.
     */
    public void DecrementEffectTimers()
    {
        for (int i = 0; i < this.currentEffects.Count; i++) 
        {
            currentEffects[i].timeTilEnd -= 1;
        }

        List<EffectMaintenanceToken> toReplace = new List<EffectMaintenanceToken>();

        for (int i = 0; i < this.currentEffects.Count; i++)
        {
            if (currentEffects[i].timeTilEnd > -1)
            {
                toReplace.Add(currentEffects[i]);
            }
        }

        currentEffects = toReplace;
    }

    /*
     * The EffectMaintenanceToken class is a simple class that contains 2 pieces of information:
     * - The string name of an effect
     * - The remaining length of that effect.
     */
    public class EffectMaintenanceToken
    {
        public int timeTilEnd { get; set; }
        public string effectName { get; set; }

        public EffectMaintenanceToken(int time, string theName)
        {
            this.timeTilEnd = time;
            this.effectName = theName;
        }
    }

    public string GetEffectStrings()
    {
        string toReturn = "";
        foreach (var thisName in currentEffects)
        {
            toReturn = toReturn + thisName.effectName + " ";
        }

        return toReturn;
    }
}
