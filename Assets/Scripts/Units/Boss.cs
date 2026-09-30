using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [SerializeField] private EnemyAttack[] attacks;
    [SerializeField] private int maxRepeat = 2;
    [SerializeField] private GameObject bossSprite;
    [SerializeField] private string turnDisplayName = "Demon Wizard";

    [Header("Meteor")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private GameObject impactVFXPrefab;
    [SerializeField] private Vector3 meteorSpawnOffset = new Vector3(3f, 8f, 0f);
    [SerializeField] private float meteorFallDuration = .8f;
    [SerializeField] private float castDelay = .5f;
    [SerializeField] private float impactDuration = .5f;

    private Health health;
    private Animator animationController;
    private Vector3 originalPosition;

    private EnemyAttack lastAttack;
    private int repeatCount = 0;

    private void Awake()
    {
        health = GetComponent<Health>();
        animationController = bossSprite.GetComponent<Animator>();
    }

    private void Start()
    {
        originalPosition = bossSprite.transform.position;
        TurnSystem.Instance.SetCurrentEnemy(turnDisplayName);
    }

    private void OnEnable()
    {
        BossEvents.OnBossHit += HandleBossHit;
        TurnEvents.OnBossTurnStart += Attack;
    }

    private void OnDisable()
    {
        BossEvents.OnBossHit -= HandleBossHit;
        TurnEvents.OnBossTurnStart -= Attack;
    }

    private EnemyAttack PickAttack(EnemyAttack exclude = null)
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
            Debug.LogWarning("Boss has no list of attacks", this);
            return;
        }

        EnemyAttack exclude = (repeatCount >= maxRepeat) ? lastAttack : null;
        EnemyAttack chosen = PickAttack(exclude);

        repeatCount = (chosen == lastAttack) ? repeatCount + 1 : 1;
        lastAttack = chosen;

        if (chosen.type == AttackType.Meteor)
        {
            StartCoroutine(MeteorAttack(chosen));
        }
        else
        {
            StartCoroutine(BossAttackAnimation(chosen));
        }

    }

    private void HandleBossHit(CardData carddata) //Boss damaged
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
        BossEvents.BossDeath();
    }

    private IEnumerator BossAttackAnimation(EnemyAttack attack)
    {
        animationController.Play("Run");
        Vector3 targetPosition = originalPosition + new Vector3(-4f, 0, 0);

        float duration = .5f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            bossSprite.transform.position = Vector3.Lerp(originalPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        animationController.Play(attack.animationName);
        PlayerEvents.PlayerHit(attack.damage);

        yield return new WaitForSeconds(attack.recoveryTime);

        timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            bossSprite.transform.position = Vector3.Lerp(targetPosition, originalPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        bossSprite.transform.position = originalPosition;

        yield return null;
    }

    private IEnumerator MeteorAttack(EnemyAttack attack)
    {
        if (playerTarget == null || meteorPrefab == null)
        {
            Debug.LogWarning("Player Target or Meteor is empty", this);
            yield break;
        }

        animationController.Play(attack.animationName);
        yield return new WaitForSeconds(castDelay);

        Vector3 impactPosition = playerTarget.position;
        Vector3 spawnPosition = impactPosition + meteorSpawnOffset;

        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

        float timeElapsed = 0f;
        while (timeElapsed < meteorFallDuration)
        {
            float t = timeElapsed / meteorFallDuration;
            meteor.transform.position = Vector3.Lerp(spawnPosition, impactPosition, t * t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(meteor);

        if (impactVFXPrefab != null)
        {
            GameObject impact = Instantiate(impactVFXPrefab, impactPosition, Quaternion.identity);
            Destroy(impact, impactDuration);
        }

        PlayerEvents.PlayerHit(attack.damage);

        yield return new WaitForSeconds(attack.recoveryTime);
        animationController.Play("Idle");
    }
}
