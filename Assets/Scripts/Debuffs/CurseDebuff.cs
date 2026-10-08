public class CurseDebuff : Debuff
{
    private readonly float percent;

    public CurseDebuff(float percent)
    {
        this.percent = percent;
    }

    public override DebuffType Type => DebuffType.Curse;

    public override void OnApply(Health health)
    {
        health.ReduceMaxHealthByPercent(percent);
    }

    public override void OnRemove(Health health)
    {
        health.RestoreMaxHealth();
    }
}