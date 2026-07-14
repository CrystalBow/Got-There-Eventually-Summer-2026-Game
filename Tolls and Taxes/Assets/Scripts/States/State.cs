using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected Character Owner { get; set; }
    
    
    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void UpdateState();

    protected void ChangeState(State newState)
    {
        ExitState();
        Owner.CurrentState = newState;
        newState.EnterState();
    }

}
