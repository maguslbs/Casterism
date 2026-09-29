using System.Collections;
using TMPro;
using UnityEngine;

public class TurnSystem : Singleton<TurnSystem>
{
    private enum TurnState{PlayerTurn, EnemyTurn}
    private TurnState currentTurn = TurnState.PlayerTurn;

    [SerializeField] private int maxActionsPerTurn = 1;
    [SerializeField] private int drawCost = 1;
    [SerializeField] private int reshuffleCost = 3;
    [SerializeField] private TextMeshProUGUI remainingActionsText;
    [SerializeField] private int turnWaitTime = 3;
    [SerializeField] private float enemyDelayTime = 2f;

    [SerializeField] private TextMeshProUGUI displayTurnState;

    private int actionsRemaining;
    private string currentEnemyName = "Name";

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
        SkeletonSoldierEvents.OnSkSoldierDeath += ClearTurnDisplay;
        PlayerEvents.OnPlayerDeath += ClearTurnDisplay;
    }

    private void OnDisable()
    {
        PlayerEvents.OnDrawCardRequested -= DrawRequested;
        PlayerEvents.OnReshuffleRequested -= ReshuffleRequested;
        PlayerEvents.OnCardPlayed -= CardPlayed;
        BossEvents.OnBossDeath -= ClearTurnDisplay;
        SkeletonSoldierEvents.OnSkSoldierDeath -= ClearTurnDisplay;
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

    private IEnumerator StartEnemyTurn()
    {
        currentTurn = TurnState.EnemyTurn;
        yield return new WaitForSeconds(enemyDelayTime);
        EnemeyTurn();
    }

    private IEnumerator EndEnemyTurn()
    {
        TurnEvents.EnemyTurnEnd();
        yield return new WaitForSeconds(enemyDelayTime);
        StartCoroutine(WaitBetweenTurns());
    }

    public void SetCurrentEnemy(string enemyName)
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
                displayTurnState.text = currentEnemyName + "'s Turn";
                StartCoroutine(StartEnemyTurn());
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

    private void EnemeyTurn()
    {
        TurnEvents.EnemyTurnStart();
        StartCoroutine(EndEnemyTurn());
    }

    private void UpdateActionsUI()
    {
        if (actionsRemaining < 0)
            actionsRemaining = 0;

        remainingActionsText.text = "Remaining Actions: " + actionsRemaining;
    }
}
