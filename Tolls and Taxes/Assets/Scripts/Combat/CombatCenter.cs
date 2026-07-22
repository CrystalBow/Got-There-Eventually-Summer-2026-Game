using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static TransferCenter;


public class CombatCenter : Character
{
    Dictionary<string, UnitData> EnemiesInCombat;
    public List<InitiativeToken> initiativeOrder;
    public GameObject cardTray;
    public List<CardUI> cardUI;


    public class InitiativeToken
    {
        public string unitName { get; init; }
        public bool isAlly { get; init; }

        public Combatant Reference;

        public InitiativeToken(string toName, bool toAlly, Combatant reference)
        {
            this.unitName = toName;
            this.isAlly = toAlly;
            this.Reference = reference;
        }
    }


    // used to determine win/loss and also random targeting
    public int aliveAllies;
    public int aliveEnemies;

    // this determines who goes when. It is decremented upon death. 
    // It is used to navigate initiativeOrder.
    public int turnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created



    protected override void Start()
    {
        CurrentState = this.AddComponent<Intialize>();
        CurrentState.EnterState();
        Combatant.OnDeath += CombatantOnOnDeath;
    }

    private void CombatantOnOnDeath(Combatant dead)
    {
        if (dead is PlayerCombatant)
        {
            aliveAllies -= 1;
        }
        else
        {
            aliveEnemies -= 1;
        }
    }
}