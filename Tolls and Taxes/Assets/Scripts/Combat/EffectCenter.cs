using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using System.Collections.Generic;

public static class EffectCenter // : MonoBehaviour
{

    /*
     * This function checks for Damage multiplier effects and returns the largest one present.
     */

    // This function checks for a non-damage enemy debuff effect
    public static bool enemyNeutralEffect(List<int> listToCheck)
    {
        foreach(var effectTerm in listToCheck)
        {
            if ((effectTerm >= 9) && (effectTerm < 12))
            {
                return true;
            } 
        }

        return false;
    }
    public static int GetDamageMultiplier(List<int> listToCheck)
    {
        if (listToCheck.Contains(4) == true)
        {
            return 4;
        }
        else if (listToCheck.Contains(3) == true)
        {
            return 3;
        }
        else if (listToCheck.Contains(2) == true)
        {
            return 2;
        }

        return 1;
    }

    /*
     * This function takes a spell's mp cost and an effect list and returns an altered cost.
     */
    public static int returnEffectedMPValue (int initialMP, EffectList listToCheck)
    {
        bool containsEfficiency = listToCheck.effectIsPresent("Magic Efficiency");
        bool containsInefficiency = listToCheck.effectIsPresent("Magic Inefficiency");

        if (containsEfficiency && containsInefficiency)
        {
            return initialMP;
        }
        else if (containsEfficiency == true)
        {
            int toReturn = initialMP;
            toReturn = initialMP / 2;

            // No free 1 mp skills
            if (toReturn == 0)
            {
                return 1;
            }

            return toReturn;
        }
        else if (containsInefficiency == true)
        {
            return (initialMP * 2);
        }

        return initialMP;

    }
    public static int GetSpeedModifier(EffectList listToCheck)
    {
        int toReturn = 0;

        if (listToCheck.effectIsPresent("Speed Up") == true)
        {
            toReturn += 1;
        }

        if (listToCheck.effectIsPresent("Speed Down") == true)
        {
            toReturn -= 1;
        }

        return toReturn;
    }

    public static int GetAttackModifier(EffectList listToCheck)
    {
        int toReturn = 0;

        if (listToCheck.effectIsPresent("Attack Up") == true)
        {
            toReturn += 1;
        }

        if (listToCheck.effectIsPresent("Attack Down") == true)
        {
            toReturn -= 1;
        }

        return toReturn;
    }

    public static int GetDefenseModifier(EffectList listToCheck)
    {
        int toReturn = 0;

        if (listToCheck.effectIsPresent("Defense Up") == true)
        {
            toReturn += 1;
        }

        if (listToCheck.effectIsPresent("Defense Down") == true)
        {
            toReturn -= 1;
        }

        return toReturn;
    }
}
