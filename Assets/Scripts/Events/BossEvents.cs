using System;
using UnityEngine;

public class BossEvents
{
    public static event Action<CardData> OnBossHit;

    public static event Action OnBossDeath;

    public static void BossHit(CardData cardData)
    {
        OnBossHit?.Invoke(cardData);
    }

    public static void BossDeath()
    {
        OnBossDeath?.Invoke();
    }
}
