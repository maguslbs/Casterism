using System;

public static class EnemyEvents
{
    public static event Action<Enemy> OnEnemyHit;
    public static event Action<Enemy> OnEnemyDeath;

    public static void EnemyHit(Enemy enemy)
    {
        OnEnemyHit?.Invoke(enemy);
    }

    public static void EnemyDeath(Enemy enemy)
    {
        OnEnemyDeath?.Invoke(enemy);
    }
}
