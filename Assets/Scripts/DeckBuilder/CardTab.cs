using TMPro;
using UnityEngine;

public class CardTab : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cardBase;
    [SerializeField] private SpriteRenderer illustration;
    [SerializeField] private TextMeshPro cardNameText;
    [SerializeField] private TextMeshPro actionsText;

    private CardData cardData;

    private Color originalColor;

    private void Start()
    {
        originalColor = cardBase.color;
    }
    public void LoadCardTabData(CardData cardData)
    {
        this.cardData = cardData;
        illustration.sprite = cardData.illustration;
        cardNameText.text = cardData.cardName;
        actionsText.text = cardData.actionCost.ToString();
    }

    private void OnMouseDown()
    {
        DeckEvents.RemoveCardFromDeck(cardData);
    }

    private void OnMouseEnter()
    {
        cardBase.color = Color.yellowGreen;
    }

    private void OnMouseExit()
    {
        cardBase.color = originalColor;
    }
}
