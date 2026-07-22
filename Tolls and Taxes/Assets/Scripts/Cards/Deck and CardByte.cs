using UnityEngine;
using System.Collections.Generic;

public class Deck
{
    // Actual references
    private readonly List<CardByte> deck = new List<CardByte>();
    private readonly List<CardByte> hand = new List<CardByte>();
    private readonly List<CardByte> discard = new List<CardByte>();

    // read-only views
    public IReadOnlyList<CardByte> DeckCards => deck;
    public IReadOnlyList<CardByte> HandCards => hand;
    public IReadOnlyList<CardByte> DiscardCards => discard;

    public void AddCard(CardByte card)
    {
        deck.Add(card);
    }

    public void RemoveCard(CardByte card)
    {
        deck.Remove(card);
        hand.Remove(card);
        discard.Remove(card);
    }

    public void DrawCard()
    {
        if (deck.Count == 0) return;
        
        // Removing from the back is faster because memory doesn't need to move around.
        int lastIndex = deck.Count - 1;
        hand.Add(deck[lastIndex]);
        deck.RemoveAt(lastIndex);
    }
    
    public void DiscardCard(CardByte card)
    {
        if (hand.Remove(card))
        {
            discard.Add(card);
        }
    }

    public void DrawHand(int count = 3)
    {
        for (int i = 0; i < count; i++)
        {
            DrawCard();
        }
    }
    
    public void DiscardHand()
    {
        discard.AddRange(hand);
        hand.Clear();
    }

    /// <summary>
    /// Shuffles the deck
    /// </summary>
    /// <remarks>
    /// Uses the Knuth Shuffle to make each possible arrangement equally likely.
    /// </remarks>
    public void Shuffle()
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            CardByte value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }
    
    /// <summary>
    /// Puts all cards back in the deck and shuffles them.
    /// </summary>
    public void ResetAndShuffle()
    {
        DiscardHand();
        deck.AddRange(discard);
        discard.Clear();
        Shuffle();
    }
}
/// <summary>
/// A container for card information.
/// </summary>
[System.Serializable]
public class CardByte
{
    // Basic info.
    public bool isSpecialty;
    public string Name;
    public string Type;
    /// <summary>
    /// String with the sprite name. add 0 for default and 1 for selected.
    /// </summary>
    public string SpriteName {get; private set;}
    
    // Reference to the Data in DataCenter. Can't be edited
    [System.NonSerialized] 
    public CardData StaticData;
    
    /// <summary>
    /// Factory method for building a CardByte.
    /// </summary>
    /// <param name="cardName">Name of Card</param>
    /// <param name="cardType">Type of Card</param>
    /// <param name="staticData">Reference CardData in DataCenter</param>
    /// <param name="isSpecialty">Weather it is a specialty card or not</param>
    /// <returns></returns>
    public static CardByte Create(string cardName, string cardType, CardData staticData, bool isSpecialty = false)
    {
        CardByte newCard = new CardByte
        {
            Name = cardName,
            Type = cardType,
            isSpecialty = isSpecialty,
            StaticData = staticData
        };
        newCard.GenerateSpriteName();
        return newCard;
    }
    
    /// <summary>
    /// Generates the sprite name.
    /// </summary>
    private void GenerateSpriteName()
    {
        //Variable Set up
        SpriteName = "";
        bool altColor = false;
        //Checks type and if it has cost for base.
        if (Type == "Attacks")
        {
            if (StaticData.Cost <= 0)
            {
                SpriteName += "Attack_Card";
            }
            else
            {
                SpriteName += "Thunder_Card";
            }
        } else if (Type == "Buffs")
        {
            altColor = true;
            if (StaticData.Cost > 0)
            {
                SpriteName += "Light_Card";
            }
            else
            {
                SpriteName += "Sun_Card";
            }
        }
        else if (Type == "Debuffs")
        {
            altColor = true;
            if (StaticData.Cost <= 0)
            {
                SpriteName += "Moon_Card";
            }
            else
            {
                SpriteName += "Void_Card";
            }
        }
        else
        {
            //Healing cards
            if (StaticData.Cost > 0)
            {
                SpriteName += "Charm_Card";
            }
            else
            {
                SpriteName += "Heal_Card";
            }
        }
        
        // Checks specialty and alt color for color value
        if (isSpecialty)
        {
            if (altColor)
            {
                SpriteName += "_5_";
            }
            else
            {
                SpriteName += "_4_";
            }
        }
        else
        {
            if (altColor)
            {
                SpriteName += "_2_";
            }
            else
            {
                SpriteName += "_3_";
            }
        }
    }
}
