using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private float attackStopDistance = 1.5f;   // BARU
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Transform enemyAttackPoint;
    private Vector3 originalPosition;
    private Animator animationController;
    private ParticleSystem healVFX;

    private Health health;

    [Header("Curse")]
    [SerializeField] private GameObject curseVFXPrefab;
    [SerializeField] private Vector3 curseVFXOffset = Vector3.zero;
    private GameObject activeCurseVFX;

    private void OnEnable()
    {
        PlayerEvents.OnCardPlayed += HandleCardPlayed;
        PlayerEvents.OnPlayerHit += PlayerHit;
        PlayerEvents.OnPlayerCursed += HandleCursed;
        PlayerEvents.OnCurseRemoved += HandleCurseRemoved;
    }

    private void OnDisable()
    {
        PlayerEvents.OnCardPlayed -= HandleCardPlayed;
        PlayerEvents.OnPlayerHit -= PlayerHit;
        PlayerEvents.OnPlayerCursed -= HandleCursed;
        PlayerEvents.OnCurseRemoved -= HandleCurseRemoved;
    }

    private void Awake()
    {
        animationController = playerSprite.GetComponent<Animator>();
        health = GetComponent<Health>();
        healVFX = playerSprite.GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        originalPosition = playerSprite.transform.position;
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

    private void HandleCursed(float percent)
    {
        animationController.Play("Hurt");
        health.ReduceMaxHealthByPercent(percent);

        if (curseVFXPrefab != null && activeCurseVFX == null)
        {
            activeCurseVFX = Instantiate(curseVFXPrefab, playerSprite.transform);
            activeCurseVFX.transform.localPosition = curseVFXOffset;
        }

        if (!health.IsAlive())
        {
            Die();
        }
    }

    private void HandleCurseRemoved()
    {
        health.RestoreMaxHealth();

        if (activeCurseVFX != null)
        {
            Destroy(activeCurseVFX);
            activeCurseVFX = null;
        }
    }

    private void Die()
    {
        animationController.Play("Death");
        PlayerEvents.PlayerDeath();
    }

    private void HandleCardPlayed(CardData cardData)
    {
        bool startedAttack = false;

        if (cardData.attackPower > 0)
        {
            Enemy target = TargetSelector.Instance.CurrentTarget;

            if (target != null)
            {
                Attack(cardData, target);
                startedAttack = true;
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

    private void Attack(CardData cardData, Enemy target)   // UBAH: menerima target
    {
        StartCoroutine(PlayerAttackAnimation(cardData, target));
    }

    private void Heal(CardData cardData)
    {
        health.HealDamage(cardData.healPower);
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
}