using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Melee")]
    public string moveAnimationName = "Walk";
    public float meleeDistance = 4f;
    public float moveDuration = 1f;
    public Vector3 meleeOffset = Vector3.zero;

    [Header("Models")]
    public GameObject prefabs;
}
