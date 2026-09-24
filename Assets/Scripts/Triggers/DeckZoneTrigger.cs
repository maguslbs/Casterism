using UnityEngine;

public class DeckZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Card card))
        {
            DeckEvents.AddCardToDeck(card.GetCardData());
            Debug.Log("Added card to deck");
        }
    }
}
