using UnityEngine;

public class EffectCenter : MonoBehaviour
{

    public int GetSpeedModifier(EffectList listToCheck)
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

    public int GetAttackModifier(EffectList listToCheck)
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

    public int GetDefenseModifier(EffectList listToCheck)
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
