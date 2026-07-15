using UnityEngine;

public class BuildDeckScript : MonoBehaviour
{
    
    void Start()
    {
        while (TransferCenter.Instance == null || DataCenter.Instance == null)
        {
            
        }
        Deck FirstDeck = new Deck();
        FirstDeck.AddCard(CardByte.Create("Momentum Strike", "Attacks", DataCenter.Instance.Allies["Samantha Pel"].SpecialtyCards.Attacks["Momentum Strike"], true));
        FirstDeck.AddCard(CardByte.Create("Sample Super Strike!","Attacks", DataCenter.Instance.Allies["Samantha Pel"].SpecialtyCards.Attacks["Sample Super Strike!"], true));
        FirstDeck.AddCard(CardByte.Create("First Aid", "Healing", DataCenter.Instance.Allies["Samantha Pel"].SpecialtyCards.Healing["First Aid"], true));
        FirstDeck.AddCard(CardByte.Create("Attack!","Attacks",DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        FirstDeck.AddCard(CardByte.Create("Attack!","Attacks",DataCenter.Instance.GlobalCards.Attacks["Attack!"]));
        FirstDeck.AddCard(CardByte.Create("Force Bolt Barrage","Attacks", DataCenter.Instance.GlobalCards.Attacks["Force Bolt Barrage"]));
        TransferCenter.Instance.SaveCharacterState("Samantha Pel", FirstDeck, DataCenter.Instance.Allies["Samantha Pel"].Hp, DataCenter.Instance.Allies["Samantha Pel"].Mp);
        Destroy(this.gameObject);
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
