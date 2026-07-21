using UnityEngine;
/// <summary>
/// An abstract class that acts as a basis for our state machines. 
/// </summary>
public abstract class State : MonoBehaviour
{
    /// <summary>
    /// The Owner of the state machine in question.
    /// </summary>
    /// <remarks>
    /// Can be used to fetch other components or cast to get other methods of a character subclass.
    /// </remarks>
    protected Character Owner { get; set; }
    
    /// <summary>
    /// Occurs when the state enters
    /// </summary>
    /// <remarks>
    /// Remember to initialize the owner
    /// </remarks>
    public abstract void EnterState();

    /// <summary>
    /// Occurs when the state exits
    /// </summary>
    /// <remarks>
    /// Remember to Destroy(this) in your exit states.
    /// </remarks>
    public abstract void ExitState();
    
    /// <summary>
    /// An updating function. you can shove it in your update somewhere or repurpose it for something else.
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// A wonderful little utility that swaps the state of the machine.
    /// </summary>
    /// <param name="newState">The new state you want to enter</param>
    /// <remarks>
    /// Calls enter and exit for you.
    /// </remarks>
    protected void ChangeState(State newState)
    {
        ExitState();
        Owner.CurrentState = newState;
        newState.EnterState();
    }

}
