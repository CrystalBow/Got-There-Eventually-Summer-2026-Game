using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text costText;
    public Image cardBackground; // Useful if you want to change colors based on Type later!

    /// <summary>
    /// Updates the text elements of this specific card UI container using runtime data.
    /// </summary>
    public void DisplayCard(CardByte card)
    {
        // 1. Assign basic string properties straight from CardByte
        nameText.text = card.Name;
        typeText.text = card.Type;

        // 2. Safely dig into the immutable StaticData reference we linked earlier
        if (card.StaticData != null)
        {
            costText.text = card.StaticData.Cost.ToString();
        }
        else
        {
            // Fallback in case static database data wasn't assigned to this CardByte instance
            costText.text = "0"; 
        }
    }
}
