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
}
