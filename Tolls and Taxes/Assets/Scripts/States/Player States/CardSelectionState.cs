using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardSelectionState : State
{
    private List<KeyValuePair<string, CardData>> cards;
    private int selectedCardIndex;
    private InputAction moveAction;

    public override void EnterState()
    {
        Owner = GetComponent<Character>();

        // Copy the existing attack cards from the DataCenter into a list.
        cards = new List<KeyValuePair<string, CardData>>(
            DataCenter.Instance.GlobalCards.Attacks
        );

        selectedCardIndex = 0;

        if (cards.Count == 0)
        {
            Debug.Log("No cards are available.");
            return;
        }

        moveAction = InputSystem.actions.FindAction("Player/Move");

        if (moveAction != null)
        {
            moveAction.performed += OnMove;
        }

        ShowSelectedCard();
    }

    public override void ExitState()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
        }

        Debug.Log("Exited Card Selection State.");
    }

    public override void UpdateState()
    {
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        // Use left and right movement to scroll through the cards.
        if (input.x > 0)
        {
            selectedCardIndex++;
        }
        else if (input.x < 0)
        {
            selectedCardIndex--;
        }
        else
        {
            return;
        }

        // Loop back around when reaching either end of the list.
        if (selectedCardIndex >= cards.Count)
        {
            selectedCardIndex = 0;
        }
        else if (selectedCardIndex < 0)
        {
            selectedCardIndex = cards.Count - 1;
        }

        ShowSelectedCard();
    }

    private void ShowSelectedCard()
    {
        KeyValuePair<string, CardData> selectedCard =
            cards[selectedCardIndex];

        Debug.Log(
            "Selected card: " +
            selectedCard.Key +
            " - " +
            selectedCard.Value.Description
        );
    }
}