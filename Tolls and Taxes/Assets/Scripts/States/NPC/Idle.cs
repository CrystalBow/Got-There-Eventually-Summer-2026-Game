using UnityEngine;
/// <summary>
/// A starting subclass when a character just needs to stand there.
/// </summary>
public class Idle : State
{

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }
    
    /// <inheritdoc/>
    public override void UpdateState()
    {
        
    }

    public override void UnsubcribeState()
    {
        
    }

    public override void ResubscribeStates()
    {
        
    }

    /// <inheritdoc/>
    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
    }

    /// <inheritdoc/>
    public override void ExitState()
    {
        
    }
    
}
