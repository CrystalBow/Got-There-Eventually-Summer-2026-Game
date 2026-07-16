using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardHandler : State
{
    public static List<Combatant> foes;
    public static List<PlayerCombatant> allies;
    public static Action CallFoes;
    public static Action CallAllies;
    public CardByte currentCard;
    public PlayerCombatant currentPlayer;
    public InputAction moveInput;
    public InputAction ApproveInput;
    public InputAction CancelInput;
    int targetIndex = 0;

    public override void EnterState()
    {
        Owner = this.GetComponent<Character>();
        foes = new List<Combatant>();
        allies = new List<PlayerCombatant>();
        moveInput = InputSystem.actions.FindAction("Player/Move");
        moveInput.performed += OnMove;
        ApproveInput = InputSystem.actions.FindAction("Player/Jump");
        ApproveInput.performed += OnApprove;
        CancelInput = InputSystem.actions.FindAction("Player/Crouch");
        CancelInput.performed += OnCancel;
        UpdateState();
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        if (currentCard.StaticData.Cost <= currentPlayer.currentMP)
        {
            currentPlayer.currentMP += currentCard.StaticData.Cost;
            if (currentPlayer.currentMP > currentPlayer.StaticPlayableData.Mp)
            {
                currentPlayer.currentMP = currentPlayer.StaticPlayableData.Mp;
            }
        }
        ChangeState(this.AddComponent<PlayerTurn>());
    }

    private void OnApprove(InputAction.CallbackContext obj)
    {
        if (allies.Count == 0 && foes.Count == 0)
        {
            evaluateExit();
        } else if (allies.Count == 0)
        {
            foes[targetIndex].damage(currentCard.StaticData.Damage + currentPlayer.StaticPlayableData.Attack);
        }
        else
        {
            allies[targetIndex].damage(currentCard.StaticData.Damage);
        }
        currentPlayer.Deck.DiscardCard(currentCard);
        evaluateExit();
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        Vector2 moveVector = moveInput.ReadValue<Vector2>();
        float direction = moveVector.x;
        if (moveVector.x > 0)
        {
            targetChange(1);
        }
        else
        {
            targetChange(-1);
        }
    }

    public override void ExitState()
    {
        allies.Clear();
        foes.Clear();
        moveInput.performed -= OnMove;
        CancelInput.performed -= OnCancel;
        ApproveInput.performed -= OnApprove;
        Destroy(this);
    }

    public override void UpdateState()
    {
        if (currentCard.StaticData.Cost <= currentPlayer.currentMP)
        {
            currentPlayer.currentMP -= currentCard.StaticData.Cost;
            if (currentPlayer.currentMP > currentPlayer.StaticPlayableData.Mp)
            {
                currentPlayer.currentMP = currentPlayer.StaticPlayableData.Mp;
            }
        }
        else
        {
            evaluateExit();
            return;
        }

        if (currentCard.StaticData.Damage > 0)
        {
            CallFoes?.Invoke();
            if (currentCard.StaticData.Effects.Contains(1))
            {
                foreach (Combatant foe in foes)
                {
                    foe.damage(currentCard.StaticData.Damage + currentPlayer.StaticPlayableData.Attack);
                }
                currentPlayer.Deck.DiscardCard(currentCard);
                evaluateExit();
            }
        } else if (currentCard.StaticData.Damage < 0)
        {
            CallAllies?.Invoke();
        }
        FocusTarget();
    }

    public void evaluateExit()
    {
        CombatCenter center = Owner as CombatCenter;
        unFocusTarget();
        center.turnPosition++;
        if (center.turnPosition >= center.initiativeOrder.Count)
        {
            ChangeState(this.AddComponent<TopofRound>());
            return;
        }

        while (center.initiativeOrder[center.turnPosition].Reference.isDead())
        {
            center.turnPosition++;
            if (center.turnPosition >= center.initiativeOrder.Count)
            {
                ChangeState(this.AddComponent<TopofRound>());
                return;
            }
        }

        if (center.initiativeOrder[center.turnPosition].isAlly)
        {
            ChangeState(this.AddComponent<PlayerTurn>());
        }
        else
        {
            ChangeState(this.AddComponent<EnemyTurn>());
        }
    }

    public void targetChange(int amount)
    {
        unFocusTarget();
        targetIndex += amount;
        if (foes.Count == 0)
        {
            if (targetIndex >= allies.Count)
            {
                targetIndex = 0;
            }
            else if (targetIndex < 0)
            {
                targetIndex = allies.Count - 1;
            }
        }
        else
        {
            if (targetIndex >= foes.Count)
            {
                targetIndex = 0;
            } 
            else if (targetIndex < 0)
            {
                targetIndex = foes.Count - 1;
            }
        }
        FocusTarget();
    }

    public void FocusTarget()
    {
        if (foes.Count == 0 && allies.Count == 0)
        {
            return;
        }
        if (foes.Count == 0)
        {
            allies[targetIndex].GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            foes[targetIndex].GetComponent<SpriteRenderer>().color = Color.orangeRed;
        }
    }
    
    public void unFocusTarget()
    {
        if (foes.Count == 0 && allies.Count == 0)
        {
            return;
        }
        if (foes.Count == 0)
        {
            allies[targetIndex].GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            foes[targetIndex].GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
