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
        leader.ArrowPointer.gameObject.SetActive(true);
        isLimited = false;
        // Activate UI
        ShowHand();
        PartyMember.MaficEffciencyExpire += PartyMemberOnMaficEffciencyExpire;
        PartyMember.MaficIneffciencyExpire += PartyMemberOnMaficIneffciencyExpire;
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

    private void PartyMemberOnMaficIneffciencyExpire()
    {
        ShowHand();
    }

    private void PartyMemberOnMaficEffciencyExpire()
    {
        ShowHand();
    }

    private void OnDiscard(InputAction.CallbackContext obj)
    {
        // Player discards their hand.
        UnfocusCard();
        _activeMember.Deck.DiscardHand();
        _activeMember.ArrowPointer.gameObject.SetActive(false);
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
        UnfocusCard();
        HideHand();
        _activeMember.ArrowPointer.gameObject.SetActive(false);
        if (CardLogic(_activeMember.Deck.HandCards[chosenCardIndex]))
        {
            _activeMember.Deck.DiscardCard(_activeMember.Deck.HandCards[chosenCardIndex]);
            ChangeState(this.AddComponent<PlayerMovement>());
            
        }
        else
        {
            TargetPicker picker =  this.AddComponent<TargetPicker>();
            picker.Card = _activeMember.Deck.HandCards[chosenCardIndex];
            picker.User = _activeMember;
            picker.HandIndex = chosenCardIndex;
            ChangeState(picker);
        }
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
        _activeMember.ArrowPointer.gameObject.SetActive(false);
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
        PartyMember.MaficEffciencyExpire -= PartyMemberOnMaficEffciencyExpire;
        PartyMember.MaficIneffciencyExpire -= PartyMemberOnMaficIneffciencyExpire;
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

    public override void UnsubcribeState()
    {
        moveAction.performed -= OnMove;
        cancelAction.performed -= OnCancel;
        NextAction.performed -= OnNext;
        PreviousAction.performed -= OnPrevious;
        approveAction.performed -= OnApproved;
        discardAction.performed -= OnDiscard;
        PartyMember.MaficEffciencyExpire -= PartyMemberOnMaficEffciencyExpire;
        PartyMember.MaficIneffciencyExpire -= PartyMemberOnMaficIneffciencyExpire;
    }

    public override void ResubscribeState()
    {
        if (moveAction == null)
        {
            return;
        }
        moveAction.performed += OnMove;
        cancelAction.performed += OnCancel;
        NextAction.performed += OnNext;
        PreviousAction.performed += OnPrevious;
        approveAction.performed += OnApproved;
        discardAction.performed += OnDiscard;
        PartyMember.MaficEffciencyExpire += PartyMemberOnMaficEffciencyExpire;
        PartyMember.MaficIneffciencyExpire += PartyMemberOnMaficIneffciencyExpire;
    }


    private void OnMove(InputAction.CallbackContext ctx)
    {
        // Check the rate limiter
        if (!isLimited)
        {
            // Change who the targeted member is.
            Vector2 direction = ctx.ReadValue<Vector2>();
            float x = direction.x;
            _activeMember.ArrowPointer.gameObject.SetActive(false);
            if (x > 0)
            {
                SwapSelection(_activeMember.NextMember);
            }
            else
            {
                SwapSelection(_activeMember.PreviousMember);
            }
            _activeMember.ArrowPointer.gameObject.SetActive(true);
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
                _activeMember.cards[i].DisplayCard(_activeMember.Deck.HandCards[i], isSelected, _activeMember.effectRoster.ContainsKey(8), _activeMember.effectRoster.ContainsKey(12));
            }
            else
            {
                _activeMember.cards[i].gameObject.SetActive(false);
            }
        }
        //Reset the selection Index
        
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
    private bool CardLogic(CardByte cardByte)
    {
        bool allDone = true;
        if (cardByte.StaticData.Cost >= 0)
        {
            Double finalCost = cardByte.StaticData.Cost;
            if (_activeMember.effectRoster.ContainsKey(8))
            {
                finalCost = Math.Floor(finalCost/2);
            }
            else if (_activeMember.effectRoster.ContainsKey(12))
            {
                if (finalCost == 0)
                {
                    finalCost = 1.0;
                }
                else
                {
                    finalCost = finalCost * 2;
                }
            }
            if (finalCost < _activeMember.MP)
            {
                _activeMember.MP -= (int)finalCost;
            }
            else
            {
                return true;
            }
        } else if (cardByte.StaticData.Cost < 0)
        {
            allDone = false;
        }

        

        bool isBuff = false;
        if (cardByte.StaticData.Effects.Count > 0)
        {
            List<int> buffsIDs = new List<int>();
            buffsIDs.Add(DataCenter.Instance.Effects["Attack Up"]);
            buffsIDs.Add(DataCenter.Instance.Effects["Defense Up"]);
            buffsIDs.Add(DataCenter.Instance.Effects["Magic Efficiency"]);
            foreach (int buffID in buffsIDs)
            {
                if (cardByte.StaticData.Effects.Contains(buffID))
                {
                    isBuff = true;
                }
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

            if (cardByte.StaticData.Effects.Contains(2))
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

            if (cardByte.StaticData.Effects.Contains(3))
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
                potentialTargets.Clear();
                leader.AttackActivation();
                potentialTargets = Destroyable.destroyables;
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

            if (cardByte.StaticData.Effects.Contains(4))
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
                potentialTargets.Clear();
                leader.AttackActivation();
                potentialTargets = Destroyable.destroyables;
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
                potentialTargets.Clear();
                leader.AttackActivation();
                potentialTargets = Destroyable.destroyables;
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
            
        } else if (isBuff || cardByte.StaticData.Damage < 0)
        {
            allDone = false;
            
        }
        
        
        //Apply Effect
        if (cardByte.StaticData.Effects.Contains(7))
        {
            leader.applyEffect(cardByte.StaticData.Time * 4, 7);
        }
        if (cardByte.StaticData.Effects.Contains(13))
        {
            _activeMember.applyEffect(cardByte.StaticData.Time * 6, 13);
        }

        return allDone;
    }
    /// <summary>
    /// Calculates Damage
    /// </summary>
    /// <param name="Base">The Base Damage of a card/param>
    /// <returns>the calculated damage</returns>
    public int DamageCalc(int Base)  
    {
        double Damage = DataCenter.Instance.AttackCalculation(DataCenter.Instance.Allies[_activeMember.MemberName], _activeMember.Level);
        if (_activeMember.effectRoster.ContainsKey(5))
        {
            Damage *= 2;
        }
        if (_activeMember.effectRoster.ContainsKey(9))
        {
            Damage = Math.Floor(Damage / 2);
        }
        return Base+(int)Damage;
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