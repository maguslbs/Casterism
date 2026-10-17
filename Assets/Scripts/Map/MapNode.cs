using System.Collections.Generic;
using UnityEngine;

public enum MapTier
{
    Start,
    Easy,
    Normal,
    Rest,
    Elite,
    Boss
}

public class MapNode : MonoBehaviour
{
    [SerializeField] private string nodeId;
    [SerializeField] private MapTier tier = MapTier.Easy;

    private readonly List<MapNode> nextNodes = new List<MapNode>();

    public string NodeId => string.IsNullOrEmpty(nodeId) ? name : nodeId;
    public MapTier Tier => tier;
    public EncounterData Encounter { get; private set; }
    public IReadOnlyList<MapNode> NextNodes => nextNodes;

    public void SetEncounter(EncounterData encounter)
    {
        Encounter = encounter;
    }

    public void AddNextNode(MapNode node)
    {
        if (node != null && !nextNodes.Contains(node))
        {
            nextNodes.Add(node);
        }
    }

    public void ClearNextNodes()
    {
        nextNodes.Clear();
    }
}