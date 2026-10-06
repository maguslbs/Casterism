using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSystem : Singleton<TurnSystem>
{
    private enum TurnState { PlayerTurn, EnemyTurn }
    private TurnState currentTurn = TurnState.PlayerTurn;

    [SerializeField] private int maxActionsPerTurn = 1;
    [SerializeField] private int drawCost = 1;
    [SerializeField] private int reshuffleCost = 3;
    [SerializeField] private TextMeshProUGUI remainingActionsText;
    [SerializeField] private int turnWaitTime = 3;
    [SerializeField] private float enemyDelayTime = 2f;

    [SerializeField] private TextMeshProUGUI displayTurnState;

    private int actionsRemaining;
    private string currentEnemyName = "Name";   // sementara, lihat catatan di bawah

    private void Start()
    {
        displayTurnState.text = "Player's Turn!";
        StartPlayerTurn();
    }

    private void OnEnable()
    {
        PlayerEvents.OnDrawCardRequested += DrawRequested;
        PlayerEvents.OnReshuffleRequested += ReshuffleRequested;
        PlayerEvents.OnCardPlayed += CardPlayed;
        BossEvents.OnBossDeath += ClearTurnDisplay;
        FallenSoldierEvents.OnFlSoldierDeath += ClearTurnDisplay;
        PlayerEvents.OnPlayerDeath += ClearTurnDisplay;
    }

    private void OnDisable()
    {
        PlayerEvents.OnDrawCardRequested -= DrawRequested;
        PlayerEvents.OnReshuffleRequested -= ReshuffleRequested;
        PlayerEvents.OnCardPlayed -= CardPlayed;
        BossEvents.OnBossDeath -= ClearTurnDisplay;
        FallenSoldierEvents.OnFlSoldierDeath -= ClearTurnDisplay;
        PlayerEvents.OnPlayerDeath -= ClearTurnDisplay;
    }

    private void StartPlayerTurn()
    {
        currentTurn = TurnState.PlayerTurn;
        actionsRemaining = maxActionsPerTurn;
        UpdateActionsUI();
        TurnEvents.PlayerTurnStart();
    }

    private void EndPlayerTurn()
    {
        TurnEvents.PlayerTurnEnd();
        StartCoroutine(WaitBetweenTurns());
    }

    private IEnumerator RunEnemyTurn()   // BARU: menggantikan StartEnemyTurn dan EnemeyTurn
    {
        currentTurn = TurnState.EnemyTurn;
        TurnEvents.EnemyTurnStart();

        List<Enemy> turnOrder = GameManager.Instance.lvl1.GetAliveEnemies();
        ApplyTurnOrder(turnOrder);

        foreach (Enemy enemy in turnOrder)
        {
            if (!GameManager.Instance.IsGameActive()) yield break;
            if (!enemy.IsAlive()) continue;

            displayTurnState.text = enemy.TurnDisplayName + "'s Turn";
            yield return new WaitForSeconds(enemyDelayTime);

            yield return enemy.PlayTurn();
        }

        StartCoroutine(EndEnemyTurn());
    }

    private void ApplyTurnOrder(List<Enemy> enemies)   // BARU: acak urutan musuh
    {
        for (int i = enemies.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (enemies[i], enemies[j]) = (enemies[j], enemies[i]);
        }
    }

    private IEnumerator EndEnemyTurn()
    {
        TurnEvents.EnemyTurnEnd();
        yield return new WaitForSeconds(enemyDelayTime);
        StartCoroutine(WaitBetweenTurns());
    }

    public void SetCurrentEnemy(string enemyName)   // sementara, lihat catatan di bawah
    {
        currentEnemyName = enemyName;
    }

    private void ClearTurnDisplay()
    {
        displayTurnState.text = "";
    }

    private IEnumerator WaitBetweenTurns()
    {
        for (int i = turnWaitTime; i > 0; i--)
        {
            displayTurnState.text = i + "...";
            yield return new WaitForSeconds(1f);
        }

        if (GameManager.Instance.IsGameActive())
        {
            if (currentTurn != TurnState.PlayerTurn)
            {
                displayTurnState.text = "Player's Turn";
                StartPlayerTurn();
            }
            else
            {
                StartCoroutine(RunEnemyTurn());   // UBAH
            }
        }
    }

    private void CardPlayed(CardData cardData)
    {
        ConsumeAction(cardData.actionCost);
    }

    private void DrawRequested()
    {
        ConsumeAction(drawCost);
    }

    private void ReshuffleRequested()
    {
        ConsumeAction(reshuffleCost);
    }

    public bool HasActionsRemaining()
    {
        return actionsRemaining > 0;
    }

    private void ConsumeAction(int amount)
    {
        actionsRemaining -= amount;
        UpdateActionsUI();

        if (actionsRemaining <= 0)
        {
            EndPlayerTurn();
        }
    }

    private void UpdateActionsUI()
    {
        if (actionsRemaining < 0)
            actionsRemaining = 0;

        remainingActionsText.text = "Remaining Actions: " + actionsRemaining;
    }
}