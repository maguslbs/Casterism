using System.Collections;
using UnityEngine;

public class Gargoyle : Enemy
{
    [Header("Fire Breath")]
    [SerializeField] private float fireHitDelay = 1f;

    protected override IEnumerator PerformAttack(EnemyAttack attack)
    {
        if (attack.type == AttackType.FireBreath)
        {
            yield return FireBreath(attack);
        }
        else
        {
            yield return base.PerformAttack(attack);
        }
    }

    protected override void Die()
    {
        base.Die();
        PlayerEvents.RemoveDebuff(DebuffType.Burn);
    }

    private IEnumerator FireBreath(EnemyAttack attack)
    {
        animationController.Play(attack.animationName);
        yield return new WaitForSeconds(fireHitDelay);

        PlayerEvents.PlayerHit(attack.damage);
        ApplyOnHitEffects(attack);

        yield return new WaitForSeconds(attack.recoveryTime);
        animationController.Play("Idle");
    }
}