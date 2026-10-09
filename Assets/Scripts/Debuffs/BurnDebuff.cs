using UnityEngine;

public class BurnDebuff : Debuff
{
    private readonly int damagePerTurn;
    private readonly int duration;
    private int turnsRemaining;

    public BurnDebuff(int damagePerTurn, int duration = 0)
    {
        this.damagePerTurn = Mathf.Max(0, damagePerTurn);
        this.duration = Mathf.Max(0, duration);
        turnsRemaining = this.duration;
    }

    public override DebuffType Type => DebuffType.Burn;

    public override bool IsExpired => duration > 0 && turnsRemaining <= 0;

    public override void OnPlayerTurnStart(Health health)
    {
        PlayerEvents.PlayerHit(damagePerTurn);

        if (duration > 0)
        {
            turnsRemaining--;
        }
    }

    public override void Refresh(Debuff newDebuff)
    {
        if (newDebuff is BurnDebuff newBurn)
        {
            turnsRemaining = newBurn.duration;
        }
    }
}