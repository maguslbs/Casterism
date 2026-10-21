using System.Collections.Generic;
using UnityEngine;

public class DeckManager : Singleton<DeckManager>
{
    [SerializeField] private int maxDeckSize = 8;
    [SerializeField] private int minDeckSize = 1;
    [SerializeField] private DefaultDeck defaultDeck;
    [SerializeField] private List<CardData> startingCollection = new List<CardData>();

    private List<CardData> currentDeck = new List<CardData>();
    private readonly List<CardData> collection = new List<CardData>();

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        DontDestroyOnLoad(gameObject);
        ResetForNewRun();
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

    public void ResetForNewRun()
    {
        currentDeck = new List<CardData>(defaultDeck.cards);
        collection.Clear();
        collection.AddRange(startingCollection);
        DeckEvents.DeckProcessed();
    }

    public List<CardData> GetDeck()
    {
        return new List<CardData>(currentDeck);
    }

    public List<CardData> GetCollection()
    {
        return new List<CardData>(collection);
    }

    public bool IsUnlocked(CardData card)
    {
        return collection.Contains(card);
    }

    public void UnlockCard(CardData card)
    {
        if (card == null || collection.Contains(card)) return;
        collection.Add(card);
    }

    private void AddCard(CardData card)
    {
        if (currentDeck.Count >= maxDeckSize)
        {
            MessageDisplay.Show($"Deck full (max {maxDeckSize} cards)");
            return;
        }

        currentDeck.Add(card);
        DeckEvents.DeckProcessed();
    }

    private void RemoveCard(CardData card)
    {
        if (currentDeck.Count <= minDeckSize)
        {
            MessageDisplay.Show($"Deck at least {minDeckSize} cards");
            return;
        }

        currentDeck.Remove(card);
        DeckEvents.DeckProcessed();
    }
}