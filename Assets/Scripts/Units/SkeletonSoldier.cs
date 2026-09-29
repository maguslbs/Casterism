using UnityEngine;
using System.Collections;

public class SkeletonSoldier : MonoBehaviour
{
    [SerializeField] private EnemyAttack[] attacks; //new variable for creating randomized attack
    [SerializeField] private int maxRepeat = 2;
    private EnemyAttack lastAttack;
    private int repeatCount = 0;
    private Health health;
    private Animator animationController;
    private Vector3 originalPosition;

    [SerializeField] private GameObject skSoldierSprite;
    [SerializeField] private string turnDisplayName = "Skeleton Soldier";

    private void Awake()
    {
        health = GetComponent<Health>();
        animationController = skSoldierSprite.GetComponent<Animator>();
    }

    private void Start()
    {
        originalPosition = skSoldierSprite.transform.position;
        TurnSystem.Instance.SetCurrentEnemy(turnDisplayName);
    }

    private void OnEnable()
    {
        SkeletonSoldierEvents.OnSkSoldierHit += HandleSkSoldierHit;
        TurnEvents.OnBossTurnStart += Attack;
    }

    private void OnDisable()
    {
        SkeletonSoldierEvents.OnSkSoldierHit -= HandleSkSoldierHit;
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
            Debug.LogWarning("SkeletonSoldier has no list of attacks", this);
            return;
        }

        EnemyAttack exclude = (repeatCount >= maxRepeat) ? lastAttack : null;
        EnemyAttack chosen = PickAttack(exclude);

        repeatCount = (chosen == lastAttack) ? repeatCount + 1 : 1; //can't repeat the same attack pattern twice
        lastAttack = chosen;

        StartCoroutine(SkSoldierAttackAnimation(chosen));
    }

    private void HandleSkSoldierHit(CardData carddata) //SkSoldier damaged
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
        SkeletonSoldierEvents.SkSoldierDeath();
    }

    private IEnumerator SkSoldierAttackAnimation(EnemyAttack attack)
    {
        animationController.Play("Walk");
        Vector3 targetPosition = originalPosition + new Vector3(-4f, 0, 0);

        float duration = .5f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            skSoldierSprite.transform.position = Vector3.Lerp(originalPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        animationController.Play(attack.animationName);
        PlayerEvents.PlayerHit(attack.damage);

        yield return new WaitForSeconds(attack.recoveryTime);

        timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            skSoldierSprite.transform.position = Vector3.Lerp(targetPosition, originalPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        skSoldierSprite.transform.position = originalPosition;

        yield return null;
    }
}
