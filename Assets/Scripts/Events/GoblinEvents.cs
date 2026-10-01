using System;
using UnityEngine;

public class GoblinEvents : MonoBehaviour
{
    public static event Action<CardData> OnGoblinHit;

    public static event Action OnGoblinDeath;

    public static void GoblinHit(CardData cardData)
    {
        OnGoblinHit?.Invoke(cardData);
    }

    public static void GoblinDeath()
    {
        OnGoblinDeath?.Invoke();
    }
}
