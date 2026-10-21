using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCollection : MonoBehaviour
{
    [SerializeField] private List<CardData> availableCards;
    [SerializeField] private Transform[] cardSlots;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float cardScale = 1f;

    [Header("Paging")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private readonly List<GameObject> spawnedCards = new List<GameObject>();
    private int currentPage = 0;

    private int CardsPerPage => cardSlots.Length;
    private int PageCount => Mathf.Max(1, Mathf.CeilToInt((float)availableCards.Count / CardsPerPage));

    private void Start()
    {
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (previousButton != null) previousButton.onClick.AddListener(PreviousPage);

        if (DeckManager.Instance != null)
        {
            availableCards = DeckManager.Instance.GetCollection();
        }

        ShowPage(0);
    }

    public void NextPage()
    {
        ShowPage(currentPage + 1);
    }

    public void PreviousPage()
    {
        ShowPage(currentPage - 1);
    }

    private void ShowPage(int page)
    {
        currentPage = Mathf.Clamp(page, 0, PageCount - 1);
        ClearCards();

        int startIndex = currentPage * CardsPerPage;

        for (int slot = 0; slot < CardsPerPage; slot++)
        {
            int cardIndex = startIndex + slot;
            if (cardIndex >= availableCards.Count) break;

            SpawnCard(availableCards[cardIndex], cardSlots[slot]);
        }

        if (previousButton != null) previousButton.interactable = currentPage > 0;
        if (nextButton != null) nextButton.interactable = currentPage < PageCount - 1;
    }

    private void SpawnCard(CardData data, Transform slot)
    {
        GameObject card = Instantiate(cardPrefab, slot.position, Quaternion.identity);
        card.transform.localScale = cardPrefab.transform.localScale * cardScale;
        card.transform.SetParent(slot);
        card.GetComponent<Card>().LoadCardData(data);

        spawnedCards.Add(card);
    }

    private void ClearCards()
    {
        foreach (GameObject card in spawnedCards)
        {
            if (card != null) Destroy(card);
        }

        spawnedCards.Clear();
    }
}