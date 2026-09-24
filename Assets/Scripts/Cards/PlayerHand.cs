using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int startingHandSize = 2;
    [SerializeField] private DiscardPile discardPile;
    [SerializeField] private float cardHoldTime = .3f;
    [SerializeField] private ParticleSystem cardPlayVFXPrefab;

    private List<Card> cardsInHand = new List<Card>();

    private void Start()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawNextCard();
        }
    }

    private void OnEnable()
    {
        TurnEvents.OnPlayerTurnEnd += DisableHand;
        TurnEvents.OnPlayerTurnStart += EnableHand;
        PlayerEvents.OnDrawCardRequested += DrawNextCard;
        PlayerEvents.OnAttackComplete += EnableHand;
    }

    private void OnDisable()
    {
        TurnEvents.OnPlayerTurnEnd -= DisableHand;
        TurnEvents.OnPlayerTurnStart -= EnableHand;
        PlayerEvents.OnDrawCardRequested -= DrawNextCard;
        PlayerEvents.OnAttackComplete -= EnableHand;
    }

    private void DisableHand()
    {
        foreach (Card card in cardsInHand)
        {
            card.SetInteractable(false);
        }
    }

    private void EnableHand()
    {
        if (TurnSystem.Instance.HasActionsRemaining())
        {
            foreach (Card card in cardsInHand)
            {
                card.SetInteractable(true);
            }

        }

    }

    public void DrawNextCard()
    {
        if (cardSlots == null || cardsInHand.Count >= cardSlots.Length)
        {
            return;
        }

        CardData cardData = deck.DrawCard();

        if (cardData == null)
        {
            return;
        }

        int slotIndex = cardsInHand.Count;
        GameObject newCard = Instantiate(cardPrefab, cardSlots[slotIndex].position, Quaternion.identity);
        Card cardComponent = newCard.GetComponent<Card>();
        cardComponent.LoadCardData(cardData);
        cardsInHand.Add(cardComponent);
        cardsInHand[slotIndex].transform.SetParent(cardSlots[slotIndex]);

        if (!TurnSystem.Instance.HasActionsRemaining())
        {
            cardComponent.SetInteractable(false);
        }
    }

    private  IEnumerator PlayCardWithDelay(Card card)
    {
        DisableHand();
        card.SetIsPlaying(true);

        ParticleSystem cardPlayVFX = Instantiate(cardPlayVFXPrefab, card.transform.position, Quaternion.identity);
        cardPlayVFX.Play();
        Destroy(cardPlayVFX.gameObject, cardPlayVFX.main.duration);

        cardsInHand.Remove(card);
        discardPile.DiscardCard(card.GetCardData());
        
        PlayerEvents.CardPlayed(card.GetCardData());
        yield return new WaitForSeconds(cardHoldTime);

        Destroy(card.gameObject);
        RepositionCards();
    }

    public void PlayCard(Card card)
    {
        StartCoroutine(PlayCardWithDelay(card));
    }

    private void RepositionCards()
    {
        //Unparent each card from current slot
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.SetParent(null);
        }

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.SetParent(cardSlots[i]);
            cardsInHand[i].transform.position = cardSlots[i].position;
        }
    }
}
