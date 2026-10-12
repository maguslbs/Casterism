using System.Collections.Generic;
using UnityEngine;

public enum MapNodeType
{
    Start,
    Combat,
    Elite,
    Rest,
    Boss
}

public class MapNode : MonoBehaviour
{
    [SerializeField] private string nodeId;
    [SerializeField] private MapNodeType nodeType = MapNodeType.Combat;
    [SerializeField] private EncounterData encounter;
    [SerializeField] private List<MapNode> nextNodes = new List<MapNode>();

    public string NodeId => string.IsNullOrEmpty(nodeId) ? name : nodeId;
    public MapNodeType NodeType => nodeType;
    public EncounterData Encounter => encounter;
    public IReadOnlyList<MapNode> NextNodes => nextNodes;
}