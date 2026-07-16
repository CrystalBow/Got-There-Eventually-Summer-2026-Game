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
    private PartyMember member;
    private Character Selection;
    int chosenCardIndex;
    selectionType SelectionType;

    
    enum selectionType
    {
        member,
        leader
    }
    
    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        Owner.body.linearVelocity = Vector2.zero;
        leader = Owner as PartyLeader;
        Selection = leader;
        Selection.spriteRenderer.color = Color.blue;
        SelectionType = selectionType.leader;
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
        if (SelectionType == selectionType.member)
        {
            member.Deck.DiscardHand();
        }
        else
        {
           leader.Deck.DiscardHand(); 
        }
        UnfocusCard();
        Selection.spriteRenderer.color = Color.white;
        HideHand();
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    private void OnApproved(InputAction.CallbackContext obj)
    {
        if (SelectionType == selectionType.member)
        {
            if (member.Deck.HandCards.Count == 0)
            {
                return;
            }
            CardLogic(member.Deck.HandCards[chosenCardIndex]);
            member.Deck.DiscardCard(member.Deck.HandCards[chosenCardIndex]);
        }
        else
        {
            if (leader.Deck.HandCards.Count == 0)
            {
                return;
            }
            CardLogic(leader.Deck.HandCards[chosenCardIndex]);
            leader.Deck.DiscardCard(leader.Deck.HandCards[chosenCardIndex]);
        }
        UnfocusCard();
        Selection.spriteRenderer.color = Color.white;
        HideHand();
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    private void OnPrevious(InputAction.CallbackContext obj)
    {
        if (SelectionType == selectionType.member)
        {
            if (member.Deck.HandCards.Count == 0)
            {
                return;
            }
            if (chosenCardIndex == 0)
            {
                alterChosenIndex(member.Deck.HandCards.Count - 1);
            }
            else
            {
                alterChosenIndex(chosenCardIndex - 1);
            }
        }
        else
        {
            if (leader.Deck.HandCards.Count == 0)
            {
                return;
            }
            if (chosenCardIndex == 0)
            {
                alterChosenIndex(leader.Deck.HandCards.Count - 1);
            }
            else
            {
                alterChosenIndex(chosenCardIndex - 1);
            }
        }
    }

    private void OnNext(InputAction.CallbackContext obj)
    {
        if (SelectionType == selectionType.member)
        {
            if (member.Deck.HandCards.Count == 0)
            {
                return;
            }
            if (chosenCardIndex >= member.Deck.HandCards.Count - 1)
            {
                alterChosenIndex(0);
            }
            else
            {
                alterChosenIndex(chosenCardIndex + 1);
            }
        }
        else
        {
            if (leader.Deck.HandCards.Count == 0)
            {
                return;
            }
            if (chosenCardIndex >= leader.Deck.HandCards.Count - 1)
            {
                alterChosenIndex(0);
            }
            else
            {
                alterChosenIndex(chosenCardIndex + 1);
            }
        }
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        Selection.spriteRenderer.color = Color.white;
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
        Selection.spriteRenderer.color = Color.white;
        if (x > 0)
        {
            if (SelectionType == selectionType.leader)
            {
                SwapSelection(leader.nextMember);
            }
            else
            {
                if (member.NextMember == null)
                {
                    SwapSelection(leader);
                }
                else
                {
                    SwapSelection(member.NextMember);
                }
            }
        }
        else
        {
            if (SelectionType == selectionType.leader)
            {
                SwapSelection(leader.LastMember);
            }
            else
            {
                if (member.PreviousMember == null)
                {
                    SwapSelection(leader);
                }
                else
                {
                    SwapSelection(member.PreviousMember);
                }
            }
        }
        Selection.spriteRenderer.color = Color.blue;
    }

    private void ShowHand() 
    {   
        if (SelectionType == selectionType.member)
        {
            member.cardTray.SetActive(true);
            member.Deck.DrawHand(5 - member.Deck.HandCards.Count);
            for (int i = 0; i < member.cards.Count; i++)
            {
                if (i < member.Deck.HandCards.Count)
                {
                    member.cards[i].gameObject.SetActive(true);
                    member.cards[i].DisplayCard(member.Deck.HandCards[i]);
                }
                else
                {
                    member.cards[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            leader.cardTray.SetActive(true);
            leader.Deck.DrawHand(5-leader.Deck.HandCards.Count);
            for (int i = 0; i < leader.cards.Count; i++)
            {
                if (i < leader.Deck.HandCards.Count)
                {
                    leader.cards[i].gameObject.SetActive(true);
                    leader.cards[i].DisplayCard(leader.Deck.HandCards[i]);
                }
                else
                {
                    leader.cards[i].gameObject.SetActive(false);
                }
            }
        }
        alterChosenIndex(0);
    }

    private void HideHand()
    {
        if (SelectionType == selectionType.member)
        {
            member.cardTray.SetActive(false);
        }
        else
        {
            leader.cardTray.SetActive(false);
        }
    }
    
    
    private void SwapSelection(Character newSelection)
    {
        HideHand();
        alterChosenIndex(0);
        Selection = newSelection;
        if (Selection is PartyMember)
        {
            SelectionType = selectionType.member;
            member = Selection as PartyMember;
        }
        else
        {
            SelectionType = selectionType.leader;
        }
        ShowHand();
    }

    private void FocusCard()
    {
        if (SelectionType == selectionType.member)
        {
            member.cards[chosenCardIndex].cardBackground.color = Color.darkViolet;
        }
        else
        {
            leader.cards[chosenCardIndex].cardBackground.color = Color.darkViolet;
        }
    }

    private void UnfocusCard()
    {
        if (SelectionType == selectionType.member)
        {
            member.cards[chosenCardIndex].cardBackground.color = Color.white;
        }
        else
        {
            leader.cards[chosenCardIndex].cardBackground.color = Color.white;
        }
    }
    
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
            if (SelectionType == selectionType.member)
            {
                if (cardByte.StaticData.Cost <= member.MP)
                {
                    member.MP -= cardByte.StaticData.Cost;
                    if (member.MP > DataCenter.Instance.Allies[member.MemberName].Mp)
                    {
                        member.MP = DataCenter.Instance.Allies[member.MemberName].Mp;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (cardByte.StaticData.Cost <= leader.MP)
                {
                    leader.MP -= cardByte.StaticData.Cost;
                    if (leader.MP > DataCenter.Instance.Allies[leader.LeaderName].Mp)
                    {
                        leader.MP = DataCenter.Instance.Allies[leader.LeaderName].Mp;
                    }
                }
                else
                {
                    return;
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

                    if (SelectionType == selectionType.member)
                    {
                        target.Hp -= DamageCalc(cardByte.StaticData.Damage);
                    }
                    else
                    {
                        target.Hp -= DamageCalc(cardByte.StaticData.Damage);
                    }
                    Destroyable.destroyables.Clear();
                }
            }
            
            
            
            
        }
        
        //Simple Healing
        if (cardByte.StaticData.Damage < 0)
        {
            if (SelectionType == selectionType.member)
                member.HP += HealCalc(cardByte.StaticData.Damage);
            else
                leader.HP += HealCalc(cardByte.StaticData.Damage);
        }
        
        //Apply Effect
        if (cardByte.StaticData.Effects.Contains(7))
        {
            leader.applyEffect(cardByte.StaticData.Time * 4, 7);
        }
    }
    
    public int DamageCalc(int Base)  
    {
        if (SelectionType == selectionType.member)
        {
            return Base + DataCenter.Instance.Allies[member.MemberName].Attack;
        }
        else
        {
            return Base + DataCenter.Instance.Allies[leader.LeaderName].Attack;
        }
    }

    public int HealCalc(int Base)
    {
        if (SelectionType == selectionType.member)
        {
            return (Base * -1) + DataCenter.Instance.Allies[member.MemberName].Attack;
        }
        else
        {
            return (Base * -1) + DataCenter.Instance.Allies[leader.LeaderName].Attack;
        }
    }

}