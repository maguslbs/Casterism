using UnityEngine;

public class StunDebuff : Debuff
{
    private int turnsRemaining;

    public StunDebuff(int turns = 1)
    {
        turnsRemaining = Mathf.Max(1, turns);
    }

    public override DebuffType Type => DebuffType.Stun;

    public override bool IsExpired => turnsRemaining <= 0;

    public override void OnPlayerTurnEnd(Health health)
    {
        turnsRemaining--;
    }

    public override void Refresh(Debuff newDebuff)
    {
        if (newDebuff is StunDebuff newStun)
        {
            turnsRemaining = Mathf.Max(turnsRemaining, newStun.turnsRemaining);
        }
    }
}