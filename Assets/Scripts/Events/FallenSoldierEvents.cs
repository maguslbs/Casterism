using System;
using UnityEngine;

public class FallenSoldierEvents : MonoBehaviour
{
    public static event Action<CardData> OnFlSoldierHit;

    public static event Action OnFlSoldierDeath;

    public static void FlSoldierHit(CardData cardData)
    {
        OnFlSoldierHit?.Invoke(cardData);
    }

    public static void FlSoldierDeath()
    {
        OnFlSoldierDeath?.Invoke();
    }
}
