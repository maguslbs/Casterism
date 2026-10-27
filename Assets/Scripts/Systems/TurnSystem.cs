using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSystem : Singleton<TurnSystem>
{
    private enum TurnState { PlayerTurn, EnemyTurn }
    private TurnState currentTurn = TurnState.PlayerTurn;

    [SerializeField] private int maxActionsPerTurn = 1;
    [SerializeField] private int actionsCap = 6;
    [SerializeField] private int drawCost = 1;
    [SerializeField] private int reshuffleCost = 3;
    [SerializeField] private TextMeshProUGUI remainingActionsText;
    [SerializeField] private float playerToEnemyDelay = 0.6f;
    [SerializeField] private float enemyDelayTime = 2f;
    [SerializeField] private float enemyToPlayerDelay = 0.6f;   
    public int ActionsRemaining => actionsRemaining;
    public int ReshuffleCost => reshuffleCost;

    [SerializeField] private TextMeshProUGUI displayTurnState;
    [SerializeField] private PlayerDebuffs playerDebuffs;   
    [SerializeField] private float stunSkipDelay = 1f;

    private int actionsRemaining;

    private int pendingCardResolutions = 0;
    private bool playerTurnEnding = false;

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
        GameEvents.OnGameOver += ClearTurnDisplay;
        PlayerEvents.OnAttackComplete += HandleCardResolved;
    }

    private void OnDisable()
    {
        PlayerEvents.OnDrawCardRequested -= DrawRequested;
        PlayerEvents.OnReshuffleRequested -= ReshuffleRequested;
        PlayerEvents.OnCardPlayed -= CardPlayed;
        GameEvents.OnGameOver -= ClearTurnDisplay;
        PlayerEvents.OnAttackComplete -= HandleCardResolved;
    }

    private void StartPlayerTurn()
    {
        currentTurn = TurnState.PlayerTurn;
        actionsRemaining = GetMaxActions();
        pendingCardResolutions = 0;   // BARU
        playerTurnEnding = false;     // BARU
        UpdateActionsUI();
        TurnEvents.PlayerTurnStart();

        if (playerDebuffs != null && playerDebuffs.IsStunned)   
            StartCoroutine(SkipStunnedTurn());
    }

    private void EndPlayerTurn()
    {
        playerTurnEnding = true;
        TurnEvents.PlayerTurnEnd();
        StartCoroutine(WaitBetweenTurns());
    }

    private IEnumerator RunEnemyTurn()   // BARU: menggantikan StartEnemyTurn dan EnemeyTurn
    {
        currentTurn = TurnState.EnemyTurn;

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
        yield return new WaitForSeconds(enemyToPlayerDelay);   // UBAH: dulu enemyDelayTime + hitung mundur 3 detik

        if (!GameManager.Instance.IsGameActive()) yield break;   // BARU

        displayTurnState.text = "Player's Turn";   // BARU
        StartPlayerTurn();                          // BARU
    }

    private void ClearTurnDisplay()
    {
        displayTurnState.text = "";
    }

    private IEnumerator WaitBetweenTurns()
    {
        yield return new WaitForSeconds(playerToEnemyDelay);   

        if (GameManager.Instance.IsGameActive())
        {
            StartCoroutine(RunEnemyTurn());
        }
    }

    private IEnumerator SkipStunnedTurn()   // BARU
    {
        playerTurnEnding = true;   // memblokir End Turn dan TryEndPlayerTurn selama jeda
        actionsRemaining = 0;      // pemain tidak bisa draw atau main kartu
        UpdateActionsUI();
        displayTurnState.text = "Stunned!";

        yield return new WaitForSeconds(stunSkipDelay);

        if (!GameManager.Instance.IsGameActive()) yield break;

        EndPlayerTurn();
    }

    private void CardPlayed(CardData cardData)
    {
        pendingCardResolutions++;
        ConsumeAction(cardData.actionCost);
    }

    private void HandleCardResolved()
    {
        pendingCardResolutions--;
        TryEndPlayerTurn();
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

    public void EndTurnPressed()
    {
        if (currentTurn != TurnState.PlayerTurn) return;
        if (playerTurnEnding) return;
        if (pendingCardResolutions > 0) return;
        if (!GameManager.Instance.IsGameActive()) return;

        EndPlayerTurn();
    }

    private int GetMaxActions()
    {
        int bonus = RunManager.Instance != null ? RunManager.Instance.BonusActions : 0;
        return Mathf.Min(maxActionsPerTurn + bonus, actionsCap);
    }

    public bool CanGainAction()
    {
        return GetMaxActions() < actionsCap;
    }

    private void ConsumeAction(int amount)
    {
        actionsRemaining -= amount;
        UpdateActionsUI();
        TryEndPlayerTurn();
    }

    private void TryEndPlayerTurn()
    {
        if (currentTurn != TurnState.PlayerTurn) return;
        if (playerTurnEnding) return;
        if (actionsRemaining > 0) return;
        if (pendingCardResolutions > 0) return;

        EndPlayerTurn();
    }

    private void UpdateActionsUI()
    {
        if (actionsRemaining < 0)
            actionsRemaining = 0;

        remainingActionsText.text = "Remaining Actions: " + actionsRemaining;
    }
}