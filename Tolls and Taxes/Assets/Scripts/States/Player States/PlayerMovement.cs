using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : State
{
    
    public InputAction moveAction;
    public Vector2 MoveDirection;
    private string leaderName;
    private InputAction interactAction;

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    public override void UpdateState()
    {
        Owner.body.linearVelocity = MoveDirection;
    }

    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        moveAction = InputSystem.actions.FindAction("Player/Move");
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        interactAction = InputSystem.actions.FindAction("Player/Interact");
        interactAction.performed += OnInteract;

        if (Owner is PartyLeader)
        {
            PartyLeader leader = Owner as PartyLeader;
            leaderName = leader.LeaderName;
        }
    }

    public override void ExitState()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        interactAction.performed -= OnInteract;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveDirection = ctx.ReadValue<Vector2>() * DataCenter.Instance.Allies[leaderName].Speed;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        // For debugging
        Debug.Log("E was pressed.");
        if (InteractableObject.CurrentInteractable == null)
        {
            return;
        }

        InteractState interactState = GetComponent<InteractState>();

        if (interactState == null)
        {
            interactState = gameObject.AddComponent<InteractState>();
        }

        // Uses the state-changing method already provided.
        ChangeState(interactState);
    }
}
