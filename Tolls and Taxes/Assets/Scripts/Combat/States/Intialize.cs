
    using Unity.VisualScripting;

    public class Intialize : State
    {
        CombatCenter combatCenter;
        
        public override void EnterState()
        {
            Owner = this.GetComponent<Character>();
            combatCenter = Owner as CombatCenter;
            
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
    }
