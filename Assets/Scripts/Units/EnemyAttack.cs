using UnityEngine;

public enum AttackType
{
    Melee,
    Meteor
}

[System.Serializable]
public class EnemyAttack
{
    public string animationName = "Attack1";
    public AttackType type = AttackType.Melee;
    public int damage = 5;
    [Min(0)] public int weight = 1;
    public float recoveryTime = .5f;

    [Header("On Hit")]
    public bool stunOnHit = false;
    [Min(1)] public int stunTurns = 1;
}

