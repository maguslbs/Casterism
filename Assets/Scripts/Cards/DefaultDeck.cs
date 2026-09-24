using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DefaultDeck", menuName = "Scriptable Objects/DefaultDeck")]
public class DefaultDeck : ScriptableObject
{
    public List<CardData> cards = new List<CardData>();
}
