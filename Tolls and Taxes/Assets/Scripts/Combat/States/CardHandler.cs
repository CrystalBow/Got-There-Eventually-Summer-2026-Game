using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardHandler : State
{
    public static List<Combatant> foes = new List<Combatant>();
    public static List<PlayerCombatant> allies =  new List<PlayerCombatant>();
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

        // Shows the controls used while choosing a target for the selected card.
        // This only updates the reminder text and does not alter targeting behavior.
        // Added by Maria G
        ControlReminderUI.Instance?.Show(
            ControlReminderContext.TargetSelection);

        moveInput = InputSystem.actions.FindAction("Player/Move");
        moveInput.performed += OnMove;
        ApproveInput = InputSystem.actions.FindAction("Player/Jump");
        ApproveInput.performed += OnApprove;
        CancelInput = InputSystem.actions.FindAction("Player/Crouch");
        CancelInput.performed += OnCancel;
        UpdateState();
    }

    public void Begin(CardByte Card, PlayerCombatant Player)
    {
        currentCard = Card;
        currentPlayer = Player;
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        if (EffectCenter.returnEffectedMPValue(currentCard.StaticData.Cost, currentPlayer.ourEffects) <= currentPlayer.currentMP)
        {
            currentPlayer.currentMP += EffectCenter.returnEffectedMPValue(currentCard.StaticData.Cost, currentPlayer.ourEffects);
            if (currentPlayer.currentMP > DataCenter.Instance.maxManaCalculation(currentPlayer.StaticPlayableData, currentPlayer.Level))
            {
                currentPlayer.currentMP = DataCenter.Instance.maxManaCalculation(currentPlayer.StaticPlayableData, currentPlayer.Level);
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
            // Because player decrements their own effects at end of turn AFTER this action, we add 1 to the time to account for this.
            if (currentCard.StaticData.Damage > 0)
            {
                // Runs as many times as the Damage effect specifies
                for (int i = 0; i < EffectCenter.GetDamageMultiplier(currentCard.StaticData.Effects); i++)
                {
                    foes[targetIndex].damage(currentCard.StaticData.Damage + DataCenter.Instance.AttackCalculation(currentPlayer.StaticPlayableData, currentPlayer.Level) + EffectCenter.GetAttackModifier(currentPlayer.ourEffects));
                }
            }
            EvaluateUserEffects(targetIndex, false, currentCard.StaticData.Effects, currentCard.StaticData.Time + 1);
            EvaluateEnemyEffects(targetIndex, false, currentCard.StaticData.Effects, currentCard.StaticData.Time);
        }
        else
        {
            if (currentCard.StaticData.Cost < 0)
            {
                allies[targetIndex].currentMP -= EffectCenter.returnEffectedMPValue(currentCard.StaticData.Cost, currentPlayer.ourEffects);
                int MaxMP = DataCenter.Instance.maxManaCalculation(currentPlayer.StaticPlayableData, currentPlayer.Level);  
                if (allies[targetIndex].currentMP > MaxMP)
                {
                    allies[targetIndex].currentMP = MaxMP;
                }
            }
            else
            {
                allies[targetIndex].damage(currentCard.StaticData.Damage - DataCenter.Instance.AttackCalculation(currentPlayer.StaticPlayableData, currentPlayer.Level) - EffectCenter.GetAttackModifier(currentPlayer.ourEffects));
                EvaluateAllyEffects(targetIndex, currentCard.StaticData.Effects, currentCard.StaticData.Time);
                EvaluateNegativeSelfEffects(currentPlayer, currentCard.StaticData.Effects, currentCard.StaticData.Time + 1);
            }
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
        
        if (EffectCenter.returnEffectedMPValue(currentCard.StaticData.Cost, currentPlayer.ourEffects) <= currentPlayer.currentMP)
        {
            currentPlayer.currentMP -= EffectCenter.returnEffectedMPValue(currentCard.StaticData.Cost, currentPlayer.ourEffects);
            int MaxMP = DataCenter.Instance.maxManaCalculation(currentPlayer.StaticPlayableData, currentPlayer.Level);
            if (currentPlayer.currentMP > MaxMP)
            {
                currentPlayer.currentMP = MaxMP;
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
                    foe.damage(currentCard.StaticData.Damage + DataCenter.Instance.AttackCalculation(currentPlayer.StaticPlayableData, currentPlayer.Level) + EffectCenter.GetAttackModifier(currentPlayer.ourEffects));
                }
                currentPlayer.Deck.DiscardCard(currentCard);
                evaluateExit();
            }
        } else if (currentCard.StaticData.Damage <= 0)
        {
            if (EffectCenter.enemyNeutralEffect(currentCard.StaticData.Effects) == true)
            {
                CallFoes?.Invoke();
            }
            else
            {
                CallAllies?.Invoke();
            }
        }
        FocusTarget();
    }

    public override void UnsubcribeState()
    {
        moveInput.performed -= OnMove;
        CancelInput.performed -= OnCancel;
        ApproveInput.performed -= OnApprove;
    }

    public override void ResubscribeState()
    {
        if (moveInput == null)
        {
            return;
        }
        moveInput.performed += OnMove;
        CancelInput.performed += OnCancel;
        ApproveInput.performed += OnApprove;
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

    public void EvaluateUserEffects(int ourTargetIndex, bool isAlly, List<int> EffectsToEvaluate, int effectTime)
    {

        foreach(var effect in EffectsToEvaluate)
        {
            switch (effect)
            {
                case 5:
                    currentPlayer.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 6:
                    currentPlayer.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 7:
                    currentPlayer.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 8:
                    currentPlayer.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 12:
                    currentPlayer.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 13:
                    currentPlayer.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(10), effectTime);
                    break;
                case 14:
                    currentPlayer.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(11), effectTime);
                    break;
            }
        }
    }

    public void EvaluateEnemyEffects(int ourTargetIndex, bool isAlly, List<int> EffectsToEvaluate, int effectTime)
    {

        foreach (var effect in EffectsToEvaluate)
        {
            switch (effect)
            {
                case 9:
                    foes[ourTargetIndex].ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 10:
                    foes[ourTargetIndex].ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 11:
                    foes[ourTargetIndex].ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
            }
        }
    }

    public void EvaluateAllyEffects(int ourTargetIndex, List<int> EffectsToEvaluate, int effectTime)
    {
        foreach (var effect in EffectsToEvaluate)
        {
            switch (effect)
            {
                case 5:
                    allies[ourTargetIndex].ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 6:
                    allies[ourTargetIndex].ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 7:
                    allies[ourTargetIndex].ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
                case 8:
                    allies[ourTargetIndex].ourEffects.instateEffect(DataCenter.Instance.GetEffectName(effect), effectTime);
                    break;
            }
        }
    }

    public void EvaluateNegativeSelfEffects(PlayerCombatant toApply, List<int> EffectsToEvaluate, int effectTime)
    {
        foreach(var effect in EffectsToEvaluate)
        {
            switch(effect)
            {
                case 13:
                    toApply.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(10), effectTime);
                    break;
                case 14:
                    toApply.ourEffects.instateEffect(DataCenter.Instance.GetEffectName(11), effectTime);
                    break;
            }
        }
    }
}
