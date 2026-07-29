using System;
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
    public void DisplayCard(CardByte card, bool isFocused = false, bool magicEffcient = false, bool magicIneffcient = false)
    {
        nameText.text = card.Name;
        typeText.text = card.Type;

        if (card.StaticData != null)
        {
            Double finalCost = card.StaticData.Cost;
            if (magicEffcient)
            {
                finalCost = Math.Floor(finalCost/2);
            }

            if (magicIneffcient)
            {
                if (finalCost == 0)
                {
                    finalCost = 1;
                }
                else
                {
                    finalCost *= 2;
                }
            }
            int finalCostInt = (int)finalCost;
            costText.text = finalCostInt.ToString();
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

        string fullSpriteKey;

        if (baseSpriteName == "Light_Card_2_" || baseSpriteName == "Light_Card_5_")
        {
            fullSpriteKey = $"{baseSpriteName}{(isFocused ? "15" : "14")}";
        }
        else
        {
            // Builds the exact sprite name in the atlas (e.g., "Attacks_Card_3_1" or "Attacks_Card_3_0")
            fullSpriteKey = $"{baseSpriteName}{(isFocused ? "1" : "0")}";
        }
        

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