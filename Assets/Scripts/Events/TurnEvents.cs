using UnityEngine;
using System;

public static class TurnEvents
{
    public static event Action OnPlayerTurnStart;
    public static event Action OnPlayerTurnEnd;
    public static event Action OnBossTurnStart;
    public static event Action OnBossTurnEnd;
    public static event Action OnFallenSoldierTurnStart;
    public static event Action OnFallenSoldierTurnEnd;
    public static event Action OnGoblinTurnStart;
    public static event Action OnGoblinTurnEnd;
    public static event Action OnSkeletonKingTurnStart;
    public static event Action OnSkeletonKingTurnEnd;

    public static void PlayerTurnStart()
    {
        OnPlayerTurnStart?.Invoke();
    }

    public static void PlayerTurnEnd()
    {
        OnPlayerTurnEnd?.Invoke();
    }

    public static void EnemyTurnStart()
    {
        OnBossTurnStart?.Invoke();
    }

    public static void EnemyTurnEnd()
    {
        OnBossTurnEnd?.Invoke();
    }
    public static void FallenSoldierTurnStart()
    {
        OnFallenSoldierTurnStart?.Invoke();
    }

    public static void FallenSoldierTurnEnd()
    {
        OnFallenSoldierTurnEnd?.Invoke();
    }

    public static void GoblinTurnStart()
    {
        OnGoblinTurnStart?.Invoke();
    }

    public static void GoblinTurnEnd()
    {
        OnGoblinTurnEnd?.Invoke();
    }

    public static void SkeletonKingStart()
    {
        OnSkeletonKingTurnStart?.Invoke();
    }

    public static void SkeletonKingEnd()
    {
        OnSkeletonKingTurnEnd?.Invoke();
    }
}
