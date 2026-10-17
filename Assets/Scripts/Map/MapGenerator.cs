using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform nodesParent;
    [SerializeField] private MapLineDrawer lineDrawer;

    [Header("Active Nodes per Tier")]
    [SerializeField, Min(1)] private int minActivePerTier = 2;
    [SerializeField, Min(1)] private int maxActivePerTier = 3;

    [Header("Branching")]
    [SerializeField, Range(0f, 1f)] private float extraPathChance = 0.25f;

    [Header("Encounter Pools")]
    [SerializeField] private List<EncounterData> easyEncounters = new List<EncounterData>();
    [SerializeField] private List<EncounterData> normalEncounters = new List<EncounterData>();
    [SerializeField] private List<EncounterData> eliteEncounters = new List<EncounterData>();
    [SerializeField] private List<EncounterData> bossEncounters = new List<EncounterData>();

    [Header("Testing")]
    [Tooltip("0 = acak setiap Play. Isi angka lain untuk selalu mendapat map yang sama.")]
    [SerializeField] private int testSeed = 0;

    public void Generate(int seed)
    {
        if (testSeed != 0) seed = testSeed;

        Debug.Log($"[Map] seed: {seed}");
        System.Random rng = new System.Random(seed);

        Dictionary<MapTier, List<MapNode>> byTier = GroupByTier();

        List<MapNode> start = byTier[MapTier.Start];
        List<MapNode> easy = PickActive(byTier[MapTier.Easy], rng);
        List<MapNode> normal = PickActive(byTier[MapTier.Normal], rng);
        List<MapNode> rest = byTier[MapTier.Rest];
        List<MapNode> elite = PickActive(byTier[MapTier.Elite], rng);
        List<MapNode> boss = byTier[MapTier.Boss];

        foreach (List<MapNode> tierNodes in byTier.Values)
        {
            foreach (MapNode node in tierNodes) node.ClearNextNodes();
        }

        ConnectAll(start, easy);
        ConnectNearest(easy, normal, rng);
        ConnectAll(normal, rest);
        ConnectAll(rest, elite);
        ConnectAll(elite, boss);

        AssignEncounters(easy, easyEncounters, rng);
        AssignEncounters(normal, normalEncounters, rng);
        AssignEncounters(elite, eliteEncounters, rng);
        AssignEncounters(boss, bossEncounters, rng);

        if (lineDrawer != null) lineDrawer.DrawLines();
    }

    private Dictionary<MapTier, List<MapNode>> GroupByTier()
    {
        Dictionary<MapTier, List<MapNode>> byTier = new Dictionary<MapTier, List<MapNode>>();

        foreach (MapTier tier in System.Enum.GetValues(typeof(MapTier)))
        {
            byTier[tier] = new List<MapNode>();
        }

        foreach (MapNode node in nodesParent.GetComponentsInChildren<MapNode>(true))
        {
            byTier[node.Tier].Add(node);
        }

        return byTier;
    }

    private List<MapNode> PickActive(List<MapNode> nodes, System.Random rng)
    {
        List<MapNode> shuffled = new List<MapNode>(nodes);
        Shuffle(shuffled, rng);

        int max = Mathf.Min(maxActivePerTier, nodes.Count);
        int min = Mathf.Min(minActivePerTier, max);
        int count = rng.Next(min, max + 1);

        List<MapNode> active = shuffled.GetRange(0, count);

        foreach (MapNode node in nodes)
        {
            node.gameObject.SetActive(active.Contains(node));
        }

        return active;
    }

    private void ConnectAll(List<MapNode> from, List<MapNode> to)
    {
        foreach (MapNode a in from)
        {
            foreach (MapNode b in to)
            {
                a.AddNextNode(b);
            }
        }
    }

    private void ConnectNearest(List<MapNode> from, List<MapNode> to, System.Random rng)
    {
        if (from.Count == 0 || to.Count == 0) return;

        HashSet<MapNode> reached = new HashSet<MapNode>();

        foreach (MapNode a in from)
        {
            MapNode nearest = FindNearestByY(a, to, null);
            a.AddNextNode(nearest);
            reached.Add(nearest);

            if (rng.NextDouble() < extraPathChance)
            {
                MapNode second = FindNearestByY(a, to, nearest);
                if (second != null)
                {
                    a.AddNextNode(second);
                    reached.Add(second);
                }
            }
        }

        foreach (MapNode b in to)
        {
            if (!reached.Contains(b))
            {
                FindNearestByY(b, from, null).AddNextNode(b);
            }
        }
    }

    private MapNode FindNearestByY(MapNode node, List<MapNode> candidates, MapNode exclude)
    {
        MapNode nearest = null;
        float bestDistance = float.MaxValue;
        float y = node.transform.position.y;

        foreach (MapNode candidate in candidates)
        {
            if (candidate == exclude) continue;

            float distance = Mathf.Abs(candidate.transform.position.y - y);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                nearest = candidate;
            }
        }

        return nearest;
    }

    private void AssignEncounters(List<MapNode> nodes, List<EncounterData> pool, System.Random rng)
    {
        List<EncounterData> validPool = pool.FindAll(e => e != null);

        if (validPool.Count == 0)
        {
            if (nodes.Count > 0) Debug.LogWarning("[Map] Kumpulan encounter kosong untuk salah satu tier!", this);
            return;
        }

        List<EncounterData> bag = new List<EncounterData>();

        foreach (MapNode node in nodes)
        {
            if (bag.Count == 0)
            {
                bag.AddRange(validPool);
                Shuffle(bag, rng);
            }

            node.SetEncounter(bag[bag.Count - 1]);
            bag.RemoveAt(bag.Count - 1);
        }
    }

    private void Shuffle<T>(List<T> list, System.Random rng)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}