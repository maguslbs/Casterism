public class FallenSoldier : Enemy
{
    protected override void Die()
    {
        base.Die();
        FallenSoldierEvents.FlSoldierDeath();
    }
}