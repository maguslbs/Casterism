using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : Singleton<DeckManager>
{
    private List<CardData> currentDeck = new List<CardData>();
    [SerializeField] private int maxDeckSize = 9;

    [SerializeField] private DefaultDeck defaultDeck;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        currentDeck = new List<CardData>(defaultDeck.cards);
    }

    private void OnEnable()
    {
        DeckEvents.OnAddCardToDeck += AddCard;
        DeckEvents.OnRemoveCardFromDeck += RemoveCard;
    }

    private void OnDisable()
    {
        DeckEvents.OnAddCardToDeck -= AddCard;
        DeckEvents.OnRemoveCardFromDeck -= RemoveCard;
    }

    private void AddCard(CardData card)
    {
        if (currentDeck.Count >= maxDeckSize)
        {
            Debug.Log("Deck full");
            return;
        }
        currentDeck.Add(card);
        DeckEvents.DeckProcessed();
    }

    private void RemoveCard(CardData card)
    {
        currentDeck.Remove(card);
        DeckEvents.DeckProcessed();
    }

    public List<CardData> GetDeck()
    {
        return new List<CardData>(currentDeck);
    }
}
