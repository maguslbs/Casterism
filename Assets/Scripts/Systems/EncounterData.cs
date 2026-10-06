using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyComposition
{
    public List<Enemy> enemies = new List<Enemy>();
}

[CreateAssetMenu(fileName = "EncounterData", menuName = "Scriptable Objects/EncounterData")]
public class EncounterData : ScriptableObject
{
    public List<EnemyComposition> compositions = new List<EnemyComposition>();

    public EnemyComposition PickRandomComposition()
    {
        if (compositions == null || compositions.Count == 0)
        {
            return null;
        }

        return compositions[Random.Range(0, compositions.Count)];
    }
}