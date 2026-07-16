using System.Collections.Generic;
using Unity.VisualScripting;

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
            ChangeState(this.AddComponent<PlayerTurn>());
        }

        public override void ExitState()
        {
            Destroy(this);
        }

        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }
    }
