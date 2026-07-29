using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetPicker : State
{
    public PartyMember Target;
    public PartyMember User;
    public CardByte Card;
    public int HandIndex;
    InputAction MoveInput;
    InputAction CancelInput;
    InputAction SelectInput;
    private List<int> BuffIDs = new List<int>();
    bool isLimited = false;
    private Coroutine limiter;
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void EnterState()
    {
        BuffIDs.Clear();
        BuffIDs.Add(5);
        BuffIDs.Add(6);
        BuffIDs.Add(8);
        Owner = this.GetComponent<Character>();
        Target = Owner as PartyMember;
        Target.ArrowPointer.gameObject.SetActive(true);
        
        MoveInput = InputSystem.actions.FindAction("Player/Move");
        MoveInput.performed += OnMove;
        
        CancelInput = InputSystem.actions.FindAction("Player/Crouch");
        CancelInput.performed += OnCancel;
        
        SelectInput = InputSystem.actions.FindAction("Player/Jump");
        SelectInput.performed += SelectInputOnperformed;
        
    }

    private void SelectInputOnperformed(InputAction.CallbackContext obj)
    {
        if (Card.StaticData.Effects.Count == 0)
        {
            
        }
        else
        { 
            foreach (int buffID in BuffIDs)
            {
                if (Card.StaticData.Effects.Contains(buffID))
                {
                    Target.applyEffect(Card.StaticData.Time * 6, buffID);
                }
            }
        }
        
        if (Card.StaticData.Cost < 0)
        {
            Target.MP -= Card.StaticData.Cost;
            int MaxMp = DataCenter.Instance.maxManaCalculation(DataCenter.Instance.Allies[Target.MemberName],
                Target.Level);
            if (Target.MP > MaxMp)
            {
                Target.MP = MaxMp;
            }
        }

        foreach (KeyValuePair<int, Coroutine> effect in User.expiringEffectRoster)
        {
            StopCoroutine(effect.Value);
        }
        User.expiringEffectRoster.Clear();
        User.Deck.DiscardCard(Card);
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        Vector2 moveVector = MoveInput.ReadValue<Vector2>();
        if (!isLimited)
        {
            Target.ArrowPointer.gameObject.SetActive(false);
            if (moveVector.x < 0)
            {
                Target = Target.PreviousMember;
            } else if (moveVector.x > 0)
            {
                Target = Target.NextMember;
            }
            Target.ArrowPointer.gameObject.SetActive(true);
        }
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        foreach (int effectID in Card.StaticData.Effects)
        {
            if (User.expiringEffectRoster.ContainsKey(effectID))
            {
                StopCoroutine(User.effectRoster[effectID]);
                User.effectRoster.Remove(effectID);
                User.effectRoster.Add(effectID, User.expiringEffectRoster[effectID]);
            }
            else
            {
                User.removeEffect(effectID);
            }
        }
        User.expiringEffectRoster.Clear();
        if (Card.StaticData.Cost >= 0)
        {
            Double finalCost = Card.StaticData.Cost;
            if (User.effectRoster.ContainsKey(8))
            {
                finalCost = Math.Floor(finalCost/2);
            }
            else if (User.effectRoster.ContainsKey(12))
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
            User.MP += (int)finalCost;
        }
        Target.ArrowPointer.gameObject.SetActive(false);
        ChangeState(this.AddComponent<PlayerMovement>());
    }

    public override void ExitState()
    {
        UnsubcribeState();
        Destroy(this);
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void UnsubcribeState()
    {
        Target.ArrowPointer.gameObject.SetActive(false);
        MoveInput.performed -= OnMove;
        CancelInput.performed -= OnCancel;
        SelectInput.performed -= SelectInputOnperformed;
        if (limiter != null)
        {
            StopCoroutine(limiter);
            isLimited = false;
        }
    }

    public override void ResubscribeState()
    {
        if (Target != null) 
        {
            Target.ArrowPointer.gameObject.SetActive(true);
        }
        if (MoveInput != null)
        {
            MoveInput.performed += OnMove;
            CancelInput.performed += OnCancel;
            SelectInput.performed += SelectInputOnperformed;
        }
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
