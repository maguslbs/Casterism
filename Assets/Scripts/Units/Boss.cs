using UnityEngine;
using System.Collections;

public class Boss : Enemy
{
    [SerializeField] private Transform playerTarget;
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private GameObject impactVFXPrefab;
    [SerializeField] private Vector3 meteorSpawnOffset = new Vector3(3f, 8f, 0f);
    [SerializeField] private float meteorFallDuration = .8f;
    [SerializeField] private float castDelay = .5f;
    [SerializeField] private float impactDuration = .5f;

    protected override IEnumerator PerformAttack(EnemyAttack attack)
    {
        if (attack.type == AttackType.Meteor)
        {
            yield return MeteorAttack(attack);
        }
        else
        {
            yield return base.PerformAttack(attack);
        }
    }

    private IEnumerator MeteorAttack(EnemyAttack attack)
    {
        if (player == null || meteorPrefab == null)
        {
            Debug.LogWarning("Player tidak ditemukan atau Meteor Prefab kosong", this);
            yield break;
        }

        animationController.Play(attack.animationName);
        yield return new WaitForSeconds(castDelay);

        Vector3 impactPosition = player.transform.position;
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