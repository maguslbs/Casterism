using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private EncounterData encounter;                                   // BARU
    [SerializeField] private List<EnemySpawner> spawners = new List<EnemySpawner>();    // BARU

    private readonly List<Enemy> enemies = new List<Enemy>();                           // UBAH: diisi oleh kode

    private void Start()   // BARU
    {
        SpawnEncounter();
    }

    private void SpawnEncounter()   // BARU
    {
        if (encounter == null)
        {
            Debug.LogWarning("LevelController: Encounter belum diisi!", this);
            return;
        }

        EnemyComposition composition = encounter.PickRandomComposition();

        if (composition == null || composition.enemies.Count == 0)
        {
            Debug.LogWarning($"LevelController: {encounter.name} tidak punya komposisi musuh!", this);
            return;
        }

        List<EnemySpawner> availableSpawners = new List<EnemySpawner>();
        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner != null) availableSpawners.Add(spawner);
        }

        ShuffleSpawners(availableSpawners);

        if (composition.enemies.Count > availableSpawners.Count)
        {
            Debug.LogWarning($"LevelController: komposisi berisi {composition.enemies.Count} musuh, " +
                             $"tapi titik spawn hanya {availableSpawners.Count}. Sisanya tidak di-spawn.", this);
        }

        int spawnCount = Mathf.Min(composition.enemies.Count, availableSpawners.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            Enemy spawned = availableSpawners[i].Spawn(composition.enemies[i], transform);

            if (spawned != null)
            {
                enemies.Add(spawned);
            }
        }
    }

    private void ShuffleSpawners(List<EnemySpawner> list)   // BARU
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void PlayerWin()
    {
        if (AreAllEnemiesDead())
        {
            GameManager.Instance.PlayerWin();
        }
    }

    public List<Enemy> GetAliveEnemies()
    {
        List<Enemy> alive = new List<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy && enemy.IsAlive())
            {
                alive.Add(enemy);
            }
        }

        return alive;
    }

    private bool AreAllEnemiesDead()
    {
        if (enemies.Count == 0)
        {
            Debug.LogWarning("LevelController: belum ada musuh yang di-spawn!", this);
            return false;
        }

        int activeCount = 0;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            activeCount++;

            if (enemy.IsAlive())
            {
                return false;
            }
        }

        if (activeCount == 0)
        {
            Debug.LogWarning("LevelController: tidak ada musuh aktif!", this);
            return false;
        }

        return true;
    }

    private void PlayerLose()
    {

    }
}