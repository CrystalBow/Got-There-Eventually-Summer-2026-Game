using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// State for interacting with interactable objects
/// </summary>
public class InteractState : State
{
    //Inputs
    private InputAction interactAction;
    //Party Leader
    private PartyLeader leader;


    /// <inheritdoc/>
    public override void EnterState()
    {
        // Setting up owner
        Owner = GetComponent<Character>();
        Owner.body.linearVelocity = Vector2.zero;
        if (Owner is PartyLeader)
        {
            leader = Owner as PartyLeader;
        }
        // Releasing the interact button will end the interaction
        interactAction = InputSystem.actions.FindAction("Player/Interact");
        interactAction.canceled += OnInteract;
        if (leader != null)
        {
            UpdateState();
        }
    }

    /// <inheritdoc/>
    public override void ExitState()
    {
        //Unsubscribe from inputs
        interactAction.canceled -= OnInteract;
        Debug.Log("Exited Interact State.");
        Destroy(this);
    }

    /// <inheritdoc/>
    public override void UpdateState()
    {
        leader.InteractionActivation();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        // When the botton is lifted player movement is made available again.
        ChangeState(this.AddComponent<PlayerMovement>());
    }
}