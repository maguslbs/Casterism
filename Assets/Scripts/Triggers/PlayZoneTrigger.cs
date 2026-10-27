using UnityEngine;

public class PlayZoneTrigger : MonoBehaviour
{
    [SerializeField] private PlayerHand playerHand;
    [SerializeField] private GameObject playZoneVisual;   // BARU

    private void Start()   // BARU
    {
        playZoneVisual.SetActive(false);
    }

    private void OnEnable()   // BARU
    {
        PlayerEvents.OnCardDragStarted += ShowVisual;
        PlayerEvents.OnCardDragEnded += HideVisual;
        PlayerEvents.OnCardPlayed += HideOnCardPlayed;
    }

    private void OnDisable()   // BARU
    {
        PlayerEvents.OnCardDragStarted -= ShowVisual;
        PlayerEvents.OnCardDragEnded -= HideVisual;
        PlayerEvents.OnCardPlayed -= HideOnCardPlayed;
    }

    private void ShowVisual() => playZoneVisual.SetActive(true);              // BARU
    private void HideVisual() => playZoneVisual.SetActive(false);             // BARU
    private void HideOnCardPlayed(CardData cardData) => HideVisual();         // BARU

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