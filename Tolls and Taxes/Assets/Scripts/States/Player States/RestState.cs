using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class RestState : State
{
    private InputAction cancelAction;
    private Coroutine restTimer;
    private PartyLeader leader;

    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        Owner.body.linearVelocity = Vector2.zero;
        Owner.spriteRenderer.color = Color.darkGreen;
        cancelAction = InputSystem.actions.FindAction("Player/Crouch");
        cancelAction.performed += OnCancel;
        leader = Owner as PartyLeader;
        restTimer = StartCoroutine(RestTimer());
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        Debug.Log("Cancelling...");
        StopCoroutine(restTimer);
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    public override void ExitState()
    {
        cancelAction.performed -= OnCancel;
        Owner.spriteRenderer.color = Color.white;
        Destroy(this);
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

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
