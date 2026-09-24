using UnityEngine;

public class PlayZoneTrigger : MonoBehaviour
{
    [SerializeField] private PlayerHand playerHand;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Card card))
        {
            playerHand.PlayCard(card);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Card card))
        {

        }
    }
}
