using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/*
 * This script manages an individual combatatant for the player.
 */
public class PlayerCombatant : Combatant
{
    public int Level;
    public int currentXP;
    public int currentMP { get; set; }


    public PlayableData StaticPlayableData { get; set; }
    public Deck Deck;
    

    public override void Initialize()
    {
        StaticPlayableData = DataCenter.Instance.Allies[CombatantName];
        StaticData = StaticPlayableData;
        TransferCenter.CharacterSessionData data = TransferCenter.Instance.GetCharacterState(CombatantName);
        currentMP = data.CurrentMp;
        currentHP =  data.CurrentHp;
        currentXP = data.CurrentXp;
        ourEffects = new EffectList();
        Level = data.CurrentLevel;
        Deck = data.Deck;
        CardHandler.CallAllies += CallAllies;
        CombatCenter.OnGameWon += CombatCenterOnOnGameWon;
    }

    private void CombatCenterOnOnGameWon()
    {
        TransferCenter.Instance.SaveCharacterState(CombatantName, Deck,currentHP, currentMP, Level, currentXP);
    }

    private void CallAllies()
    {
        if (isDead())
        {
            return;
        }
        CardHandler.allies.Add(this);
    }

    public override void damage(int damageNumber)
    {
        int defense = DataCenter.Instance.DefenseCalculation(StaticPlayableData, Level) + EffectCenter.GetDefenseModifier(ourEffects);

        if (damageNumber < 0)
        {
            int toHeal = damageNumber * (-1);

            if (currentHP + toHeal > StaticPlayableData.Hp)
            {
                currentHP = StaticPlayableData.Hp;
            }
            else
            {
                currentHP += toHeal;
            }

            return;
        }

        if (defense >= damageNumber)
        {
            
        }
        else
        {
            currentHP -= (damageNumber - defense);
            if (isDead())
            {
                OnDeathInvoker(this);
            }
        }
        
    }
}
