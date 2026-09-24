using UnityEngine;
using System;

public class DeckEvents
{
    public static event Action<CardData> OnRemoveCardFromDeck;

    public static event Action<CardData> OnAddCardToDeck;

    public static event Action OnDeckProcessed;

    public static void RemoveCardFromDeck(CardData card)
    {
        OnRemoveCardFromDeck?.Invoke(card);
    }

    public static void AddCardToDeck(CardData card)
    {
        OnAddCardToDeck?.Invoke(card);
    }

    public static void DeckProcessed()
    {
        OnDeckProcessed?.Invoke();
    }
}
