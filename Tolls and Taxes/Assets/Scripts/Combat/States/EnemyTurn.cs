using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * The EnemyTurn class is a state class that performs an enemy turn when triggered by a separate state.
 * It currently works relatively simply by attacking random, living party members.
 * It will be improved later to have a more complex "raffle system" probabilistic AI to make combat more interesting.
 */
public class EnemyTurn : State
{
    CombatCenter combatCenter;
    public override void EnterState()
    {
        Owner = this.GetComponent<CombatCenter>();
        combatCenter = Owner as CombatCenter;
        performEnemyTurn();
    }

    public override void ExitState()
    {
        combatCenter.initiativeOrder[combatCenter.turnPosition].Reference.ourEffects.DecrementEffectTimers();
        Debug.Log(combatCenter.initiativeOrder[combatCenter.turnPosition].Reference.CombatantName + " has these effects: " + combatCenter.initiativeOrder[combatCenter.turnPosition].Reference .ourEffects.GetEffectStrings());
        Destroy(this);
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void UnsubcribeState()
    {
        
    }

    public override void ResubscribeState()
    {
        
    }

    public void performEnemyTurn()
    {
        // Get an index of all living allies
        List<int> alliedIndices = new List<int>();

        /*
         * In order to get the index of living allies, we read the initiativeOrder.
         * We then check if a member is both alive and an ally, so we add them to the alliedIndices for potential targeting.
         */
        for (int i = 0; i < combatCenter.initiativeOrder.Count; i++)
        {
            if ((combatCenter.initiativeOrder[i].isAlly == true) && (combatCenter.initiativeOrder[i].Reference.isDead() == false))
            {
                alliedIndices.Add(i);
            }
        }

        /*
         * We already have a set of ally indices in the new list, so we choose a random target.
         * It's important to note, the indexes of attackTarget is NOT the index of the ally in initiativeOrder
         * attackTarget[x] holds an index for an ally in the initiativeOrder, which means you must reference both.
         */
        int attackTarget = Random.Range(0, alliedIndices.Count);
        
        // Right now, we just do basic damage.
        // Recall that defense is automatically accounted for without external input by the combatant classes.
        combatCenter.initiativeOrder[alliedIndices[attackTarget]].Reference.damage(combatCenter.initiativeOrder[combatCenter.turnPosition].Reference.StaticData.Attack);

        // We know we just attacked, so we reference this automatically tracked variable to see if the player has lost.
        if (combatCenter.aliveAllies == 0)
        {
            SceneManager.LoadScene("Prototype GameOver");
            return;
        }

        bool foundDeadGuy = true;
        // Need to increment first to properly check for out of bounds
        combatCenter.turnPosition += 1;

        /*
         * The initiativeOrder system maintains flexibility by maintaining all combatants alive and dead in the initiativeOrder.
         * This is so we can provide future support for revival mechanics if we want to implement it. 
         * The byproduct is that we always need to check if someone is alive before we enter a new turn.
         */
        while (foundDeadGuy == true)
        {
            // if out of bounds, the round is over so go to top of round
            if (combatCenter.turnPosition >= combatCenter.initiativeOrder.Count)
            {
                ChangeState(this.AddComponent<TopofRound>());
                return;
            }

            // the isdead functions returns a boolean of true if dead, so we use it to know when we find a living person.
            foundDeadGuy = combatCenter.initiativeOrder[combatCenter.turnPosition].Reference.isDead();

            if (foundDeadGuy == true)
            {
                combatCenter.turnPosition += 1;
            }
        }

        // We check if the alive person is an ally and change state accordingly.
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
