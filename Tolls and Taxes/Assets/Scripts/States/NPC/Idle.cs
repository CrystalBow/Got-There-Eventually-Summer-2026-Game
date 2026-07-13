using UnityEditor.VersionControl;
using UnityEngine;

public class Idle : State
{

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    public override void UpdateState()
    {
        
    }

    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
    }

    public override void ExitState()
    {
        
    }
    
}
