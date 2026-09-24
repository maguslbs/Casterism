using UnityEngine;
using System;

public static class TurnEvents
{
    public static event Action OnPlayerTurnStart;
    public static event Action OnPlayerTurnEnd;
    public static event Action OnBossTurnStart;
    public static event Action OnBossTurnEnd;

    public static void PlayerTurnStart()
    {
        OnPlayerTurnStart?.Invoke();
    }

    public static void PlayerTurnEnd()
    {
        OnPlayerTurnEnd?.Invoke();
    }

    public static void BossTurnStart()
    {
        OnBossTurnStart?.Invoke();
    }

    public static void BossTurnEnd()
    {
        OnBossTurnEnd?.Invoke();
    }
}
