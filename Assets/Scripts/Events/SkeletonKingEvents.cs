using System;
using UnityEngine;

public class SkeletonKingEvents : MonoBehaviour
{
    public static event Action<CardData> OnSkeletonKingHit;

    public static event Action OnSkeletonKingDeath;

    public static void SkeletonKingHit(CardData cardData)
    {
        OnSkeletonKingHit?.Invoke(cardData);
    }

    public static void SkeletonKingDeath()
    {
        OnSkeletonKingDeath?.Invoke();
    }
}
