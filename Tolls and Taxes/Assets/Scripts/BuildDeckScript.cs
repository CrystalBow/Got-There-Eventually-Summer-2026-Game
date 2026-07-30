using UnityEngine;

public class BuildDeckScript : MonoBehaviour
{
    public static BuildDeckScript Instance { get; private set; }
    Deck HealerDeck;
    Deck AttackerDeck;
    Deck DefenderDeck;
    
    
    void Start()
    {
        while (TransferCenter.Instance == null || DataCenter.Instance == null)
        {
            
        }
        
        

        if (Instance == null)
        {
            BuildDeckScript.Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        int Level = 1;
        // Deck Builds
        Deck currDeck = new Deck();
        //Healer (Samantha Pel)
        PlayableData currAlly = DataCenter.Instance.Allies["Samantha Pel"];
        foreach (string cardName in currAlly.SpecialtyCards.Attacks.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Attacks",currAlly.SpecialtyCards.Attacks[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Buffs.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Buffs",currAlly.SpecialtyCards.Buffs[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Debuffs.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Debuffs",currAlly.SpecialtyCards.Debuffs[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Healing.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Healing",currAlly.SpecialtyCards.Healing[cardName], true));
        }
        currDeck.AddCard(CardByte.Create("Accelerate","Buffs",DataCenter.Instance.GlobalCards.Buffs["Accelerate"]));
        currDeck.AddCard(CardByte.Create("Force Bolt Barrage","Attacks",DataCenter.Instance.GlobalCards.Attacks["Force Bolt Barrage"]));
        currDeck.AddCard(CardByte.Create("Force Bolt Barrage","Attacks",DataCenter.Instance.GlobalCards.Attacks["Force Bolt Barrage"]));
        currDeck.AddCard(CardByte.Create("First Aid","Healing", DataCenter.Instance.GlobalCards.Healing["First Aid"]));
        currDeck.AddCard(CardByte.Create("First Aid","Healing", DataCenter.Instance.GlobalCards.Healing["First Aid"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        HealerDeck = currDeck;
        TransferCenter.Instance.SaveCharacterState("Samantha Pel", HealerDeck, DataCenter.Instance.maxHealthCalculation(currAlly, Level), DataCenter.Instance.maxManaCalculation(currAlly, Level),level:Level,0);
        
        
        //Defender (John Goblinus)
        currDeck = new Deck();
        currAlly = DataCenter.Instance.Allies["John Goblinus"];
        foreach (string cardName in currAlly.SpecialtyCards.Attacks.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Attacks",currAlly.SpecialtyCards.Attacks[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Buffs.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Buffs",currAlly.SpecialtyCards.Buffs[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Debuffs.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Debuffs",currAlly.SpecialtyCards.Debuffs[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Healing.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Healing",currAlly.SpecialtyCards.Healing[cardName], true));
        }
        currDeck.AddCard(CardByte.Create("Accelerate","Buffs",DataCenter.Instance.GlobalCards.Buffs["Accelerate"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("First Aid","Healing", DataCenter.Instance.GlobalCards.Healing["First Aid"]));
        currDeck.AddCard(CardByte.Create("Sharpen Steel","Buffs", DataCenter.Instance.GlobalCards.Buffs["Sharpen Steel"]));
        currDeck.AddCard(CardByte.Create("Reckless Attack", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Reckless Attack"]));
        
        
        DefenderDeck = currDeck;
        TransferCenter.Instance.SaveCharacterState("John Goblinus",DefenderDeck,DataCenter.Instance.maxHealthCalculation(currAlly, Level), DataCenter.Instance.maxManaCalculation(currAlly, Level),Level, 0);
        
        
        //Attacker (Marvin Bold)
        currDeck = new Deck();
        currAlly = DataCenter.Instance.Allies["Marvin Bold"];
        foreach (string cardName in currAlly.SpecialtyCards.Attacks.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Attacks",currAlly.SpecialtyCards.Attacks[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Buffs.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Buffs",currAlly.SpecialtyCards.Buffs[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Debuffs.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Debuffs",currAlly.SpecialtyCards.Debuffs[cardName], true));
        }
        foreach (string cardName in currAlly.SpecialtyCards.Healing.Keys)
        {
            currDeck.AddCard(CardByte.Create(cardName, "Healing",currAlly.SpecialtyCards.Healing[cardName], true));
        }
        
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("Attack!", "Attacks", DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        currDeck.AddCard(CardByte.Create("First Aid","Healing", DataCenter.Instance.GlobalCards.Healing["First Aid"]));
        currDeck.AddCard(CardByte.Create("Growl","Debuffs", DataCenter.Instance.GlobalCards.Debuffs["Growl"]));
        AttackerDeck = currDeck;
        TransferCenter.Instance.SaveCharacterState("Marvin Bold",AttackerDeck,DataCenter.Instance.maxHealthCalculation(currAlly,Level), DataCenter.Instance.maxManaCalculation(currAlly,Level),Level,0);
        currDeck = new Deck();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // This ensures that whenever the game is run, this object will be instantiated.
    // This way we don't need to cluttered our scenes with the game object every single scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeScene()
    {
        GameObject g = new GameObject("Generated_BuildDeckScript");
        g.AddComponent<BuildDeckScript>();
    }
    
}
