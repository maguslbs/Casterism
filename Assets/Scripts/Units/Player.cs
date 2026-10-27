using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private float attackStopDistance = 1.5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Transform enemyAttackPoint;

    [Header("Execute")]
    [SerializeField] private string castAnimationName = "Cast";
    [SerializeField] private GameObject executeSwordPrefab;
    [SerializeField] private Vector3 swordImpactOffset = Vector3.zero;
    [SerializeField] private float swordDropHeight = 6f;
    [SerializeField] private float swordHoverTime = 0.3f;
    [SerializeField] private float swordFallDuration = 0.25f;
    [SerializeField] private float swordLingerTime = 0.4f;

    [Header("Sword Rain")]
    [SerializeField] private GameObject rainSwordPrefab;
    [SerializeField] private int minRainSwords = 8;
    [SerializeField] private int maxRainSwords = 10;
    [SerializeField] private float rainSpreadX = 0.6f;
    [SerializeField] private float rainSpreadY = 0.5f;
    [SerializeField] private float rainSwordInterval = 0.06f;
    [SerializeField] private float rainDropHeight = 6f;
    [SerializeField] private float rainFallDuration = 0.2f;
    [SerializeField] private float rainLingerTime = 0.3f;

    private Vector3 originalPosition;
    private Animator animationController;
    private ParticleSystem healVFX;

    private Health health;
    private PlayerDebuffs debuffs;

    private void OnEnable()
    {
        PlayerEvents.OnCardPlayed += HandleCardPlayed;
        PlayerEvents.OnPlayerHit += PlayerHit;
        PlayerEvents.OnDebuffApplied += HandleDebuffApplied;   
        PlayerEvents.OnDebuffRemoved += HandleDebuffRemoved;   
        GameEvents.OnGameOver += SaveHealthToRun;
    }

    private void OnDisable()
    {
        PlayerEvents.OnCardPlayed -= HandleCardPlayed;
        PlayerEvents.OnPlayerHit -= PlayerHit;
        PlayerEvents.OnDebuffApplied -= HandleDebuffApplied;   
        PlayerEvents.OnDebuffRemoved -= HandleDebuffRemoved;   
        GameEvents.OnGameOver -= SaveHealthToRun;
    }

    private void Awake()
    {
        animationController = playerSprite.GetComponent<Animator>();
        health = GetComponent<Health>();
        debuffs = GetComponent<PlayerDebuffs>();   
        healVFX = playerSprite.GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        originalPosition = playerSprite.transform.position;

        if (RunManager.Instance != null && RunManager.Instance.HasPlayerHealth)
        {
            health.SetHealth(RunManager.Instance.PlayerHealth, RunManager.Instance.PlayerMaxHealth);
        }
    }

    private void SaveHealthToRun()
    {
        if (RunManager.Instance == null || !health.IsAlive()) return;

        RunManager.Instance.SavePlayerHealth(health.CurrentHealth, health.BaseMaxHealth);
    }

    public Vector3 GetEnemyAttackPosition()
    {
        if (enemyAttackPoint != null)
        {
            return enemyAttackPoint.position;
        }

        return originalPosition + new Vector3(attackStopDistance, 0f, 0f);
    }

    private void PlayerHit(int damage)
    {
        animationController.Play("Hurt");

        health.TakeDamage(damage);

        if (!health.IsAlive())
        {
            Die();
        }
    }

    private void HandleDebuffApplied(Debuff debuff)   // BARU: menggantikan HandleCursed
    {
        if (!health.IsAlive()) return;

        animationController.Play("Hurt");
        debuffs.Apply(debuff);

        if (!health.IsAlive())
        {
            Die();
        }
    }

    private void HandleDebuffRemoved(DebuffType type)   // BARU: menggantikan HandleCurseRemoved
    {
        debuffs.Remove(type);
    }

    private void Die()
    {
        animationController.Play("Death");
        PlayerEvents.PlayerDeath();
    }

    private void HandleCardPlayed(CardData cardData)
    {
        bool startedAttack = false;

        if (cardData.cleanse)
        {
            debuffs.CleanseAll();
        }

        if (cardData.attackPower > 0)
        {
            if (cardData.hitsAllEnemies)
            {
                StartCoroutine(PlayerSwordRainAnimation(cardData));
                startedAttack = true;
            }
            else
            {
                Enemy target = TargetSelector.Instance.CurrentTarget;

                if (target != null)
                {
                    if (cardData.execute)
                    {
                        StartCoroutine(PlayerExecuteAnimation(cardData, target));
                    }
                    else
                    {
                        Attack(cardData, target);
                    }

                    startedAttack = true;
                }
            }
        }

        if (cardData.deflect)   
        {
            Enemy target = TargetSelector.Instance.CurrentTarget;

            if (target != null)
            {
                target.ApplyDeflect();
            }
        }

        if (cardData.healPower > 0)
        {
            Heal(cardData);
        }

        if (!startedAttack)
        {
            PlayerEvents.AttackComplete();
        }
    }


    private void Attack(CardData cardData, Enemy target)
    {
        StartCoroutine(PlayerAttackAnimation(cardData, target));
    }

    private void Heal(CardData cardData)
    {
        health.HealDamage(cardData.GetHealPower());
        healVFX.Play();
        PlayerEvents.PlayerHealed();
    }

    private IEnumerator MoveSprite(Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        float duration = distance / Mathf.Max(moveSpeed, 0.01f);
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            playerSprite.transform.position = Vector3.Lerp(from, to, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerSprite.transform.position = to;
    }

    private IEnumerator DropSword(GameObject prefab, Vector3 hitPosition, float dropHeight,
                              float hoverTime, float fallDuration, float lingerTime)
    {
        Vector3 startPosition = hitPosition + Vector3.up * dropHeight;
        GameObject sword = Instantiate(prefab, startPosition, prefab.transform.rotation);

        if (hoverTime > 0f)
        {
            yield return new WaitForSeconds(hoverTime);
        }

        float timeElapsed = 0f;
        while (timeElapsed < fallDuration)
        {
            float t = timeElapsed / fallDuration;
            sword.transform.position = Vector3.Lerp(startPosition, hitPosition, t * t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        sword.transform.position = hitPosition;
        Destroy(sword, lingerTime);
    }

    private IEnumerator PlayerAttackAnimation(CardData cardData, Enemy target)
    {
        animationController.Play("Run");

        Vector3 attackPosition = target.GetAttackPosition(attackStopDistance);
        Vector3 targetPosition = new Vector3(attackPosition.x, attackPosition.y, originalPosition.z);

        yield return MoveSprite(originalPosition, targetPosition);

        animationController.Play("Attack");

        if (target != null && target.IsAlive())
        {
            target.TakeHit(cardData);
        }

        yield return new WaitForSeconds(.5f);

        yield return MoveSprite(targetPosition, originalPosition);

        PlayerEvents.AttackComplete();
    }

    private IEnumerator PlayerExecuteAnimation(CardData cardData, Enemy target)
    {
        animationController.Play(castAnimationName);

        if (executeSwordPrefab != null)
        {
            Vector3 hitPosition = target.GetHitPosition() + swordImpactOffset;
            yield return DropSword(executeSwordPrefab, hitPosition, swordDropHeight,
                                   swordHoverTime, swordFallDuration, swordLingerTime);
        }

        if (target != null && target.IsAlive())
        {
            target.TakeHit(cardData);
        }

        yield return new WaitForSeconds(swordLingerTime);

        animationController.Play("Idle");
        PlayerEvents.AttackComplete();
    }

    private IEnumerator PlayerSwordRainAnimation(CardData cardData)
    {
        animationController.Play(castAnimationName);

        List<Enemy> targets = LevelController.Instance.GetAliveEnemies();

        if (targets.Count == 0)
        {
            animationController.Play("Idle");
            PlayerEvents.AttackComplete();
            yield break;
        }

        if (rainSwordPrefab != null)
        {
            GetRainArea(targets, out Vector2 areaMin, out Vector2 areaMax);
            int swordCount = Random.Range(minRainSwords, maxRainSwords + 1);

            for (int i = 0; i < swordCount; i++)
            {
                Vector3 hitPosition;

                if (i < targets.Count)
                {
                    hitPosition = targets[i].GetHitPosition();
                }
                else
                {
                    hitPosition = new Vector3(Random.Range(areaMin.x, areaMax.x),
                                              Random.Range(areaMin.y, areaMax.y), 0f);
                }

                StartCoroutine(DropSword(rainSwordPrefab, hitPosition, rainDropHeight,
                                         0f, rainFallDuration, rainLingerTime));

                yield return new WaitForSeconds(rainSwordInterval);
            }

            yield return new WaitForSeconds(rainFallDuration);
        }

        foreach (Enemy enemy in targets)
        {
            if (enemy != null && enemy.IsAlive())
            {
                enemy.TakeHit(cardData);
            }
        }

        yield return new WaitForSeconds(rainLingerTime);

        animationController.Play("Idle");
        PlayerEvents.AttackComplete();
    }

    private void GetRainArea(List<Enemy> targets, out Vector2 min, out Vector2 max)
    {
        min = new Vector2(float.MaxValue, float.MaxValue);
        max = new Vector2(float.MinValue, float.MinValue);

        foreach (Enemy enemy in targets)
        {
            Vector3 position = enemy.GetHitPosition();
            min = Vector2.Min(min, position);
            max = Vector2.Max(max, position);
        }

        min -= new Vector2(rainSpreadX, rainSpreadY);
        max += new Vector2(rainSpreadX, rainSpreadY);
    }
}