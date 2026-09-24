using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private GameObject cardTabPrefab;

    private List<GameObject> cardTabGameObjects = new List<GameObject>();

    private const float VERTICAL_SPACING = 0.65f;

    private void Start()
    {
        BuildUI();
    }

    private void OnEnable()
    {
        DeckEvents.OnDeckProcessed += BuildUI;
    }
    private void OnDisable()
    {
        DeckEvents.OnDeckProcessed -= BuildUI;
    }

    private void BuildUI()
    {
        foreach (GameObject cardTab in cardTabGameObjects)
        {
            Debug.Log(cardTab);
            Destroy(cardTab);
        }

        cardTabGameObjects.Clear();

        List<CardData> deck = DeckManager.Instance.GetDeck();

        for (int i = 0; i < deck.Count; i++)
        {
            GameObject cardTab = Instantiate(cardTabPrefab, transform);
            cardTab.GetComponent<CardTab>().LoadCardTabData(deck[i]);
            cardTab.transform.localPosition = new Vector3(0f, -i * VERTICAL_SPACING, 0f);
            cardTabGameObjects.Add(cardTab);
        }
    }

}
