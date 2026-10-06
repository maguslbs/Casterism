using UnityEngine;
using System.Collections;

public class SkeletonKing : Enemy
{
    [Header("Curse")]
    [SerializeField] private string castAnimationName = "Cast";
    [Range(0f, 100f)]
    [SerializeField] private float cursePercent = 30f;
    [SerializeField] private float castDelay = .5f;
    [SerializeField] private float curseRecoveryTime = .5f;

    private bool hasCursed = false;

    protected override IEnumerator TakeTurn()
    {
        if (!hasCursed)
        {
            hasCursed = true;
            yield return CastCurse();
            yield break;
        }

        yield return base.TakeTurn();
    }

    protected override void Die()
    {
        base.Die();

        if (hasCursed)
        {
            PlayerEvents.CurseRemoved();
        }

        SkeletonKingEvents.SkeletonKingDeath();
    }

    private IEnumerator CastCurse()
    {
        animationController.Play(castAnimationName);
        yield return new WaitForSeconds(castDelay);

        PlayerEvents.PlayerCursed(cursePercent);

        yield return new WaitForSeconds(curseRecoveryTime);
        animationController.Play("Idle");
    }
}