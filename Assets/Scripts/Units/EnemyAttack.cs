using UnityEngine;

[System.Serializable]
public class EnemyAttack
{
    public string animationName = "Attack1";
    public int damage = 5;
    [Min(0)] public int weight = 1;
    public float recoveryTime = .5f;
}