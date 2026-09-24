using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DiscardPile : MonoBehaviour
{
    [SerializeField] private List<CardData> discardPile = new List<CardData>();
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Deck deck;

    private const float VERTICAL_SPACING = .25f;

    public void DiscardCard(CardData cardData)
    {
        discardPile.Add(cardData);

        GameObject discardedCard = Instantiate(cardPrefab, transform);

        discardedCard.GetComponent<Card>().LoadCardData(cardData);

        discardedCard.GetComponent<Card>().SetInteractable(false);

        SortingGroup sortingGroup = discardedCard.GetComponent<SortingGroup>();
        sortingGroup.sortingOrder = discardPile.Count - 1;

        discardedCard.transform.SetParent(transform);

        discardedCard.transform.localPosition = new Vector3(0f, (discardPile.Count - 1) * -VERTICAL_SPACING, 0f);
    }

    public void MoveCardsToDeck(List<CardData> drawpPile)
    {
        if (drawpPile == null || discardPile.Count == 0)
            return;

        drawpPile.AddRange(discardPile);
        ClearPile();
    }

    private void ClearPile()
    {
        discardPile.Clear();
        foreach (Transform discardedCard in transform)
        {
            Destroy(discardedCard.gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (!TurnSystem.Instance.HasActionsRemaining() || discardPile.Count <= 0)
        {
            return;
        }

        PlayerEvents.ReshuffleRequested();
        deck.ReshuffleFromDiscardPile();
    }

}
