using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapGenerator generator;
    [SerializeField] private Transform nodesParent;

    [Header("Node Colors")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private Color completedColor = new Color(0.4f, 0.8f, 0.4f);

    private MapNode[] nodes;

    private void Start()
    {
        int seed = RunManager.Instance != null ? RunManager.Instance.Seed : System.Environment.TickCount;
        generator.Generate(seed);

        nodes = nodesParent.GetComponentsInChildren<MapNode>();

        foreach (MapNode node in nodes)
        {
            Button button = node.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => HandleNodeClicked(node));
            }
        }

        RefreshNodes();
    }

    private void RefreshNodes()
    {
        foreach (MapNode node in nodes)
        {
            bool completed = IsCompleted(node);
            bool available = IsAvailable(node);

            Button button = node.GetComponentInChildren<Button>();
            if (button != null) button.interactable = available;

            Image image = node.GetComponentInChildren<Image>();
            if (image != null)
            {
                image.color = completed ? completedColor : available ? availableColor : lockedColor;
            }
        }
    }

    private bool IsCompleted(MapNode node)
    {
        if (node.Tier == MapTier.Start) return true;
        return RunManager.Instance != null && RunManager.Instance.IsCompleted(node.NodeId);
    }

    private bool IsAvailable(MapNode node)
    {
        MapNode current = GetCurrentNode();
        return current != null && current.NextNodes.Contains(node);
    }

    private MapNode GetCurrentNode()
    {
        string currentId = RunManager.Instance != null ? RunManager.Instance.LastCompletedNodeId : null;

        foreach (MapNode node in nodes)
        {
            if (string.IsNullOrEmpty(currentId))
            {
                if (node.Tier == MapTier.Start) return node;
            }
            else if (node.NodeId == currentId)
            {
                return node;
            }
        }

        return null;
    }

    private void HandleNodeClicked(MapNode node)
    {
        if (!IsAvailable(node)) return;

        if (RunManager.Instance == null)
        {
            Debug.LogWarning("[Map] RunManager tidak ditemukan di scene!", this);
            return;
        }

        if (node.Tier == MapTier.Rest)
        {
            RunManager.Instance.RestAtNode(node.NodeId);
            RefreshNodes();
            return;
        }

        RunManager.Instance.EnterNode(node);
    }
}