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

    public int GetAttackPower()   
    {
        if (attackPower <= 0 || RunManager.Instance == null) return attackPower;
        return attackPower + RunManager.Instance.BonusAttack;
    }

    public int GetHealPower()     
    {
        if (healPower <= 0 || RunManager.Instance == null) return healPower;
        return healPower + RunManager.Instance.BonusHeal;
    }

    public string GetDescription()   
    {
        return description
            .Replace("{attack}", GetAttackPower().ToString())
            .Replace("{heal}", GetHealPower().ToString());
    }
}
