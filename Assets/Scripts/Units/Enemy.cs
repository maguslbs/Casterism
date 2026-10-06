using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private string turnDisplayName = "Enemy";
    [SerializeField] protected GameObject sprite;
    [SerializeField] private EnemyAttack[] attacks;
    [SerializeField] private int maxRepeat = 2;

    [Header("Melee")]
    [SerializeField] private string moveAnimationName = "Walk";
    [SerializeField] private float meleeDistance = 4f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private Vector3 meleeOffset = Vector3.zero;

    [Header("Targeting")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform indicatorPoint;

    protected Health health;
    protected Animator animationController;
    protected Vector3 originalPosition;
    protected Player player;

    private EnemyAttack lastAttack;
    private int repeatCount = 0;

    public string TurnDisplayName => turnDisplayName;

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        animationController = sprite.GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        originalPosition = sprite.transform.position;
        player = FindAnyObjectByType<Player>();
    }


    public bool IsAlive()
    {
        return health.IsAlive();
    }

    public IEnumerator PlayTurn()   // BARU: dipanggil TurnSystem saat giliran musuh ini
    {
        if (!health.IsAlive()) yield break;

        yield return TakeTurn();
    }

    public Vector3 GetAttackPosition(float fallbackDistance)
    {
        if (attackPoint != null)
        {
            return attackPoint.position;
        }

        return transform.position + new Vector3(-fallbackDistance, 0f, 0f);
    }

    public Vector3 GetIndicatorPosition(Vector3 fallbackOffset)
    {
        if (indicatorPoint != null)
        {
            return indicatorPoint.position;
        }

        return transform.position + fallbackOffset;
    }

    public void TakeHit(CardData cardData)
    {
        if (!health.IsAlive()) return;

        animationController.Play("Hurt");
        health.TakeDamage(cardData.attackPower);
        EnemyEvents.EnemyHit(this);

        if (!health.IsAlive())
        {
            Die();
        }

        GameManager.Instance.lvl1.PlayerWin();
    }

    protected virtual void Die()
    {
        animationController.Play("Death");
        EnemyEvents.EnemyDeath(this);
    }

    protected virtual IEnumerator TakeTurn()
    {
        EnemyAttack chosen = ChooseAttack();
        if (chosen == null) yield break;

        yield return PerformAttack(chosen);
    }

    protected virtual IEnumerator PerformAttack(EnemyAttack attack)
    {
        yield return MeleeAttack(attack);
    }

    protected EnemyAttack ChooseAttack()
    {
        if (attacks == null || attacks.Length == 0)
        {
            Debug.LogWarning($"{name} has no list of attacks", this);
            return null;
        }

        EnemyAttack exclude = (repeatCount >= maxRepeat) ? lastAttack : null;
        EnemyAttack chosen = PickAttack(exclude);

        repeatCount = (chosen == lastAttack) ? repeatCount + 1 : 1;
        lastAttack = chosen;

        return chosen;
    }

    private EnemyAttack PickAttack(EnemyAttack exclude)
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

    protected IEnumerator MeleeAttack(EnemyAttack attack)
    {
        Vector3 targetPosition = GetMeleeTargetPosition();

        animationController.Play(moveAnimationName);
        yield return MoveSprite(originalPosition, targetPosition);

        animationController.Play(attack.animationName);
        PlayerEvents.PlayerHit(attack.damage);
        yield return new WaitForSeconds(attack.recoveryTime);

        yield return MoveSprite(targetPosition, originalPosition);
        animationController.Play("Idle");
    }

    private Vector3 GetMeleeTargetPosition()
    {
        if (player != null)
        {
            Vector3 point = player.GetEnemyAttackPosition();
            return new Vector3(point.x, point.y, originalPosition.z) + meleeOffset;
        }

        return originalPosition + new Vector3(-meleeDistance, 0f, 0f);
    }

    private IEnumerator MoveSprite(Vector3 from, Vector3 to)
    {
        float timeElapsed = 0f;

        while (timeElapsed < moveDuration)
        {
            sprite.transform.position = Vector3.Lerp(from, to, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        sprite.transform.position = to;
    }
}