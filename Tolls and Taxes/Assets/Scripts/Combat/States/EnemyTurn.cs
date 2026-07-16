using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTurn : State
{
    CombatCenter combatCenter;
    public override void EnterState()
    {
        Owner = this.GetComponent<CombatCenter>();
        combatCenter = Owner as CombatCenter;
        // throw new System.NotImplementedException();
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

    public void performEnemyTurn()
    {
        // Get an index of all living allies
        List<int> alliedIndices = new List<int>();

        for (int i = 0; i < combatCenter.initiativeOrder.Count; i++)
        {
            if ((combatCenter.initiativeOrder[i].isAlly == true) && (combatCenter.initiativeOrder[i].Reference.isDead() == false))
            {
                alliedIndices.Add(i);
            }
        }

        int attackTarget = Random.Range(0, alliedIndices.Count);

        combatCenter.initiativeOrder[alliedIndices[attackTarget]].Reference.damage(combatCenter.initiativeOrder[combatCenter.turnPosition].Reference.StaticData.Attack);

        if (combatCenter.aliveAllies == 0)
        {
            // Do scene transition here for loss
        }

        bool foundDeadGuy = true;
        // Need to increment first to properly check for out of bounds
        combatCenter.turnPosition += 1;

        while (foundDeadGuy == true)
        {
            // if out of bounds, the round is over so go to top of round
            if (combatCenter.turnPosition >= combatCenter.initiativeOrder.Count)
            {
                ChangeState(this.AddComponent<TopofRound>());
            }

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
    }
}
