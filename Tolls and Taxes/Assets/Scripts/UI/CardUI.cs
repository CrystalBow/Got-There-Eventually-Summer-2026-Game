using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text costText;
    public Image cardBackground;

    /// <summary>
    /// Updates the text elements of this specific card UI container using runtime data.
    /// </summary>
    public void DisplayCard(CardByte card)
    {
        // Assign basic string properties straight from CardByte
        nameText.text = card.Name;
        typeText.text = card.Type;

        // Safely dig into the immutable StaticData
        if (card.StaticData != null)
        {
            costText.text = card.StaticData.Cost.ToString();
        }
        else
        {
            // Error
            costText.text = "0-0"; 
        }
    }
}
