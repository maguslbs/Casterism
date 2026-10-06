using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static event Action OnGameOver;

    public static void GameOver()
    {
        OnGameOver?.Invoke();
    }
}
