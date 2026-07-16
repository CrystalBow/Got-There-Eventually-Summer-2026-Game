using System.Collections.Generic;
using UnityEngine;
using static TransferCenter;

public enum CombatState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }

public class CombatCenter : MonoBehaviour
{
    Dictionary<string, UnitData> EnemiesInCombat;
    Dictionary<string, UnitData> EnemiesInformation;
    Dictionary<string, CharacterSessionData> AlliesInCombat;
    Dictionary<string, PlayableData> AlliesInformation;
    List<InitiativeToken> initiativeOrder;

    Dictionary<string, EnemyCombatant> ourEnemies;
    Dictionary<string, PlayerCombatant> ourAllies;

    // used to determine win/loss and also random targeting
    int aliveAllies;
    int aliveEnemies;

    // this determines who goes when. It is decremented upon death. 
    // It is used to navigate initiativeOrder.
    int currentTurnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    CombatState instanceCombatState;

    void Start()
    {
        instanceCombatState = CombatState.START;

        // Initialize the stuff we need
        EnemiesInCombat = GenerateEnemies.createTutorialEnemies();
        EnemiesInformation = new Dictionary<string, UnitData>();
        AlliesInCombat = new Dictionary<string, CharacterSessionData>();
        AlliesInformation = new Dictionary<string, PlayableData>();
        initiativeOrder = new List<InitiativeToken>();

        ourEnemies = new Dictionary<string, EnemyCombatant>();
        ourAllies = new Dictionary<string, PlayerCombatant>();

        currentTurnPosition = 0;

        aliveAllies = 0;
        aliveEnemies = 0;



        foreach (var tempMember in TransferCenter.Instance.PartyOrder)
        {
            Debug.Log("Char name is " + tempMember);
            Debug.Log("Char name is again " + TransferCenter.Instance.GetCharacterState(tempMember).CharacterName);
            Debug.Log("Char HP is again " + TransferCenter.Instance.GetCharacterState(tempMember).CurrentHp);
            Debug.Log("Standard speed is " + DataCenter.Instance.Allies[tempMember].Speed);

            ourAllies.Add(tempMember, createPlayerCombatant(TransferCenter.Instance.GetCharacterState(tempMember), DataCenter.Instance.Allies[tempMember]));

            // Because this for loop adds allies, we set isAlly to true.
            InitiativeToken tempInit = new InitiativeToken(tempMember, true);
            initiativeOrder.Add(tempInit);

            aliveAllies += 1;
        }

        foreach (var tempEnemy in EnemiesInCombat.Keys)
        {
            InitiativeToken tempInit = new InitiativeToken(tempEnemy, false);
            initiativeOrder.Add(tempInit);

            // adding to max stat list, so you can't heal past max
            // Also making a single enemy class for ease of use
            EnemyCombatant tempPracticalEnemy = new EnemyCombatant(tempEnemy, EnemiesInCombat[tempEnemy].Hp, EnemiesInCombat[tempEnemy].Attack, EnemiesInCombat[tempEnemy].Defense, EnemiesInCombat[tempEnemy].Speed);
            ourEnemies.Add(tempEnemy, tempPracticalEnemy);
            EnemiesInformation.Add(tempEnemy, EnemiesInCombat[tempEnemy]);

            aliveEnemies += 1;
        }

        initiativeOrder = SortTurnOrder(initiativeOrder, ourAllies, ourEnemies);
        outputTurnOrder(initiativeOrder);
        PerformCombat();
    }

    /// <summary>
    /// We must reference both static information (max hp) and current information (current hp)
    /// Therefore, we create a single PlayerCombatant class using this function which automatically merges the two.
    /// </summary>
    /// <param name="currentData"></param>
    /// <param name="usualData"></param>
    /// <returns></returns>
    PlayerCombatant createPlayerCombatant(CharacterSessionData currentData, PlayableData usualData)
    {
        PlayerCombatant toReturn = new PlayerCombatant(currentData.CharacterName, usualData.Hp,
            currentData.CurrentHp, usualData.Mp, currentData.CurrentMp, usualData.Attack,
            usualData.Defense, usualData.Speed, currentData.Deck);

        return toReturn;
    }

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
            instanceCombatState = CombatState.LOSE;
        }
        if (aliveEnemies == 0)
        {
            instanceCombatState = CombatState.WIN;
        }

        if (initiativeOrder[currentTurnPosition].isAlly == true)
        {
            instanceCombatState = CombatState.PLAYERTURN;
        }
        else
        {
            instanceCombatState = CombatState.ENEMYTURN;
        }
    }
    void EnemyTurn()
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

        // When the loop ends, we deal damage to the chosen ally

        ourAllies[initiativeOrder[lastAllyIndex].unitName].damage(ourEnemies[initiativeOrder[currentTurnPosition].unitName].attack);

        if (ourAllies[initiativeOrder[lastAllyIndex].unitName].isDead())
        {
            aliveAllies -= 1;
        }

        incrementTurnOrder();
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
                Debug.Log("Ally speed is " + ourAllies[combatant.unitName].speed);
            }
            else
            {
                Debug.Log("Enemy speed is " + ourEnemies[combatant.unitName].speed);
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
    List<InitiativeToken> SortTurnOrder(List<InitiativeToken> toSort, Dictionary<string, PlayerCombatant> AlliesInformation, Dictionary<string, EnemyCombatant> EnemiesInCombat)
    {
        // We return this list, and search the previous list to create a functional initiative list.
        List<InitiativeToken> toReturn = new List<InitiativeToken>();
        int originalCount = toSort.Count;

        // the list for sorting gets smaller, so we need a way to remember original list size
        for (int i = 0; i < originalCount; i++)
        {
            // Sets topSpeed to sort by.
            int topSpeed = -1;
            int topSpeedIndex = -1;

            for (int j = 0; j < toSort.Count; j++)
            {
                if (toSort[j].isAlly == true)
                {
                    if (AlliesInformation[toSort[j].unitName].speed > topSpeed)
                    {
                        topSpeed = AlliesInformation[toSort[j].unitName].speed;
                        topSpeedIndex = j;
                    }
                }
                else
                {
                    if (EnemiesInCombat[toSort[j].unitName].speed > topSpeed)
                    {
                        topSpeed = EnemiesInCombat[toSort[j].unitName].speed;
                        topSpeedIndex = j;
                    }
                }
            }

            // We add the highest speed to the list, so on and so forth
            toReturn.Add(toSort[topSpeedIndex]);
            toSort.RemoveAt(topSpeedIndex);
        }

        return toReturn;
    }

    /// <summary>
    /// For combat, initiative requires we know two things
    /// We need to know who goes when, and if the person going is an enemy or an ally
    /// Therefore, we have this very simply initiative token
    /// </summary>
    public class InitiativeToken
    {
        public string unitName { get; init; }
        public bool isAlly { get; init; }

        public InitiativeToken(string toName, bool toAlly)
        {
            this.unitName = toName;
            this.isAlly = toAlly;
        }
    }

    /// <summary>
    /// An individual has multiple choices of card to use;
    /// therefore, we take an integer input dependent on which
    /// button was pressed, then choose that card from the deck
    /// </summary>
    /// <param name="cardChosen"></param>
    public void CardButtonSelector(int cardChosen)
    {
        Debug.Log("I am working");
    }

    /// <summary>
    /// This function increments the turn position
    /// It also avoids dead units
    /// </summary>
    public void incrementTurnOrder()
    {
        if ((currentTurnPosition + 1) == initiativeOrder.Count)
        {
            currentTurnPosition = 0;
        }
        else
        {
            currentTurnPosition++;
        }

        // If we ever land on a dead enemy, they just get skipped.
        if (initiativeOrder[currentTurnPosition].isAlly == true)
        {
            if (ourAllies[initiativeOrder[currentTurnPosition].unitName].isDead())
            {
                incrementTurnOrder();
            }
        }
        else
        {
            if (ourEnemies[initiativeOrder[currentTurnPosition].unitName].isDead())
            {
                incrementTurnOrder();
            }
        }
    }
}
