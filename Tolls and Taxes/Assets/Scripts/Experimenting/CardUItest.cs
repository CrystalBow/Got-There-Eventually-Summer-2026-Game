using UnityEngine;

public class CardUItest : MonoBehaviour
{
    [SerializeField] private CardUI cardUiInstance;

    private void Start()
    {
        // Simulate generating a dummy CardByte with fake data to test look & feel
        CardData fakeData = new CardData { Cost = 3, Damage = 15, Description = "Big Smash" };
        CardByte testCard = CardByte.Create("Heavy Strike", "Attack", fakeData);

        // Update the UI
        cardUiInstance.DisplayCard(testCard);
    }
}
