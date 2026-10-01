using UnityEngine;
using System.Collections;

public class SkeletonKing : MonoBehaviour
{
    [SerializeField] private EnemyAttack[] attacks; //new variable for creating randomized attack
    [SerializeField] private int maxRepeat = 2;
    private EnemyAttack lastAttack;
    private int repeatCount = 0;
    private Health health;
    private Animator animationController;
    private Vector3 originalPosition;

    [SerializeField] private GameObject skeletonKingSprite;
    [SerializeField] private string turnDisplayName = "Skeleton King";

    [Header("Curse")]
    [SerializeField] private string castAnimationName = "Cast";
    [SerializeField] private int curseAmount = 30;
    [SerializeField] private float castDelay = .5f;
    [SerializeField] private float curseRecoveryTime = .5f;

    private bool hasCursed = false;

    private void Awake()
    {
        health = GetComponent<Health>();
        animationController = skeletonKingSprite.GetComponent<Animator>();
    }

    private void Start()
    {
        originalPosition = skeletonKingSprite.transform.position;
        TurnSystem.Instance.SetCurrentEnemy(turnDisplayName);
    }

    private void OnEnable()
    {
        SkeletonKingEvents.OnSkeletonKingHit += HandleSkeletonKingHit;
        TurnEvents.OnBossTurnStart += Attack;
    }

    private void OnDisable()
    {
        SkeletonKingEvents.OnSkeletonKingHit -= HandleSkeletonKingHit;
        TurnEvents.OnBossTurnStart -= Attack;
    }

    private EnemyAttack PickAttack(EnemyAttack exclude = null) //choosing attack based on weight
    {
        int totalWeight = 0;
        foreach (EnemyAttack attack in attacks)
        {
            if (attack != exclude) totalWeight += attack.weight;
        }

        if (totalWeight <= 0) return exclude ?? attacks[0];

        int roll = Random.Range(0, totalWeight);

        foreach (EnemyAttack attack in attacks)
        {
            if (attack == exclude) continue;

            if (roll < attack.weight) return attack;
            roll -= attack.weight;
        }

        return attacks[0];
    }

    private void Attack()
    {
        if (!health.IsAlive()) return;

        if (attacks == null || attacks.Length == 0)
        {
            Debug.LogWarning("Skeleton King has no list of attacks", this);
            return;
        }

        if (!hasCursed)
        {
            hasCursed = true;
            StartCoroutine(CastCurse());
            return;
        }

        EnemyAttack exclude = (repeatCount >= maxRepeat) ? lastAttack : null;
        EnemyAttack chosen = PickAttack(exclude);

        repeatCount = (chosen == lastAttack) ? repeatCount + 1 : 1; //can't repeat the same attack pattern twice
        lastAttack = chosen;

        StartCoroutine(SkeletonKingAttackAnimation(chosen));
    }

    private void HandleSkeletonKingHit(CardData carddata) //Skeleton King damaged
    {
        if (!health.IsAlive()) return;

        animationController.Play("Hurt");

        health.TakeDamage(carddata.attackPower);

        if (!health.IsAlive())
        {
            Die();
        }
    }

    private void Die()
    {
        animationController.Play("Death");

        if (hasCursed)                      
        {                                   
            PlayerEvents.CurseRemoved();    
        }

        SkeletonKingEvents.SkeletonKingDeath();
    }

    private IEnumerator SkeletonKingAttackAnimation(EnemyAttack attack)
    {
        animationController.Play("Walk");
        Vector3 targetPosition = originalPosition + new Vector3(-3f, 0, 0);

        float duration = .5f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            skeletonKingSprite.transform.position = Vector3.Lerp(originalPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        animationController.Play(attack.animationName);
        PlayerEvents.PlayerHit(attack.damage);

        yield return new WaitForSeconds(attack.recoveryTime);

        timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            skeletonKingSprite.transform.position = Vector3.Lerp(targetPosition, originalPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        skeletonKingSprite.transform.position = originalPosition;

        yield return null;
    }

    private IEnumerator CastCurse()
    {
        animationController.Play(castAnimationName);
        yield return new WaitForSeconds(castDelay);

        PlayerEvents.PlayerCursed(curseAmount);

        yield return new WaitForSeconds(curseRecoveryTime);
        animationController.Play("Idle");
    }
}
