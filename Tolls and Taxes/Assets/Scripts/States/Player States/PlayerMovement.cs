using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : State
{
    
    public InputAction moveAction;
    public Vector2 MoveDirection;
    private string leaderName;
    private InputAction interactAction;
    private InputAction cardAction;

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
        
        cardAction = InputSystem.actions.FindAction("Player/Jump");
        cardAction.performed += OnCard;
        
        interactAction = InputSystem.actions.FindAction("Player/Interact");
        interactAction.performed += OnInteract;

        if (Owner is PartyLeader)
        {
            PartyLeader leader = Owner as PartyLeader;
            leaderName = leader.LeaderName;
        }
    }

    private void OnCard(InputAction.CallbackContext obj)
    {
        ChangeState(this.AddComponent<CardPicker>());
    }

    public override void ExitState()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        cardAction.performed -= OnCard;
        interactAction.performed -= OnInteract;
        Destroy(this);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveDirection = ctx.ReadValue<Vector2>() * DataCenter.Instance.Allies[leaderName].Speed;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        // For debugging
        Debug.Log("E was pressed.");
        
        // Uses the state-changing method already provided.
        ChangeState(this.AddComponent<InteractState>());
    }
}
