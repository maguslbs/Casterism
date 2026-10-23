    using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;

    public string description;

    public int actionCost;

    public Sprite illustration;

    public int attackPower;

    public int healPower;
    public bool requiresTarget;
    public bool cleanse;
    public bool execute;
    [Range(0f, 1f)] public float executeThreshold = 0.3f;
    public bool hitsAllEnemies;
    public bool deflect;
}
