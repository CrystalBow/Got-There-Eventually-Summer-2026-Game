using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TopofRound : State {

    CombatCenter combatCenter;
    
    public override void EnterState()
    {
        Owner = this.GetComponent<CombatCenter>();
        combatCenter = Owner as CombatCenter;

        sortInitiativeList();

        if (combatCenter.aliveEnemies == 0)
        {
            SceneManager.LoadScene("Prototype Start");
        }
        else if (combatCenter.aliveAllies == 0)
        {
            SceneManager.LoadScene("Prototype GameOver");
        }

        combatCenter.turnPosition = 0;

        bool foundDeadGuy = true;

        while (foundDeadGuy == true)
        {
            foundDeadGuy = combatCenter.initiativeOrder[combatCenter.turnPosition].Reference.isDead();

            if (foundDeadGuy == true)
            {
                combatCenter.turnPosition += 1;
            }
        }

        if (combatCenter.initiativeOrder[combatCenter.turnPosition].isAlly == true)
        {
            ChangeState(this.AddComponent<PlayerTurn>());
        }
        else
        {
            ChangeState(this.AddComponent<EnemyTurn>());
        }
        // throw new System.NotImplementedException();
    }

    /*
     * As you might expect, this function sorts initiativeOrder.
     * It's important to note that this only runs at top of round, 
     * meaning speed changes come to effect only after the new round begins.
     */
    public void sortInitiativeList()
    {
        // We have 2 lists, the current initiativeOrder, and a new list.
        // We replace the old list with a reference to toReturn after toSort is dealt with.
        // We need to replace the staticdata references once we add the effects manager
        List<CombatCenter.InitiativeToken> toSort = combatCenter.initiativeOrder;
        List<CombatCenter.InitiativeToken> toReturn = new List<CombatCenter.InitiativeToken>();

        int originalCount = toSort.Count;

        for (int i = 0; i < originalCount; i++)
        {
            int topSpeed = -1;
            int indexOfFastest = -1;

            for(int j = 0; j < toSort.Count; j++)
            {
                if (toSort[j].Reference.StaticData.Speed > topSpeed)
                {
                    topSpeed = toSort[j].Reference.StaticData.Speed;
                    indexOfFastest = j;
                }
            }

            toReturn.Add(toSort[indexOfFastest]);
            toSort.RemoveAt(indexOfFastest);
        }

        combatCenter.initiativeOrder = toReturn;
    }

    public override void ExitState()
    {
        Destroy(this);
        // throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}
