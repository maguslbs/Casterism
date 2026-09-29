using System;
using UnityEngine;

public class SkeletonSoldierEvents : MonoBehaviour
{
    public static event Action<CardData> OnSkSoldierHit;

    public static event Action OnSkSoldierDeath;

    public static void SkSoldierHit(CardData cardData)
    {
        OnSkSoldierHit?.Invoke(cardData);
    }

    public static void SkSoldierDeath()
    {
        OnSkSoldierDeath?.Invoke();
    }
}
