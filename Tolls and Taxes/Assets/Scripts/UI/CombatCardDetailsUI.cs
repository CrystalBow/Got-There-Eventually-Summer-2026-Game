using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatCardDetailsUI : MonoBehaviour
{
    [SerializeField] private GameObject detailsPanel;

    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardTypeText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private TMP_Text effectsText;
    [SerializeField] private TMP_Text descriptionText;

    private CombatCenter combatCenter;
    private CardByte displayedCard;

    private void Awake()
    {
        if (detailsPanel != null)
        {
            detailsPanel.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (combatCenter == null)
        {
            combatCenter =
                FindFirstObjectByType<CombatCenter>();
        }

        if (combatCenter == null)
        {
            HideDetails();
            return;
        }

        CardByte selectedCard = GetSelectedCard();

        if (selectedCard == null ||
            selectedCard.StaticData == null)
        {
            HideDetails();
            return;
        }

        if (selectedCard != displayedCard ||
            !detailsPanel.activeSelf)
        {
            DisplayCard(selectedCard);
        }
    }

    private CardByte GetSelectedCard()
    {
        if (combatCenter.CurrentState is PlayerTurn playerTurn)
        {
            if (playerTurn.combatant == null ||
                playerTurn.combatant.Deck == null)
            {
                return null;
            }

            IReadOnlyList<CardByte> hand =
                playerTurn.combatant.Deck.HandCards;

            int index = playerTurn.chosenCardIndex;

            if (hand == null ||
                index < 0 ||
                index >= hand.Count)
            {
                return null;
            }

            return hand[index];
        }

        // Keep showing the selected card while choosing a target.
        if (combatCenter.CurrentState is CardHandler cardHandler)
        {
            return cardHandler.currentCard;
        }

        return null;
    }

    private void DisplayCard(CardByte card)
    {
        displayedCard = card;
        CardData data = card.StaticData;

        detailsPanel.SetActive(true);

        cardNameText.text =
            string.IsNullOrWhiteSpace(card.Name)
                ? "UNNAMED CARD"
                : card.Name.ToUpperInvariant();

        cardTypeText.text =
            string.IsNullOrWhiteSpace(card.Type)
                ? "UNKNOWN TYPE"
                : card.Type.ToUpperInvariant();

        costText.text = $"MP COST: {data.Cost}";

        if (data.Damage > 0)
        {
            valueText.text =
                $"BASE DAMAGE: {data.Damage}";
        }
        else if (data.Damage < 0)
        {
            valueText.text =
                $"HEALING: {Mathf.Abs(data.Damage)}";
        }
        else
        {
            valueText.text = "DAMAGE / HEALING: --";
        }

        durationText.text =
            data.Time > 0
                ? $"DURATION: {data.Time}"
                : "DURATION: --";

        effectsText.text =
            BuildEffectsText(data.Effects);

        descriptionText.text =
            string.IsNullOrWhiteSpace(data.Description)
                ? "No description available."
                : data.Description;
    }

    private string BuildEffectsText(List<int> effectIds)
    {
        if (effectIds == null ||
            effectIds.Count == 0 ||
            DataCenter.Instance == null)
        {
            return "EFFECTS: NONE";
        }

        List<string> effectNames =
            DataCenter.Instance.GetEffectNamesForCard(effectIds);

        return effectNames.Count == 0
            ? "EFFECTS: NONE"
            : $"EFFECTS: {string.Join(", ", effectNames)}";
    }

    private void HideDetails()
    {
        displayedCard = null;

        if (detailsPanel != null)
        {
            detailsPanel.SetActive(false);
        }
    }
}