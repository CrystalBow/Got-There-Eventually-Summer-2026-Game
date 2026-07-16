using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerCombatant : Combatant
{
    public int currentMP { get; set; }
    public PlayableData StaticPlayableData { get; set; }
    public Deck Deck;

    protected override void Start()
    {
        StaticPlayableData = DataCenter.Instance.Allies[CombatantName];
        TransferCenter.CharacterSessionData data = TransferCenter.Instance.GetCharacterState(CombatantName);
        currentMP = data.CurrentMp;
        currentHP =  data.CurrentHp;
        Deck = data.Deck;
        StaticData = StaticPlayableData;
        CardHandler.CallAllies += CallAllies;
    }

    private void CallAllies()
    {
        if (isDead())
        {
            return;
        }
        CardHandler.allies.Add(this);
    }
}
