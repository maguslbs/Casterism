using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    [SerializeField] private List<CardData> availableCards;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private GameObject cardPrefab;

    private void Start()
    {
        for (int i = 0; i < availableCards.Count; i++)
        {
            AddCardToColection(i);
        }
    }

    private void AddCardToColection(int cardIndex)
    {
        GameObject card = Instantiate(cardPrefab, cardSlots[cardIndex].position, Quaternion.identity);
        Card cardComponent = card.GetComponent<Card>();
        cardComponent.LoadCardData(availableCards[cardIndex]);
        card.transform.SetParent(cardSlots[cardIndex].transform);
    }
}
