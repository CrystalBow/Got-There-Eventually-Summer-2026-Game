using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// State that runs player movement.
/// </summary>
public class PlayerMovement : State
{
    // Movement Stuff
    public InputAction moveAction;
    public Vector2 MoveDirection;
    
    //leader information
    private PartyLeader leader;
    private string leaderName;
    
    //Actions (Controls)
    private InputAction interactAction;
    private InputAction cardAction;
    private InputAction resetAction;
    private InputAction DeckViewAction;


    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    /// <inheritdoc/>
    public override void UpdateState()
    {
        if (MoveDirection.x < 0)
        {
            Owner.spriteRenderer.flipX = true;
        }
        else if (MoveDirection.x > 0)
        {
            Owner.spriteRenderer.flipX = false;
        }
        Owner.body.linearVelocity = MoveDirection;
    }

    public override void UnsubcribeState()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        cardAction.performed -= OnCard;
        interactAction.performed -= OnInteract;
        resetAction.performed -= OnReset;
        DeckViewAction.performed -= DeckViewActionOnPerformed;
    }

    public override void ResubscribeState()
    {
        if (moveAction == null)
        {
            return;
        }
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        cardAction.performed += OnCard;
        interactAction.performed += OnInteract;
        resetAction.performed += OnReset;
        DeckViewAction.performed += DeckViewActionOnPerformed;
    }

    /// <inheritdoc/>
    public override void EnterState()
    {
        //Initialize and Cast
        Owner = this.GetComponent<Character>();
        leader =  Owner as PartyLeader;
        
        //Initialize Controls
        moveAction = InputSystem.actions.FindAction("Player/Move");
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        
        cardAction = InputSystem.actions.FindAction("Player/Jump");
        cardAction.performed += OnCard;
        
        interactAction = InputSystem.actions.FindAction("Player/Interact");
        interactAction.performed += OnInteract;
        
        resetAction = InputSystem.actions.FindAction("Player/Attack");
        resetAction.performed += OnReset;
        
        DeckViewAction = InputSystem.actions.FindAction("Player/DeckView");
        DeckViewAction.performed += DeckViewActionOnPerformed;
        
    }

    private void DeckViewActionOnPerformed(InputAction.CallbackContext obj)
    {
        ChangeState(this.AddComponent<DeckViewing>());
    }

    // We shuffle the deck
    private void OnReset(InputAction.CallbackContext obj)
    {
        ChangeState(this.AddComponent<RestState>());
    }
    
    // We open the card menu
    private void OnCard(InputAction.CallbackContext obj)
    {
        ChangeState(this.AddComponent<CardPicker>());
    }
    
    /// <inheritdoc/>
    public override void ExitState()
    {
        //Unsubscribe from controls
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        cardAction.performed -= OnCard;
        interactAction.performed -= OnInteract;
        resetAction.performed -= OnReset;
        DeckViewAction.performed -= DeckViewActionOnPerformed;
        //the required destroy
        Destroy(this);
    }
    
    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveDirection = ctx.ReadValue<Vector2>() * leader.speed;
    }
    
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        // For debugging
        Debug.Log("E was pressed.");
        
        // Uses the state-changing method already provided.
        ChangeState(this.AddComponent<InteractState>());
    }
    
    
}
