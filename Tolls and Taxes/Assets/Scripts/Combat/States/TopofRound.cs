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

        combatCenter.initiativeOrder.Sort();

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
