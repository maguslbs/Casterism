using UnityEngine;

public enum AttackType
{
    Melee,
    Meteor,
    FireBreath
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

    public bool burnOnHit = false;
    [Min(0)] public int burnDamagePerTurn = 5;
    [Tooltip("0 = sampai di-cleanse atau sumbernya mati")]
    [Min(0)] public int burnDuration = 0;
}

