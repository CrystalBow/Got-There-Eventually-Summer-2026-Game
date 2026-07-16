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
    
    //---------------LEGACY STUFF-------------TO BE DELETED

    /// <summary>
    /// PerformCombat intakes all enemies and party members, constructs a list of strings,
    /// and uses those to get enemy stats and construct units
    /// We also separate session information (AlliesInCombat) from their static stats (AlliesInformation)
    /// </summary>
    /// 
    void checkInitiativeAndState()
    {
        if (aliveAllies == 0)
        {
            //instanceCombatState = CombatState.LOSE;
        }
        if (aliveEnemies == 0)
        {
            //instanceCombatState = CombatState.WIN;
        }

        if (initiativeOrder[turnPosition].isAlly == true)
        {
            //instanceCombatState = CombatState.PLAYERTURN;
        }
        else
        {
            //instanceCombatState = CombatState.ENEMYTURN;
        }
    }
    /*void EnemyTurn()
    {
        int whoToHit = Random.Range(0, aliveAllies);

        // We iterate over the turn order list looking for player characters to attack
        int temporaryTurnIterator = 0;
        // Index of who we attack
        int lastAllyIndex = 0;
        while (whoToHit > -1)
        {
            if ((initiativeOrder[temporaryTurnIterator].isAlly == true) && (ourAllies[initiativeOrder[temporaryTurnIterator].unitName].currentHP > 0))
            {
                lastAllyIndex = temporaryTurnIterator;
                whoToHit -= 1;
            }
            temporaryTurnIterator++;
        }
*/
        // When the loop ends, we deal damage to the chosen ally

        //ourAllies[initiativeOrder[lastAllyIndex].unitName].damage(ourEnemies[initiativeOrder[turnPosition].unitName].attack);

        //if (ourAllies[initiativeOrder[lastAllyIndex].unitName].isDead())
        //{
            //aliveAllies -= 1;
        //}

/*        //incrementTurnOrder();
        checkInitiativeAndState();
    }

    void outputTurnOrder(List<InitiativeToken> toOutput)
    {
        Debug.Log("We are here");
        foreach (var combatant in toOutput)
        {
            Debug.Log("Name is " + combatant.unitName + " isAlly is " + combatant.isAlly);
            if (combatant.isAlly == true)
            {
                //Debug.Log("Ally speed is " + ourAllies[combatant.unitName].speed);
            }
            else
            {
                //Debug.Log("Enemy speed is " + ourEnemies[combatant.unitName].speed);
            }
        }
    }
    void PerformCombat()
    {
        checkInitiativeAndState();
    }

    /// <summary>
    /// At the start of the round, we take in our initiative list and sort it according to speed.
    /// We prioritize players going first over enemies.
    /// We remove the top speed from toSort and add it to toReturn
    /// Then, we return the latter, sorted list.
    /// This should allow us to modify speed in combat if needed.
    /// </summary>
    /// <param name="toSort"></param>
    /// <param name="AlliesInformation"></param>
    /// <param name="EnemiesInCombat"></param>
    /// <returns></returns>
    //List<InitiativeToken> SortTurnOrder(List<InitiativeToken> toSort, Dictionary<string, PlayerCombatant> AlliesInformation, Dictionary<string, Combatant> EnemiesInCombat)
    //{
        // We return this list, and search the previous list to create a functional initiative list.
       // List<InitiativeToken> toReturn = new List<InitiativeToken>();
        //int originalCount = toSort.Count;

        // the list for sorting gets smaller, so we need a way to remember original list size
        /*for (int i = 0; i < originalCount; i++)
        {
            // Sets topSpeed to sort by.
            int topSpeed = -1;
            int topSpeedIndex = -1;

            for (int j = 0; j < toSort.Count; j++)
            {
                if (toSort[j].isAlly == true)
                {
                   // if (AlliesInformation[toSort[j].unitName].speed > topSpeed)
                   // {
                   //     topSpeed = AlliesInformation[toSort[j].unitName].speed;
                   //     topSpeedIndex = j;
                   // }
                }
                else
                {
                   // if (EnemiesInCombat[toSort[j].unitName].speed > topSpeed)
                   // {
                    //    topSpeed = EnemiesInCombat[toSort[j].unitName].speed;
                     //   topSpeedIndex = j;
                    }
                }
            }

            // We add the highest speed to the list, so on and so forth
          //  toReturn.Add(toSort[topSpeedIndex]);
           // toSort.RemoveAt(topSpeedIndex);
        //}

     //   return toReturn;
    }
    */

    /// <summary>
    /// An individual has multiple choices of card to use;
    /// therefore, we take an integer input dependent on which
    /// button was pressed, then choose that card from the deck
    /// </summary>
    /// <param name="cardChosen"></param>
   // public void CardButtonSelector(int cardChosen)
    //{
       // Debug.Log("I am working");
    //}

    /// <summary>
    /// This function increments the turn position
    /// It also avoids dead units
    /// </summary>
    //public void incrementTurnOrder()
    //{
    //    if ((turnPosition + 1) == initiativeOrder.Count)
      //  {
    //        turnPosition = 0;
        }
    //    else
        //{
     //       turnPosition++;
        //}

        // If we ever land on a dead enemy, they just get skipped.
    //    if (initiativeOrder[turnPosition].isAlly == true)
        //{
     //       if (ourAllies[initiativeOrder[turnPosition].unitName].isDead())
            //{
                //incrementTurnOrder();
            //}
        //}
    //    else
     //   {
    //        if (ourEnemies[initiativeOrder[turnPosition].unitName].isDead())
           // {
                //incrementTurnOrder();
           // }
       // }
   // }
//}
