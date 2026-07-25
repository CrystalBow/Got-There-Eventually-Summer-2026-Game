using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckViewOverLay : MonoBehaviour
{
    
    [SerializeField]public List<CardUI> Cards;
    [SerializeField]public GameObject PartyPanel;
    [SerializeField]public TextMeshProUGUI CardName;
    [SerializeField]public TextMeshProUGUI CardCost;
    [SerializeField]public TextMeshProUGUI CardDescription;
    [SerializeField] public GameObject IconCloner;

}
