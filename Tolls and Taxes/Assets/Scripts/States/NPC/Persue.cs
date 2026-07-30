using UnityEngine;

public class Persue : State
{


    public override void EnterState()
    {
        Owner = GetComponent<Character>();
        Owner.body.linearVelocity =  Vector2.zero;
    }

    public override void ExitState()
    {
        Destroy(this);
    }

    public override void UpdateState()
    {
        
    }

    public override void UnsubcribeState()
    {
        
    }

    public override void ResubscribeState()
    {
        
    }
}
