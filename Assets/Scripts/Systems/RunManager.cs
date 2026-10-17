using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : Singleton<RunManager>
{
    [SerializeField] private string battleSceneName = "GameScene1";
    [SerializeField] private string mapSceneName = "MapScene";
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    private readonly List<string> completedNodeIds = new List<string>();

    public int Seed { get; private set; }
    public string LastCompletedNodeId { get; private set; }
    public string CurrentNodeId { get; private set; }
    public EncounterData CurrentEncounter { get; private set; }
    public MapTier CurrentNodeTier { get; private set; }
    public int BonusActions { get; private set; }
    public bool HasPlayerHealth { get; private set; }
    public int PlayerHealth { get; private set; }
    public int PlayerMaxHealth { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        DontDestroyOnLoad(gameObject);
        StartNewRun();
    }

    public void StartNewRun()
    {
        Seed = System.Environment.TickCount;
        LastCompletedNodeId = null;
        CurrentNodeId = null;
        CurrentEncounter = null;
        BonusActions = 0;
        HasPlayerHealth = false;
        completedNodeIds.Clear();
    }

    public bool IsCompleted(string nodeId)
    {
        return completedNodeIds.Contains(nodeId);
    }

    public void SavePlayerHealth(int current, int max)
    {
        PlayerHealth = current;
        PlayerMaxHealth = max;
        HasPlayerHealth = true;
    }

    public void RestAtNode(string nodeId)
    {
        if (HasPlayerHealth)
        {
            PlayerHealth = PlayerMaxHealth;
        }

        MarkCompleted(nodeId);
    }

    public void AddMaxHealth(int amount)
    {
        if (!HasPlayerHealth || amount <= 0) return;

        PlayerMaxHealth += amount;
        PlayerHealth += amount;
    }

    public void AddBonusAction()
    {
        BonusActions++;
    }

    public void EnterNode(MapNode node)
    {
        CurrentNodeId = node.NodeId;
        CurrentEncounter = node.Encounter;
        CurrentNodeTier = node.Tier;
        SceneManager.LoadScene(battleSceneName);
    }

    public void CompleteNodeWithoutBattle(string nodeId)
    {
        MarkCompleted(nodeId);
    }

    public void CompleteCurrentNode()
    {
        MarkCompleted(CurrentNodeId);
        CurrentNodeId = null;
        CurrentEncounter = null;
        SceneManager.LoadScene(mapSceneName);
    }

    public void EndRun()
    {
        SceneManager.LoadScene(mainMenuSceneName);
        Destroy(gameObject);
    }

    private void MarkCompleted(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId)) return;

        if (!completedNodeIds.Contains(nodeId))
        {
            completedNodeIds.Add(nodeId);
        }

        LastCompletedNodeId = nodeId;
    }
}