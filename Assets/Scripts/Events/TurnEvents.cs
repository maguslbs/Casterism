using UnityEngine;
using System;

public static class TurnEvents
{
    public static event Action OnPlayerTurnStart;
    public static event Action OnPlayerTurnEnd;
    public static event Action OnBossTurnStart;
    public static event Action OnBossTurnEnd;
    public static event Action OnSkSoldierTurnStart;
    public static event Action OnSkSoldierTurnEnd;

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
    public static void SkSoldierTurnStart()
    {
        OnSkSoldierTurnStart?.Invoke();
    }

    public static void SkSoldierTurnEnd()
    {
        OnSkSoldierTurnEnd?.Invoke();
    }
}
