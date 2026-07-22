using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurn : State
{
    public InputAction moveAction;
    public InputAction approveAction;
    public InputAction discardAction;
    public InputAction restAction;
    public CombatCenter center;
    public PlayerCombatant combatant;
    public int chosenCardIndex;

    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        center = Owner as CombatCenter;
        combatant = center.initiativeOrder[center.turnPosition].Reference as PlayerCombatant;
        if (combatant.isDead())
        {
            iterate();
            return;
        }
        
        moveAction = InputSystem.actions.FindAction("Player/Move");
        moveAction.performed += OnMove;
        approveAction = InputSystem.actions.FindAction("Player/Jump");
        approveAction.performed += OnApproved;
        discardAction = InputSystem.actions.FindAction("Player/Crouch");
        discardAction.performed += OnDiscard;
        restAction = InputSystem.actions.FindAction("Player/Attack");
        restAction.performed += OnRest;
        if (combatant.Deck.HandCards.Count == 0)
        {
            moveAction.performed -= OnMove;
            discardAction.performed -= OnDiscard;
        }
        chosenCardIndex = 0;
        UpdateState();
    }

    private void OnRest(InputAction.CallbackContext obj)
    {
        if (combatant.Deck.HandCards.Count > 0)
        {
            UnfocusCurrentCard();
        }
        combatant.Deck.ResetAndShuffle();
        iterate();
    }

    private void OnDiscard(InputAction.CallbackContext obj)
    {
        UnfocusCurrentCard();
        combatant.Deck.DiscardHand();
        iterate();
    }

    private void OnApproved(InputAction.CallbackContext obj)
    {
        if (combatant.Deck.HandCards.Count <= 0)
        {
            iterate();
            return;
        }
        UnfocusCurrentCard();
        CardHandler handler = this.AddComponent<CardHandler>();
        handler.currentPlayer = combatant;
        handler.currentCard = combatant.Deck.HandCards[chosenCardIndex];
        ChangeState(handler);
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        Vector2 MoveVector = obj.action.ReadValue<Vector2>();
        float direction = MoveVector.x;
        if (direction > 0)
        {
            changeIndex(1);
        }
        else
        {
            changeIndex(-1);
        }
    }

    public override void ExitState()
    {
        moveAction.performed -= OnMove;
        approveAction.performed -= OnApproved;
        discardAction.performed -= OnDiscard;
        restAction.performed -= OnRest;
        Destroy(this);
    }

    public override void UpdateState()
    {
        combatant.Deck.DrawHand(5-combatant.Deck.HandCards.Count);
        center.cardTray.SetActive(true);
        for (int i = 0; i < center.cardUI.Count; i++)
        {
            if (i < combatant.Deck.HandCards.Count)
            {
                center.cardUI[i].gameObject.SetActive(true);
                bool isSelected = (i == chosenCardIndex);
                // Pass focus state directly to DisplayCard
                center.cardUI[i].DisplayCard(combatant.Deck.HandCards[i], isSelected);
            }
            else
            {
                center.cardUI[i].gameObject.SetActive(false);
            }
        }
    }

    public void changeIndex(int amount)
    {
        UnfocusCurrentCard();
        chosenCardIndex += amount;
        if (chosenCardIndex >= combatant.Deck.HandCards.Count)
        {
            chosenCardIndex = 0;
        } else if (chosenCardIndex < 0)
        {
            chosenCardIndex = combatant.Deck.HandCards.Count - 1;
        }
        FocusCurrentCard();
    }
    
    private void FocusCurrentCard()
    {
        var hand = combatant.Deck.HandCards;
        center.cardUI[chosenCardIndex].SetFocusState(hand[chosenCardIndex], isFocused: true);
    }
    
    private void UnfocusCurrentCard()
    {
        var hand = combatant.Deck.HandCards;
        center.cardUI[chosenCardIndex].SetFocusState(hand[chosenCardIndex], isFocused: false);
    }
    

    public void iterate()
    {
        center.turnPosition++;
        if (center.turnPosition >= center.initiativeOrder.Count)
        {
            center.turnPosition = 0;
            ChangeState(this.AddComponent<TopofRound>());
        }
        else if (center.initiativeOrder[center.turnPosition].isAlly)
        {
            ChangeState(this.AddComponent<PlayerTurn>());
        }
        else
        {
            if (center.initiativeOrder[center.turnPosition].Reference.isDead())
            {
                iterate();
            }
            else
            {
                ChangeState(this.AddComponent<EnemyTurn>());
            }
        }
    }

    private void OnDestroy()
    {
        ExitState();
    }
}
