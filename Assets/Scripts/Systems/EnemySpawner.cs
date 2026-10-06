using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy Spawn(Enemy enemyPrefab, Transform parent)
    {
        if (enemyPrefab == null)
        {
            return null;
        }

        return Instantiate(enemyPrefab, transform.position, Quaternion.identity, parent);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}