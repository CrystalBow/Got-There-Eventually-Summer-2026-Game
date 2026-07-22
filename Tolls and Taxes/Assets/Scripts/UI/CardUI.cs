using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.U2D; // Needed for SpriteAtlas

public class CardUI : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text costText;
    public Image cardBackground;
    

    
    
    
    [Header("Sprite Atlas Reference")]
    [SerializeField] private SpriteAtlas cardAtlas;
    /// <summary>
    /// Updates the text and background sprite of this UI container.
    /// </summary>
    public void DisplayCard(CardByte card, bool isFocused = false)
    {
        nameText.text = card.Name;
        typeText.text = card.Type;

        if (card.StaticData != null)
        {
            costText.text = card.StaticData.Cost.ToString();
        }
        else
        {
            costText.text = "0-0"; 
        }

        // Set the background sprite based on focus state
        SetCardSprite(card.SpriteName, isFocused);
    }

    /// <summary>
    /// Swaps the card background sprite when selection changes.
    /// </summary>
    public void SetFocusState(CardByte card, bool isFocused)
    {
        SetCardSprite(card.SpriteName, isFocused);
    }

    private void SetCardSprite(string baseSpriteName, bool isFocused)
    {
        if (string.IsNullOrEmpty(baseSpriteName) || cardBackground == null || cardAtlas == null) 
            return;

        // Builds the exact sprite name in the atlas (e.g., "Attacks_Card_3_1" or "Attacks_Card_3_0")
        string fullSpriteKey = $"{baseSpriteName}{(isFocused ? "1" : "0")}";

        // Grabs sprite directly from the atlas by key
        Sprite loadedSprite = cardAtlas.GetSprite(fullSpriteKey);

        if (loadedSprite != null)
        {
            cardBackground.sprite = loadedSprite;
        }
        else
        {
            Debug.LogWarning($"[CardUI] Sprite '{fullSpriteKey}' was not found inside CardSpriteAtlas!");
        }
    }
}