using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class RestState : State
{
    // Input
    private InputAction cancelAction;
    
    // Coroutine Container
    private Coroutine restTimer;
    
    // The leader
    private PartyLeader leader;


    /// <inheritdoc/>
    public override void EnterState()
    {
        // Set owner and leader
        Owner = this.GetComponent<Character>();
        Owner.body.linearVelocity = Vector2.zero;
        Owner.spriteRenderer.color = Color.darkGreen;
        leader = Owner as PartyLeader;
        
        // Subscribe to controls
        cancelAction = InputSystem.actions.FindAction("Player/Crouch");
        cancelAction.performed += OnCancel;
        
        //Start the rest countdown
        restTimer = StartCoroutine(RestTimer());
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        Debug.Log("Cancelling...");
        StopCoroutine(restTimer);
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    /// <inheritdoc/>
    public override void ExitState()
    {
        // Unsubscribe from controls
        cancelAction.performed -= OnCancel;
        Owner.spriteRenderer.color = Color.white;
        Destroy(this);
    }

    /// <inheritdoc/>
    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Coroutine that shuffles everyone's decks after timer expires
    /// </summary>
    IEnumerator RestTimer()
    {
        Debug.Log("Resting!");
        yield return new WaitForSeconds(3f);
        leader.Deck.ResetAndShuffle();
        if (leader.NextMember != leader)
        {
            PartyMember currentMember = leader.NextMember;
            while (currentMember != leader)
            {
                currentMember.Deck.ResetAndShuffle();
                currentMember = currentMember.NextMember;
            }
        }
        restTimer = null;
        ChangeState(this.AddComponent<PlayerMovement>());
    }
    
}
