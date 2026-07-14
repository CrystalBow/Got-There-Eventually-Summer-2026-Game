using UnityEngine;
using UnityEngine.InputSystem;

public class InteractState : State
{
    private InputAction interactAction;

    public override void EnterState()
    {
        // Make sure it's using right character.
        Owner = GetComponent<Character>();
        Owner.body.linearVelocity = Vector2.zero;

        if (InteractableObject.CurrentInteractable != null)
        {
            InteractableObject.CurrentInteractable.Interact();
        }

        // Pressing Interact again will close the interaction.
        interactAction = InputSystem.actions.FindAction("Player/Interact");
        interactAction.performed += OnInteract;
    }

    public override void ExitState()
    {
        interactAction.performed -= OnInteract;
        Debug.Log("Exited Interact State.");
    }

    public override void UpdateState()
    {
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        PlayerMovement movementState = GetComponent<PlayerMovement>();

        if (movementState == null)
        {
            movementState = gameObject.AddComponent<PlayerMovement>();
        }

        ChangeState(movementState);
    }
}