using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private EncounterData encounter;
    [SerializeField] private List<EnemySpawner> spawners = new List<EnemySpawner>();

    [Tooltip("Titik spawn khusus untuk musuh bertanda Spawn At Back (misalnya boss).")]
    [SerializeField] private EnemySpawner backSpawner;   // BARU

    private readonly List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        SpawnEncounter();
    }

    private void SpawnEncounter()
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

        List<Enemy> backEnemies = new List<Enemy>();
        List<Enemy> otherEnemies = new List<Enemy>();

        foreach (Enemy prefab in composition.enemies)
        {
            if (prefab == null) continue;

            if (prefab.SpawnAtBack)
            {
                backEnemies.Add(prefab);
            }
            else
            {
                otherEnemies.Add(prefab);
            }
        }

        foreach (Enemy prefab in backEnemies)
        {
            EnemySpawner spawner = TakeBackSpawner(availableSpawners);
            if (spawner == null) break;

            SpawnAt(spawner, prefab);
        }

        foreach (Enemy prefab in otherEnemies)
        {
            if (availableSpawners.Count == 0) break;

            EnemySpawner spawner = availableSpawners[0];
            availableSpawners.RemoveAt(0);

            SpawnAt(spawner, prefab);
        }
    }

    private EnemySpawner TakeBackSpawner(List<EnemySpawner> list)   // BARU
    {
        if (backSpawner != null && list.Contains(backSpawner))
        {
            list.Remove(backSpawner);
            return backSpawner;
        }

        return TakeBackmostSpawner(list);
    }

    private EnemySpawner TakeBackmostSpawner(List<EnemySpawner> list)
    {
        if (list.Count == 0) return null;

        EnemySpawner backmost = list[0];

        foreach (EnemySpawner spawner in list)
        {
            if (spawner.transform.position.x > backmost.transform.position.x)
            {
                backmost = spawner;
            }
        }

        list.Remove(backmost);
        return backmost;
    }

    private void SpawnAt(EnemySpawner spawner, Enemy prefab)
    {
        Enemy spawned = spawner.Spawn(prefab, transform);

        if (spawned != null)
        {
            enemies.Add(spawned);
        }
    }

    private void ShuffleSpawners(List<EnemySpawner> list)
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