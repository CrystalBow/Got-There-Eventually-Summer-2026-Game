using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Intialize : State
    {
        CombatCenter combatCenter;
        
        public override void EnterState()
        {
            Owner = this.GetComponent<Character>();
            combatCenter = Owner as CombatCenter;
            combatCenter.initiativeOrder = new List<CombatCenter.InitiativeToken>();
            combatCenter.turnPosition = 0;
            combatCenter.aliveAllies = 0;
            combatCenter.aliveEnemies = 0;
            Combatant[] combatants =  this.GameObject().GetComponentsInChildren<Combatant>();
            foreach (Combatant combatant in combatants)
            {
                combatant.Initialize();
                if (combatant is PlayerCombatant)
                {
                    combatCenter.aliveAllies += 1;
                    combatCenter.initiativeOrder.Add(new CombatCenter.InitiativeToken(combatant.CombatantName, true, combatant));
                }
                else
                {
                    combatCenter.aliveEnemies += 1;
                    combatCenter.initiativeOrder.Add(new CombatCenter.InitiativeToken(combatant.CombatantName, false, combatant));
                }
            }
            //ChangeState(this.AddComponent<TopofRound>());
            Debug.Log("Going to top of Round");
            ChangeState(this.AddComponent<TopofRound>());
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

        public override void ResubscribeStates()
        {
            
        }
    }
