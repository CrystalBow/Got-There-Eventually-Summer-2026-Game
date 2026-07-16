using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TopofRound : State {

    CombatCenter combatCenter;
    
    public override void EnterState()
    {
        Owner = this.GetComponent<CombatCenter>();
        combatCenter = Owner as CombatCenter;

        sortInitiativeList();

        if (combatCenter.aliveEnemies == 0)
        {
            // do something for victory
        }
        else if (combatCenter.aliveAllies == 0)
        {
            // do something for getting your ass beat
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

    public void sortInitiativeList()
    {
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
