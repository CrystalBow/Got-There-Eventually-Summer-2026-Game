using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardPicker : State
{
    private int selectedCardIndex;
    private InputAction moveAction;
    private InputAction cancelAction;
    private InputAction NextAction;
    private InputAction PreviousAction;
    private InputAction approveAction;
    private InputAction discardAction;
    private PartyLeader leader;
    private PartyMember _activeMember;

    int chosenCardIndex;
    
    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        Owner.body.linearVelocity = Vector2.zero;
        leader = Owner as PartyLeader;
        _activeMember = leader;
        leader.spriteRenderer.color = Color.blue;
        ShowHand();
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
        _activeMember.Deck.DiscardHand();
        UnfocusCard();
        _activeMember.spriteRenderer.color = Color.white;
        HideHand();
        ChangeState(this.AddComponent<PlayerMovement>());
    }

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
        _activeMember.spriteRenderer.color = Color.white;
        HideHand();
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    public override void ExitState()
    {
        moveAction.performed -= OnMove;
        cancelAction.performed -= OnCancel;
        NextAction.performed -= OnNext;
        PreviousAction.performed -= OnPrevious;
        approveAction.performed -= OnApproved;
        discardAction.performed -= OnDiscard;
        
        Destroy(this);
    }

    public override void UpdateState()
    {
        
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
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
    }

    private void ShowHand() 
    {   
        _activeMember.cardTray.SetActive(true);
        _activeMember.Deck.DrawHand(5 - _activeMember.Deck.HandCards.Count);
        for (int i = 0; i < _activeMember.cards.Count; i++)
        {
            if (i < _activeMember.Deck.HandCards.Count)
            {
                _activeMember.cards[i].gameObject.SetActive(true);
                _activeMember.cards[i].DisplayCard(_activeMember.Deck.HandCards[i]);
            }
            else
            {
                _activeMember.cards[i].gameObject.SetActive(false);
            }
        }
        alterChosenIndex(0);
    }

    private void HideHand()
    {
        _activeMember.cardTray.SetActive(false);
    }
    
    
    private void SwapSelection(PartyMember newSelection)
    {
        HideHand();
        alterChosenIndex(0);
        _activeMember = newSelection;
        ShowHand();
    }

    private void FocusCard() => _activeMember.cards[chosenCardIndex].cardBackground.color = Color.darkViolet;


    private void UnfocusCard() => _activeMember.cards[chosenCardIndex].cardBackground.color = Color.white;

    
    private void alterChosenIndex(int index)
    {
        UnfocusCard();
        chosenCardIndex = index;
        FocusCard();
    }

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
    
    public int DamageCalc(int Base)  
    {
        return Base + DataCenter.Instance.Allies[_activeMember.MemberName].Attack;
    }

    public int HealCalc(int Base)
    {
        return (Base * -1) + DataCenter.Instance.Allies[_activeMember.MemberName].Attack;
    }

}