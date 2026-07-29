using JetBrains.Annotations;
using NUnit;
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
            int xpToAward = 0;

            foreach (var combatant in combatCenter.initiativeOrder)
            {
                if (combatant.isAlly == false)
                {
                    xpToAward += combatant.Reference.StaticData.Attack;
                    xpToAward += combatant.Reference.StaticData.Defense;
                    xpToAward += combatant.Reference.StaticData.Speed;
                }
            }

            foreach (var combatant in combatCenter.initiativeOrder)
            {
                if (combatant.isAlly == true)
                {
                    PlayerCombatant temporaryAllyReference = combatant.Reference as PlayerCombatant;

                    int xpPostLevel;
                    int endLevel;
                    int hpToSave;

                    if (DataCenter.Instance.shouldLevel(temporaryAllyReference.Level, temporaryAllyReference.currentXP + xpToAward) == true)
                    {
                        xpPostLevel = DataCenter.Instance.XPRemainder(temporaryAllyReference.Level, temporaryAllyReference.currentXP + xpToAward);
                        endLevel = temporaryAllyReference.Level + 1;
                    }
                    else
                    {
                        xpPostLevel = temporaryAllyReference.currentXP + xpToAward;
                        endLevel = temporaryAllyReference.Level;
                    }

                    if (temporaryAllyReference.isDead() == true)
                    {
                        hpToSave = 1;
                    }
                    else
                    {
                        hpToSave = temporaryAllyReference.currentHP;
                    }

                        TransferCenter.Instance.SaveCharacterState(temporaryAllyReference.CombatantName, temporaryAllyReference.Deck,
                            hpToSave, temporaryAllyReference.currentMP, endLevel, xpPostLevel);
                }
            }

            CombatTransitionManager.Instance.EndCombat();
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
    }
    

    public override void ExitState()
    {
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
}
