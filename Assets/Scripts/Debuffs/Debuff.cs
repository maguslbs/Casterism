public enum DebuffType
{
    Curse,
    Burn,
    Stun
}

public abstract class Debuff
{
    public abstract DebuffType Type { get; }

    public virtual bool IsExpired => false;

    public virtual void OnApply(Health health) { }
    public virtual void OnPlayerTurnStart(Health health) { }
    public virtual void OnPlayerTurnEnd(Health health) { }
    public virtual void OnRemove(Health health) { }

    public virtual void Refresh(Debuff newDebuff) { }
}