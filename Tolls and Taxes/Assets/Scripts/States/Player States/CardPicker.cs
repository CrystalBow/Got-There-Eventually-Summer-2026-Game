using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// State for playing cards in the overworld.
/// </summary>
public class CardPicker : State
{
    // Actions
    private InputAction moveAction;
    private InputAction cancelAction;
    private InputAction NextAction;
    private InputAction PreviousAction;
    private InputAction approveAction;
    private InputAction discardAction;
    
    // Controller Limiting
    private Coroutine limiter;
    private bool isLimited;
    
    // Party member selection
    private PartyLeader leader;
    private PartyMember _activeMember;

    // Card selection
    int chosenCardIndex;
    

    /// <inheritdoc/>
    public override void EnterState()
    {
        // Set owner and other variables
        Owner = this.GetComponent<Character>();
        Owner.body.linearVelocity = Vector2.zero;
        leader = Owner as PartyLeader;
        _activeMember = leader;
        leader.spriteRenderer.color = Color.blue;
        isLimited = false;
        // Activate UI
        ShowHand();
        
        // Subscribe to controls
        moveAction = InputSystem.actions.FindAction("Player/Move");
        moveAction.performed += OnMove;
        cancelAction = InputSystem.actions.FindAction("Player/Crouch");
        cancelAction.performed += OnCancel;
        NextAction = InputSystem.actions.FindAction("Player/Next");
        NextAction.performed += OnNext;
        PreviousAction = InputSystem.actions.FindAction("Player/Previous");
        PreviousAction.performed += OnPrevious;
        approveAction = InputSystem.actions.FindAction("Player/Jump");
        approveAction.performed += OnApproved;
        discardAction =  InputSystem.actions.FindAction("Player/Attack");
        discardAction.performed += OnDiscard;
    }
    
    private void OnDiscard(InputAction.CallbackContext obj)
    {
        // Player discards their hand.
        _activeMember.Deck.DiscardHand();
        UnfocusCard();
        _activeMember.spriteRenderer.color = Color.white;
        HideHand();
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    /// <summary>
    /// A card get played
    /// </summary>
    private void OnApproved(InputAction.CallbackContext obj)
    {
        if (_activeMember.Deck.HandCards.Count == 0)
        {
            return;
        }
        CardLogic(_activeMember.Deck.HandCards[chosenCardIndex]);
        _activeMember.Deck.DiscardCard(_activeMember.Deck.HandCards[chosenCardIndex]);
        UnfocusCard();
        _activeMember.spriteRenderer.color = Color.white;
        HideHand();
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    private void OnPrevious(InputAction.CallbackContext obj)
    {
        //Select left
        if (_activeMember.Deck.HandCards.Count == 0)
        {
            return;
        }
        if (chosenCardIndex == 0)
        {
            alterChosenIndex(_activeMember.Deck.HandCards.Count - 1);
        }
        else
        {
            alterChosenIndex(chosenCardIndex - 1);
        }
    }

    private void OnNext(InputAction.CallbackContext obj)
    {
        //Select Right
        if (_activeMember.Deck.HandCards.Count == 0)
        {
            return;
        }
        if (chosenCardIndex >= _activeMember.Deck.HandCards.Count - 1)
        {
            alterChosenIndex(0);
        }
        else
        {
            alterChosenIndex(chosenCardIndex + 1);
        }
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        // Canceling sends us back to walking
        _activeMember.spriteRenderer.color = Color.white;
        HideHand();
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    /// <inheritdoc/>
    public override void ExitState()
    {
        //Unsubscribe from controls
        moveAction.performed -= OnMove;
        cancelAction.performed -= OnCancel;
        NextAction.performed -= OnNext;
        PreviousAction.performed -= OnPrevious;
        approveAction.performed -= OnApproved;
        discardAction.performed -= OnDiscard;
        // Halt the rate limiter
        if (limiter != null)
        {
            StopCoroutine(limiter);
        }
        Destroy(this);
    }
    
    /// <inheritdoc/>
    public override void UpdateState()
    {
        
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        // Check the rate limiter
        if (!isLimited)
        {
            // Change who the targeted member is.
            Vector2 direction = ctx.ReadValue<Vector2>();
            float x = direction.x;
            _activeMember.spriteRenderer.color = Color.white;
            if (x > 0)
            {
                SwapSelection(_activeMember.NextMember);
            }
            else
            {
                SwapSelection(_activeMember.PreviousMember);
            }
            _activeMember.spriteRenderer.color = Color.blue;
            limiter = StartCoroutine(slowJoystick(0.2f));
        }
    }

    private void ShowHand() 
    {   
        //Activate the card tray
        _activeMember.cardTray.SetActive(true);
        //Make sure hand is full
        _activeMember.Deck.DrawHand(5 - _activeMember.Deck.HandCards.Count);
        for (int i = 0; i < _activeMember.cards.Count; i++)
        {
            //Iterate over UI to swt active and display the cards in hand.
            if (i < _activeMember.Deck.HandCards.Count)
            {
                _activeMember.cards[i].gameObject.SetActive(true);
                bool isSelected = (i == chosenCardIndex);
                _activeMember.cards[i].DisplayCard(_activeMember.Deck.HandCards[i], isSelected);
            }
            else
            {
                _activeMember.cards[i].gameObject.SetActive(false);
            }
        }
        //Reset the selection Index
        alterChosenIndex(0);
    }

    private void HideHand()
    {
        _activeMember.cardTray.SetActive(false);
    }
    
    /// <summary>
    /// Change the selected party member
    /// </summary>
    /// <param name="newSelection">The new party member.</param>
    private void SwapSelection(PartyMember newSelection)
    {
        HideHand();
        alterChosenIndex(0);
        _activeMember = newSelection;
        ShowHand();
    }

    /// <summary>
    /// Highlights selected card.
    /// </summary>
    private void FocusCard()
    {
        CardByte card = _activeMember.Deck.HandCards[chosenCardIndex];
        _activeMember.cards[chosenCardIndex].SetFocusState(card, isFocused: true);
    }

    /// <summary>
    /// Unhighlights selected card.
    /// </summary>
    private void UnfocusCard()
    {
        CardByte card = _activeMember.Deck.HandCards[chosenCardIndex];
        _activeMember.cards[chosenCardIndex].SetFocusState(card, isFocused: false);
    }

    /// <summary>
    /// Alters the chose index and changes the highlighting.
    /// </summary>
    /// <param name="index">The new index</param>
    private void alterChosenIndex(int index)
    {
        UnfocusCard();
        chosenCardIndex = index;
        FocusCard();
    }

    /// <summary>
    /// Takes in a card byte and executes the card.
    /// </summary>
    /// <param name="cardByte">the chose cards data</param>
    private void CardLogic(CardByte cardByte)
    {
        //Register Cost
        if (cardByte.StaticData.Cost != 0)
        {
            if (cardByte.StaticData.Cost <= _activeMember.MP)
            {
                _activeMember.MP -= cardByte.StaticData.Cost;
                if (_activeMember.MP > DataCenter.Instance.Allies[_activeMember.MemberName].Mp)
                {
                    _activeMember.MP = DataCenter.Instance.Allies[_activeMember.MemberName].Mp;
                }
            }
            else
            {
                return;
            }
        }
        
        //Deal Damage
        if (cardByte.StaticData.Damage > 0)
        {
            if (cardByte.StaticData.Effects.Contains(1))
            {
                leader.AOEAttackActivation(DamageCalc(cardByte.StaticData.Damage));
            }
            else
            {
                leader.AttackActivation();
                List<Destroyable> potentialTargets = Destroyable.destroyables;
                if (potentialTargets.Count > 0)
                {
                    Destroyable target = potentialTargets[0];
                    float targetDistance = Vector2.Distance(leader.transform.position, target.transform.position);
                    for (int i = 1; i < potentialTargets.Count; i++)
                    {
                        if (targetDistance > Vector2.Distance(potentialTargets[i].transform.position,
                                leader.transform.position))
                        {
                            target = potentialTargets[i];
                            targetDistance = Vector2.Distance(potentialTargets[i].transform.position,
                                leader.transform.position);
                        }
                    }
                    
                    target.Hp -= DamageCalc(cardByte.StaticData.Damage);
                    Destroyable.destroyables.Clear();
                }
            }
            
            
            
            
        }
        
        //Simple Healing
        if (cardByte.StaticData.Damage < 0)
        {
            _activeMember.HP += HealCalc(cardByte.StaticData.Damage);
        }
        
        //Apply Effect
        if (cardByte.StaticData.Effects.Contains(7))
        {
            leader.applyEffect(cardByte.StaticData.Time * 4, 7);
        }
    }
    /// <summary>
    /// Calculates Damage
    /// </summary>
    /// <param name="Base">The Base Damage of a card/param>
    /// <returns>the calculated damage</returns>
    public int DamageCalc(int Base)  
    {
        return Base + DataCenter.Instance.Allies[_activeMember.MemberName].Attack;
    }

    /// <summary>
    /// Calculates Healing
    /// </summary>
    /// <param name="Base">Base healing of the card</param>
    /// <returns>the calculated healing</returns>
    public int HealCalc(int Base)
    {
        return (Base * -1) + DataCenter.Instance.Allies[_activeMember.MemberName].Attack;
    }

    /// <summary>
    /// Slows the joystick to make party member selection more manageable on controller.
    /// </summary>
    /// <param name="time">How long to lock it out for.</param>
    IEnumerator slowJoystick(float time)
    {
        isLimited = true;
        yield return new WaitForSeconds(time);
        isLimited = false;
    }
}