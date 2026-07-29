using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DeckViewing : State
{
    PartyLeader leader;
    PartyMember CurrentMember;
    List<GameObject> PartyMemberIcons;
    private List<ViewIcon> ArrowList;
    private int ChosenPartyIndex;
    private List<CardByte> deckList;
    private int CardInspectIndex;
    
    private InputAction nextInput;
    private InputAction previousInput;
    private InputAction moveInput;
    private InputAction cancelInput;
    
    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        leader = Owner as PartyLeader;
        CurrentMember = Owner as PartyMember;
        ArrowList =  new List<ViewIcon>();
        leader.DeckMenu.gameObject.SetActive(true);
        PartyMemberIcons = new List<GameObject>();
        PartyMemberIcons.Add(Instantiate(leader.DeckMenu.IconCloner, leader.DeckMenu.PartyPanel.GetComponent<RectTransform>(), false));
        ChosenPartyIndex = 1;
        PartyMemberIcons[0].GetComponent<Image>().type = Image.Type.Sliced;
        PartyMemberIcons[0].GetComponent<Image>().sprite = leader.spriteRenderer.sprite;
        PartyMemberIcons[0].gameObject.SetActive(true);
        ArrowList.Add(PartyMemberIcons[0].GetComponent<ViewIcon>());
        CurrentMember = leader.NextMember;
        while (leader != CurrentMember)
        {
            PartyMemberIcons.Add(Instantiate(leader.DeckMenu.IconCloner, leader.DeckMenu.PartyPanel.GetComponent<RectTransform>(), false));
            PartyMemberIcons[ChosenPartyIndex].GetComponent<Image>().type = Image.Type.Sliced;
            PartyMemberIcons[ChosenPartyIndex].GetComponent<Image>().sprite = CurrentMember.spriteRenderer.sprite;
            PartyMemberIcons[ChosenPartyIndex].gameObject.SetActive(true);
            ArrowList.Add(PartyMemberIcons[ChosenPartyIndex].GetComponent<ViewIcon>());
            CurrentMember = CurrentMember.NextMember;
            ChosenPartyIndex++;
        }
        deckList = new List<CardByte>();
        ChosenPartyIndex = 0;
        CardInspectIndex = 0;
        showDeck();
        
        moveInput = InputSystem.actions.FindAction("Player/Move");
        moveInput.performed += MoveInputOnperformed;
        
        previousInput = InputSystem.actions.FindAction("Player/Previous");
        previousInput.performed += PreviousInputOnperformed;
        
        nextInput = InputSystem.actions.FindAction("Player/Next");
        nextInput.performed += NextInputOnperformed;
        
        cancelInput = InputSystem.actions.FindAction("Player/Crouch");
        cancelInput.performed += CancelInputOnperformed;
        
    }

    private void CancelInputOnperformed(InputAction.CallbackContext obj)
    {
        ChangeState(this.AddComponent<PlayerMovement>());
        foreach (var memeber in PartyMemberIcons)
        {
            Destroy(memeber);
        }
    }

    private void NextInputOnperformed(InputAction.CallbackContext obj)
    {
        NextCard();
    }

    private void PreviousInputOnperformed(InputAction.CallbackContext obj)
    {
        PreviousCard();
    }

    private void MoveInputOnperformed(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.action.ReadValue<Vector2>();
        if (input.x > 0)
        { 
            changePartyMemeber(1);   
        }
        else
        { 
            changePartyMemeber(-1);
        }
    }

    public override void ExitState()
    {
       leader.DeckMenu.gameObject.SetActive(false);
       moveInput.performed -= MoveInputOnperformed;
       nextInput.performed -= NextInputOnperformed;
       previousInput.performed -= PreviousInputOnperformed;
       cancelInput.performed -= CancelInputOnperformed;
       Destroy(this);
    }

    public override void UpdateState()
    {
        Debug.Log(CardInspectIndex);
        leader.DeckMenu.Cards[CardInspectIndex].DisplayCard(deckList[CardInspectIndex], true);
        displayDetails();
    }

    public override void UnsubcribeState()
    {
        moveInput.performed -= MoveInputOnperformed;
        nextInput.performed -= NextInputOnperformed;
        previousInput.performed -= PreviousInputOnperformed;
        cancelInput.performed -= CancelInputOnperformed;
    }

    public override void ResubscribeState()
    {
        if (moveInput == null)
        {
            return;
        }
        moveInput.performed += MoveInputOnperformed;
        nextInput.performed += NextInputOnperformed;
        previousInput.performed += PreviousInputOnperformed;
        cancelInput.performed += CancelInputOnperformed;
    }

    public void showDeck()
    {
        deckList.Clear();
        deckList.AddRange(CurrentMember.Deck.DiscardCards);
        deckList.AddRange(CurrentMember.Deck.DeckCards);
        deckList.AddRange(CurrentMember.Deck.HandCards);
        deckList.Sort();
        for (int i = 0; i < leader.DeckMenu.Cards.Count; i++)
        {
            if (i < deckList.Count)
            {
                leader.DeckMenu.Cards[i].gameObject.SetActive(true);
                bool isInspected = (i == CardInspectIndex);
                leader.DeckMenu.Cards[i].DisplayCard(deckList[i], isInspected);
            }
            else
            {
                leader.DeckMenu.Cards[i].gameObject.SetActive(false);
            }
            if (CardInspectIndex >= deckList.Count)
            {
                CardInspectIndex = 0;
            }
            displayDetails();
        }
    }
    
    public void displayDetails()
    {
        leader.DeckMenu.CardName.text = deckList[CardInspectIndex].Name;
        leader.DeckMenu.CardDescription.text = deckList[CardInspectIndex].StaticData.Description;
        leader.DeckMenu.CardCost.text = "Cost: " + deckList[CardInspectIndex].StaticData.Cost;
    }

    public void NextCard()
    {
        leader.DeckMenu.Cards[CardInspectIndex].DisplayCard(deckList[CardInspectIndex]);
        CardInspectIndex++;
        if (CardInspectIndex >= deckList.Count)
        {
            CardInspectIndex = 0;
        }
        UpdateState();
    }

    public void PreviousCard()
    {
        leader.DeckMenu.Cards[CardInspectIndex].DisplayCard(deckList[CardInspectIndex]);
        CardInspectIndex--;
        if (CardInspectIndex < 0)
        {
            CardInspectIndex = deckList.Count - 1;
        }
        UpdateState();
    }
    
    public void changePartyMemeber(int by)
    {
        ArrowList[ChosenPartyIndex].Arrow.gameObject.SetActive(false);
        if (by > 0)
        {
            ChosenPartyIndex++;
            CurrentMember = CurrentMember.NextMember;
        }
        else
        {
            ChosenPartyIndex--;
            CurrentMember = CurrentMember.PreviousMember;
        }
        if (ChosenPartyIndex >= PartyMemberIcons.Count)
        {
            ChosenPartyIndex = 0;
        }
        else if (ChosenPartyIndex < 0)
        {
            ChosenPartyIndex = PartyMemberIcons.Count - 1;
        }
        ArrowList[ChosenPartyIndex].Arrow.gameObject.SetActive(true);
        showDeck();
    }
    
}
