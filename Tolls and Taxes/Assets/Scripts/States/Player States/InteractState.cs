using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractState : State
{
    private InputAction interactAction;
    private PartyLeader leader;

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

    public override void ExitState()
    {
        interactAction.canceled -= OnInteract;
        Debug.Log("Exited Interact State.");
        Destroy(this);
    }

    public override void UpdateState()
    {
        leader.InteractionActivation();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        ChangeState(this.AddComponent<PlayerMovement>());
    }
}